using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using salhoubapi.Models;
using Microsoft.Extensions.Configuration;
using System.IO;
using salhoubapi.Models.Dtos;
namespace salhoubapi.Services
{


    public class AuthService : IAuthService
    {
        private readonly IConfiguration _configuration;
        private readonly AppDbContext _context;


        public AuthService(IConfiguration configuration, AppDbContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto)
        {
            // Check if the email already exists in the database
            var userExists = await _context.Users
                .AnyAsync(user => user.Email == registerDto.Email);

            if (userExists)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "User already exists"
                };
            }

            // Save the new user to the database
            var newUser = new User
            {

                //EmailConfirmed=registerDto.EmailConfirmed,
                Email = registerDto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password),
                PhoneNumber = registerDto.PhoneNumber,
                Name = registerDto.Name,
                Role = registerDto.Role,
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            return new AuthResponseDto
            {
                Success = true,
                Message = "User registered successfully"
            };
        }
        public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
        {
            // Check if the user exists in the database
            var user = await _context.Users
                .FirstOrDefaultAsync(user => user.Email == loginDto.Email);

            if (user == null || !VerifyPassword(user.PasswordHash, loginDto.Password))
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Invalid credentials"
                };
            }

            // Generate JWT token for the authenticated user
            var token = GenerateJwtToken(user);

            return new AuthResponseDto
            {
                Success = true,
                Token = token,
                Message = "Login successful"
            };
            //}

            //public string GenerateJwtToken(User user)
            //{
            //    if (user == null) throw new ArgumentNullException(nameof(user), "User cannot be null.");

            //    var secretKey = Configuration["Jwt:SecretKey"];
            //    if (string.IsNullOrEmpty(secretKey))
            //    {
            //        throw new ArgumentNullException("Jwt:SecretKey", "Secret key is not configured.");
            //    }

            //    var keyBytes = System.Text.Encoding.UTF8.GetBytes(secretKey);

            //    var tokenHandler = new JwtSecurityTokenHandler();
            //    var tokenDescriptor = new SecurityTokenDescriptor
            //    {
            //        Subject = new ClaimsIdentity(new[]
            //        {
            //    new Claim(ClaimTypes.Name, user.Username),
            //    new Claim("UserId", user.Id.ToString())
            //}),
            //        Expires = DateTime.UtcNow.AddDays(7),
            //        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(keyBytes), SecurityAlgorithms.HmacSha256Signature)
            //    };

            //    var token = tokenHandler.CreateToken(tokenDescriptor);
            //    return tokenHandler.WriteToken(token);
        }


        private bool VerifyPassword(string hashedPassword, string password)
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }

        //private string GenerateJwtToken(User user)
        //{
        //    if (user == null) throw new ArgumentNullException(nameof(user));

        //    var claims = new[]
        //    {

        //      new Claim(ClaimTypes.Name, user.Email), // Use user's email or username
        //      new Claim(ClaimTypes.Role, "DefaultRole"),
        //        new Claim("UserId", user.Id.ToString()) // Custom claim
        // };

        //    var jwtKey = _configuration["Jwt:Key"] ?? throw new ArgumentNullException("Jwt:Key not found in configuration.");
        //    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        //    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        //    var token = new JwtSecurityToken(
        //        issuer: _configuration["Jwt:Issuer"],
        //        audience: _configuration["Jwt:Audience"],
        //        claims: claims,
        //        expires: DateTime.UtcNow.AddMinutes(30),
        //        signingCredentials: creds);

        //    return new JwtSecurityTokenHandler().WriteToken(token);
        //}

        private string GenerateJwtToken(User user)
        {
          
            if (user == null) throw new ArgumentNullException(nameof(user));
            var validRoles = new[] { "User", "Admin" };
            var userRole = validRoles.Contains(user.Role) ? user.Role : "User"; // Re

            var claims = new[]
            {
        new Claim(ClaimTypes.Name, user.Email), // Use user's email
        new Claim(ClaimTypes.Role, user.Role ), // Include the user's role
        new Claim("UserId", user.Id.ToString()) // Custom claim for User ID
    };

            var jwtKey = _configuration["Jwt:Key"] ?? throw new ArgumentNullException("Jwt:Key not found in configuration.");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(30),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}
