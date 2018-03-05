using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;

namespace SuperSportDataEngine.Application.WebApi.SystemApi.Authentication
{
    /// <summary>
    /// Class to handle API Authentication
    /// </summary>
    public class ApiSecurity
    {
        /// <summary>
        /// Method that accepts cms key and check if it matches with the one in the webconfig
        /// </summary>
        /// <param name="key"></param>
        /// <returns>true or false</returns>
        public static bool VaidateUser(string key)
        {
            // Check if it is valid credential  
            if (WebConfigurationManager.AppSettings["CmsKey"] == key)  
                return true;
            return false;
        }
    }
}