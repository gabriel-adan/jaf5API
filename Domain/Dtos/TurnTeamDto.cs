using System;

namespace Domain.Dtos
{
    public class TurnTeamDto : EntityDto
    {
        public string TeamName { get; set; }
        public string CampName { get; set; }
        public string Address { get; set; }
        public DateTime Timestamp { get; set; }
        public int PlayersAmount { get; set; }
    }
}
