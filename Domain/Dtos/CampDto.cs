namespace Domain.Dtos
{
    public class CampDto : EntityDto
    {
        public string Name { get; set; }
        public string Street { get; set; }
        public string Number { get; set; }
        public bool IsEnabled { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
    }
}
