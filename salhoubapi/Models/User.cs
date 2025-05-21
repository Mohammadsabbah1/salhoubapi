using Microsoft.AspNetCore.Identity;

namespace salhoubapi.Models
{
    public class User: IdentityUser
    {
        public float Percantage { get; set; }=0;
        public string Name { get; set; }
        public string Role { get; set; }
        public ICollection<Reserv> Reservs { get; set; } = new List<Reserv>();

    }
}
