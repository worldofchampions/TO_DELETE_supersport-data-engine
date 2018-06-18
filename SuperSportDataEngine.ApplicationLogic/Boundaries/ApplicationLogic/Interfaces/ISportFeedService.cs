using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperSportDataEngine.ApplicationLogic.Boundaries.ApplicationLogic.Interfaces
{
    public interface ISportFeedService
    {
        Task<bool> IsCategoryInvalid(string category);
    }
}
