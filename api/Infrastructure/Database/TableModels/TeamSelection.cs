namespace SpoRE.Infrastructure.Database
{
    public class TeamSelection
    {
        public int AccountParticipationId { get; set; }
        public int RiderParticipationId { get; set; }

        public virtual AccountParticipation AccountParticipation { get; set; }
        public virtual RiderParticipation RiderParticipation { get; set; }
    }
}
