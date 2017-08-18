namespace SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models
{
    using System;
    using System.Collections.Generic;

    // TODO: Temporary example reference code for team (implement all required fields correctly later etc.).
    public class Sport
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public virtual List<Player> Players { get; set; }
    }
}