using System;

namespace Domain.Dtos
{
    public class TurnResultDto : EntityDto
    {
        public DateTime DateTime { get; set; }
        public string Name { get; set; }
        public string Field { get; set; }
    }
}
