namespace SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models
{
    /// <summary>
    /// A definition table to track references to a particular data provider.
    /// This will be required once there is data for more than one provider for a particular sport.
    /// </summary>
    public class DataProvider
    {
        /// <summary> A manually assigned int primary key to identify a data provider. </summary>
        public int Id { get; set; }

        /// <summary> A fixed code for lookup purposes. Use this for any lookups against defined string constants. </summary>
        public string Code { get; set; }

        /// <summary> A name for descriptive/debug purposes. Do not do any lookups against this value. </summary>
        public string Name { get; set; }
    }
}