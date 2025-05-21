namespace salhoubapi.Models
{
    public class Reserv
    {
        public int Id { get; set; }
        public DateTime DateTime { get; set; } = DateTime.Now;
        public int PatientId {  get; set; }
        public Patient patient { get; set; }
        public bool Iscome {  get; set; }
        public decimal price {  get; set; }
        public string UserId { get; set; }

 
        public User User { get; set; }
    }
}
