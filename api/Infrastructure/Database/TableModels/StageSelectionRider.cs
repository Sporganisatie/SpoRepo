namespace SpoRE.Infrastructure.Database
{
    public class StageSelectionRider
    {
        public int StageSelectionId { get; set; }
        public int RiderParticipationId { get; set; }

        public virtual StageSelectie StageSelection { get; set; }
        public virtual RiderParticipation RiderParticipation { get; set; }
    }
}
