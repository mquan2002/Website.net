namespace Final.net.Models
{
    public class Store
    {
        public int Id { get; set; }  
        public string Name { get; set; }  
        public string Address { get; set; }  
        public string Description { get; set; }  
        public double Latitude { get; set; }  // Vĩ độ (số nguyên)
        public double Longitude { get; set; }  // Kinh độ (số nguyên)
    }
}
