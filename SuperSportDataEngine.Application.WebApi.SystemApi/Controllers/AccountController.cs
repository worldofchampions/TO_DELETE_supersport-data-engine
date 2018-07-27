using Microsoft.AspNet.Identity;
using SuperSportDataEngine.Application.WebApi.SystemApi.Models;
using SuperSportDataEngine.Application.WebApi.SystemApi.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace SuperSportDataEngine.Application.WebApi.SystemApi.Controllers
{
    /// <summary>
    /// Account Controller for Managing Users
    /// !!! Not using this right now, but don't delete as plan for future usage !!!
    /// </summary>
    public class AccountController : ApiController
    {
        private readonly AuthService _authService = null;

        /// <summary>
        /// Account default constructor
        /// </summary>
        public AccountController()
        {
            _authService = new AuthService();
        }

        /// <summary>
        /// Create new user, that will be verified when granting token
        /// </summary>
        /// <param name="userModel"></param>
        /// <returns></returns>
        // POST api/Account/Register
        [AllowAnonymous]
        [ActionName("Register")]
        public async Task<IHttpActionResult> Register(UserModel userModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            IdentityResult result = await _authService.RegisterUser(userModel);

            IHttpActionResult errorResult = GetErrorResult(result);

            if (errorResult != null)
            {
                return errorResult;
            }

            return Ok();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _authService.Dispose();
            }

            base.Dispose(disposing);
        }

        private IHttpActionResult GetErrorResult(IdentityResult result)
        {
            if (result == null)
            {
                return InternalServerError();
            }

            if (!result.Succeeded)
            {
                if (result.Errors != null)
                {
                    foreach (string error in result.Errors)
                    {
                        ModelState.AddModelError("", error);
                    }
                }

                if (ModelState.IsValid)
                {
                    // No ModelState errors are available to send, so just return an empty BadRequest.
                    return BadRequest();
                }

                return BadRequest(ModelState);
            }

            return null;
        }
    }
}
