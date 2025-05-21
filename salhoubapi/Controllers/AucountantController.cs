using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using salhoubapi.Models;

namespace salhoubapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AucountantController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AucountantController(AppDbContext context)
        {
            _context = context;
        }
        [Authorize(Roles ="Admin")]
        [HttpGet("24get")]
        public async Task<string> GetPricesStringFromLast24HoursAsync()
        {

            var last24Hours = DateTime.UtcNow.AddHours(-24);

            // Ensure you use the correct entity model and property names
            var prices = await _context.acountings
                .Where(a => a.dateTime >= last24Hours)
                .Select(a => a.Price.ToString()) // Convert each price to string
                .ToListAsync();

            // Join prices into a single string separated by commas
            return string.Join(",", prices);

        }
        [Authorize(Roles ="Admin")]
        [HttpGet("30dget")]
        public async Task<string> getprice30days()
        {
            var last30days = DateTime.UtcNow.AddDays(-30);
            var prices=await _context.acountings
                .Where(a => a.dateTime >= last30days)
                .Select(a => a.Price.ToString()) // Convert each price to string
                .ToListAsync();
            return string.Join(",", prices);
        }
        [Authorize(Roles ="Admin")]
        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _context.Users
                .Select(user => new
                {
                    user.Name,
                    user.Percantage,
                    user.Role
                }).ToListAsync();

            return Ok(users);
        }
        [Authorize(Roles ="Admin")]
        [HttpPost("update-percentage/{userName}")]
        public async Task<IActionResult> UpdateUserPercentage(string userName, [FromBody] float newPercentage)
        {
            // التحقق من صلاحيات المسؤول
            var currentUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == User.Identity.Name);
            if (currentUser == null || currentUser.Role != "Admin")
            {
                return Unauthorized("Only admins can update percentages.");
            }

            // البحث عن المستخدم بالاسم
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Name == userName);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            // تحديث النسبة
            user.Percantage = newPercentage;
            await _context.SaveChangesAsync();

            return Ok("Percentage updated successfully.");
        }
        [HttpGet("user-percentage/{userName}")]
        public async Task<IActionResult> CalculateUserPercentage(string userName)
        {
            var user = await _context.Users
                .Include(u => u.Reservs)
                .FirstOrDefaultAsync(u => u.Name == userName);

            if (user == null)
            {
                return NotFound("User not found.");
            }

            var totalReservations = user.Reservs.Count;
            var totalPrice = user.Reservs.Sum(r => r.price);

            return Ok(new
            {
                UserName = user.Name,
                TotalReservations = totalReservations,
                TotalPrice = totalPrice,
                Percentage = user.Percantage
            });
        }

    }

}
