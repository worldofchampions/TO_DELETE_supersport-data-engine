using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SuperSportDataEngine.Application.WebApi.SystemApi.Context
{
    /// <summary>
    /// Class to communicate to DB for Identifying Users
    /// </summary>
    public class AuthContext : IdentityDbContext<IdentityUser>
    {
        public AuthContext() : base("AuthContext")
        {

        }
    }
}