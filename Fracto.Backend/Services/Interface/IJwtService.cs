using Fracto.Backend.Models;

namespace Fracto.Backend.Services.Interface
{
    public interface IJwtService
    {
        string GenerateToken(User user);
    }
}