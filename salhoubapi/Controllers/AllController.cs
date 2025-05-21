using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using salhoubapi.Models;
using salhoubapi.Models.Dtos;

namespace salhoubapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AllController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AllController(AppDbContext context)
        {
            _context = context;
        }
        [HttpGet("Items")]
        public async Task<IActionResult> GetAllItems()
        {
            var items = await _context.Items.ToListAsync();
            if (items == null || !items.Any()) // Check if the list is null or empty
            {
                return BadRequest("no items");
            }
            return Ok(items);
        }
        [HttpGet("Category")]
        public async Task<IActionResult> GetAllCategorys()
        {
            var items = await _context.Categories2.ToListAsync();
            if (items == null || !items.Any()) // Check if the list is null or empty
            {
                return BadRequest("no category");
            }
            return Ok(items);
        }
        [Route("item{id}")]
        [HttpGet] // Define the route parameter
        public async Task<IActionResult> GetItemByIdAsync(int id)
        {
            var item = await _context.Items.FindAsync(id);

            if (item == null)
            {
                return NotFound("no item this id"); // Return 404 if the item is not found
            }

            return Ok(item); // Return the item with HTTP 200
        }
        [Route("Category{id}")]
        [HttpGet] // Define the route parameter
        public async Task<IActionResult> GetCatByIdAsync(int id)
        {
            var Cat = await _context.Items.FindAsync(id);

            if (Cat == null)
            {
                return NotFound("no item this id"); // Return 404 if the item is not found
            }

            return Ok(Cat); // Return the item with HTTP 200
        }
        [Authorize]
        [Route("item/AddItem")]
        [HttpPost]
        public async Task<IActionResult> AddItemAsync(string Name, double Price, double Quantity)
        {

            // Validate input parameters
            if (string.IsNullOrWhiteSpace(Name))
            {
                return BadRequest("Name is required.");
            }

            if (Price <= 0 || Quantity <= 0)
            {
                return BadRequest("Price and Quantity must be greater than zero.");
            }

            // Create a new item object
            var newItem = new Items
            {
                Name = Name,
                Price = Price,
                Quantity = Quantity
            };

            try
            {
                // Save the item to the database
                _context.Items.Add(newItem);
                await _context.SaveChangesAsync();

                return Ok(new { Message = "Item added successfully.", Item = newItem });
            }
            catch (Exception ex)
            {
                // Handle errors
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [Authorize]
        [Route("AddCategory")]
        [HttpPost]
        public async Task<IActionResult> AddCategoryasync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return BadRequest("Name is required.");
            }
            var newCAt = new Category
            {
                Name = name
            };
            try
            {
                // Save the item to the database
                _context.Categories2.Add(newCAt);
                await _context.SaveChangesAsync();

                return Ok(new { Message = "Item added successfully.", Category = newCAt });
            }
            catch (Exception ex)
            {
                // Handle errors
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }

        }
        [Authorize]
        [HttpGet("reservs")]
        public async Task<IActionResult> GetAllReservs()
        {
            var reservs = await _context.Reservations
                .Include(r => r.User)
                .Select(r => new
                {
                    r.Id,
                    r.price,
                    r.DateTime,
                    UserName = r.User.Name
                }).ToListAsync();

            return Ok(reservs);
        }

        [Authorize]
        [HttpGet("reservss/{userName}")]
        public async Task<IActionResult> GetUserReservs(string userName)
        {
            var reservs = await _context.Reservations
                .Include(r => r.User)
                .Where(r => r.User.Name == userName)
                .Select(r => new
                {
                    r.Id,
                    r.price,
                    r.DateTime
                }).ToListAsync();

            if (!reservs.Any())
            {
                return NotFound("No reservations found for the user.");
            }

            return Ok(reservs);
        }
        [Authorize]
        [HttpPut("update-reserv/{id}")]
        public async Task<IActionResult> UpdateReserv(int id, [FromBody] ReservDto reservDto)
        {
            var reserv = await _context.Reservations.FirstOrDefaultAsync(r => r.Id == id);
            if (reserv == null)
            {
                return NotFound("Reservation not found.");
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Name == reservDto.UserName);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            reserv.price = reservDto.Price;
            reserv.DateTime = reservDto.Date;
            reserv.UserId = user.Id;

            await _context.SaveChangesAsync();

            return Ok("Reservation updated successfully.");
        }
        [Authorize]
        [HttpDelete("delete-reserv/{id}")]
        public async Task<IActionResult> DeleteReserv(int id)
        {
            var reserv = await _context.Reservations.FirstOrDefaultAsync(r => r.Id == id);
            if (reserv == null)
            {
                return NotFound("Reservation not found.");
            }

            _context.Reservations.Remove(reserv);
            await _context.SaveChangesAsync();

            return Ok("Reservation deleted successfully.");
        }

        //[HttpGet("{id}")]
    }

}

