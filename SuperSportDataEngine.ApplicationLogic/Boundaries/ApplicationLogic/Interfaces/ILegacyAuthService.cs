using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperSportDataEngine.ApplicationLogic.Boundaries.ApplicationLogic.Interfaces
{
    public interface ILegacyAuthService
    {
        bool IsAuthorised(string authKey, string siteId = null);
    }
}
