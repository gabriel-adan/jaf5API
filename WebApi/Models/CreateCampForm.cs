namespace WebApi.Models
{
    public class CreateCampForm
    {
        public int CustomerId { get; set; }
        public string Name { get; set; }
        public string Street { get; set; }
        public string Number { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
    }
}
