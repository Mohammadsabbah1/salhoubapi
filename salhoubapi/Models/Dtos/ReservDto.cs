namespace salhoubapi.Models.Dtos
{
    public class ReservDto
    {
       
            public string UserName { get; set; } 
            public decimal Price { get; set; } // سعر الحجز
            public DateTime Date { get; set; } // تاريخ الحجز
        
    }
}
