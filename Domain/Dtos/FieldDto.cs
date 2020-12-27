namespace Domain.Dtos
{
    public class FieldDto : EntityDto
    {
        public string Name { get; set; }
        public bool IsEnabled { get; set; }
        public int CampId { get; set; }
    }
}
