namespace salhoubapi.Models
{
    public class MedicalCondition
    {
        public string Id { get; set; }  
        public string Name { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public string TypeDescription { get; set; } = string.Empty;

    }
}

