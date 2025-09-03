import { Component } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { AuthService } from '../auth-service';
import { Router, RouterLink } from '@angular/router';

@Component({
  selector: 'app-login-component',
  imports: [ReactiveFormsModule,RouterLink],
  templateUrl: './login-component.html',
  styleUrl: './login-component.css'
})
export class LoginComponent {
  hover: boolean = false;
  loginForm: FormGroup;
  errorMessage: string = '';

  constructor(
    private readonly fb : FormBuilder,
    private readonly authService: AuthService,
    private readonly router: Router
  ){
    //Initialize the reactive form
    this.loginForm = this.fb.group({
      username: ['',[Validators.required]],
      password: ['',[Validators.required]]
    });
  }

  async onSubmit(): Promise<void> {
    if (this.loginForm.invalid) {
      this.errorMessage = 'Please enter both username and password.';
      return;
    }

    this.errorMessage = '';
    const credentials = this.loginForm.value;

    try {
      // Call the login method from our authService
      const response = await this.authService.login(credentials);
      console.log('Login successful', response);

      // On success, check the role and navigate to the correct dashboard
      const role = this.authService.getRole();
      if (role === 'Admin') {
        this.router.navigate(['/admin-dashboard']);
      } else {
        this.router.navigate(['/doctor-search']); // Or your main user dashboard
      }
    } catch (error) {
      console.error('Login failed', error);
      this.errorMessage = 'Invalid username or password. Please try again.';
    }
  }
}
