namespace SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models
{
    public abstract class MotorSpeed
    {
        public double Value { get; set; }
        public string SpeedUnitId { get; set; }
        public string SpeedUnitAbbreviation { get; set; }
    }
}