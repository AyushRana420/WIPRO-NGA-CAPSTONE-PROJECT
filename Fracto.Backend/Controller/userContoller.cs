using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Fracto.Data;
using Fracto.Backend.Models;
using Microsoft.AspNetCore.Identity;
using Fracto.Backend.Services.Interface;
using LoginRequest = Fracto.Backend.DTO.LoginRequest;
using Fracto.Backend.DTO;

namespace Fracto.Backend.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly FractoDbContext _context;
        private readonly IJwtService _jwtService;
        private readonly IFileService _fileService;

        public UserController(FractoDbContext context, IJwtService jwtService, IFileService fileService)
        {
            _context = context;
            _jwtService = jwtService;
            _fileService = fileService;
        }

        // ================= USER & ADMIN REGISTRATION =================

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromForm] RegisterRequest request, IFormFile? profileImage)
        {
            if (await _context.Users.AnyAsync(u => u.username == request.username))
                return BadRequest("Username already exists.");

            var user = new User
            {
                username = request.username,
                // Role is set from the request. Defaults to "User" in the DTO.
                // An admin could potentially pass "Admin" here if needed.
                role = request.role
            };

            // Hash password
            var passwordHasher = new PasswordHasher<User>();
            user.password = passwordHasher.HashPassword(user, request.password);

            // Handle profile image
            if (profileImage != null)
            {
                user.profileImagePath = await _fileService.SaveImageAsync(profileImage, "users");
            }
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return Ok("User registered successfully.");
        }

        // ================= USER SIDE =================

        // POST: api/User/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            if (loginRequest == null || string.IsNullOrWhiteSpace(loginRequest.username) || string.IsNullOrWhiteSpace(loginRequest.password))
                return BadRequest("Username and password are required.");

            var user = await _context.Users.FirstOrDefaultAsync(u => u.username == loginRequest.username);
            if (user == null)
                return Unauthorized("Invalid username or password.");

            var passwordHasher = new PasswordHasher<User>();
            var hashedPassword = user.password ?? string.Empty;
            var result = passwordHasher.VerifyHashedPassword(user, hashedPassword, loginRequest.password);

            if (result == PasswordVerificationResult.Failed)
                return Unauthorized("Invalid username or password.");

            var token = _jwtService.GenerateToken(user);
            return Ok(new { Token = token, Role = user.role });
        }


        // POST: api/User/logout
        [HttpPost("logout")]
        [Authorize]
        public IActionResult Logout()
        {
            return Ok(new { message = "Logout successful. Please remove the token from client storage." });
        }

        // ================= ADMIN SIDE =================

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetUsers()
        {
            // getiing all data from the server but not passing everything
            var usersFromDb = await _context.Users.ToListAsync();

            // basically not passing the password and all so its work on abstraction.
            var UserDto = usersFromDb.Select(user => new UserDto
            {
                id = user.userId,
                username = user.username ?? string.Empty,
                role = user.role ?? "User",
                profileImagePath = user.profileImagePath
            });
            return Ok(UserDto);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetUser(int id)
        {
            var userFromDb = await _context.Users.FindAsync(id);

            if (userFromDb != null)
            {
                var UserDto = new UserDto
                {
                    id = userFromDb.userId,
                    username = userFromDb.username ?? string.Empty,
                    role = userFromDb.role ?? "User",
                    profileImagePath = userFromDb.profileImagePath
                };

                return Ok(UserDto);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] User userUpdate)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            user.username = userUpdate.username;

            if (!string.IsNullOrEmpty(userUpdate.password))
            {
                var passwordHasher = new PasswordHasher<User>();
                user.password = passwordHasher.HashPassword(user, userUpdate.password);
            }

            user.role = userUpdate.role;

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return Ok("User updated successfully.");
        }

        // DELETE: api/User/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            //Added a new feature
            // When deleting a user, also delete their profile image from the server.
            if (!string.IsNullOrEmpty(user.profileImagePath))
            {
                _fileService.DeleteImage(user.profileImagePath);
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return Ok("User deleted successfully.");
        }
    }
}