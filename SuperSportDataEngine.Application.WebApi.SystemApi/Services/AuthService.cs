using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using SuperSportDataEngine.Application.WebApi.SystemApi.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using SuperSportDataEngine.Application.WebApi.SystemApi.Models;

namespace SuperSportDataEngine.Application.WebApi.SystemApi.Services
{
    /// <summary>
    /// Auth service class to handle authentication methods
    /// This is inside the SystemAPI project because of the OWIN packages specific
    /// to this project
    /// </summary>
    public class AuthService : IDisposable
    {
        private AuthContext _authContext;

        private UserManager<IdentityUser> _userManager;

        /// <summary>
        /// AuthService default Constructor
        /// </summary>
        public AuthService()
        {
            _authContext = new AuthContext();
            _userManager = new UserManager<IdentityUser>(new UserStore<IdentityUser>(_authContext));
        }

        /// <summary>
        /// Create a new user
        /// </summary>
        /// <param name="userModel"></param>
        /// <returns></returns>
        public async Task<IdentityResult> RegisterUser(UserModel userModel)
        {
            IdentityUser user = new IdentityUser
            {
                UserName = userModel.UserName
            };

            var result = await _userManager.CreateAsync(user, userModel.Password);

            return result;
        }

        /// <summary>
        /// Attempt to find an existing user
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task<IdentityUser> FindUser(string userName, string password)
        {
            IdentityUser user = await _userManager.FindAsync(userName, password);

            return user;
        }

        /// <summary>
        /// Dispose DB connections
        /// </summary>
        public void Dispose()
        {
            _authContext.Dispose();
            _userManager.Dispose();
        }
    }
}