using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.Common.Models.Enums;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models;
using SuperSportDataEngine.ApplicationLogic.Boundaries.Repository.EntityFramework.PublicSportData.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperSportDataEngine.ApplicationLogic.Entities.SystemAPI
{
    public class RugbySeasonEntity
    {
        /// <summary> The provider's record identifier. </summary>
        public int ProviderSeasonId { get; set; }

        /// <summary> A provider driven value. </summary>
        public string Name { get; set; }

        /// <summary> A CMS driven value to set whether the season is current. </summary>
        public bool IsCurrent { get; set; }

        /// <summary> A provider driven value. </summary>
        public DateTimeOffset StartDateTime { get; set; }

        /// <summary> A provider driven value. </summary>
        public DateTimeOffset EndDateTime { get; set; }

        /// <summary> 
        /// A provider driven value for the current round number for the season.
        /// The exception for this being in a Sevens tournament,
        /// the CurrentRoundNumber is CMS driven.
        /// </summary>
        public int CurrentRoundNumber { get; set; }
    }
}
