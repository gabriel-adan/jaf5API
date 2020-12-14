using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Authentication.Token.Provider;
using WebApi.Models;

namespace WebApi.Controllers
{
    public class AccountManagerController : ApiBaseController
    {
        private readonly IAuthenticationTokenProvider authenticationTokenProvider;

        public AccountManagerController(ILogger<ApiBaseController> logger, IAuthenticationTokenProvider authenticationTokenProvider) : base(logger)
        {
            this.authenticationTokenProvider = authenticationTokenProvider;
        }

        [HttpPost("LogIn")]
        public IActionResult LogIn([FromForm] UserForm userForm)
        {
            try
            {
                string token = authenticationTokenProvider.LogIn(userForm.UserName, userForm.Password);
                if (!string.IsNullOrEmpty(token))
                    return Ok(new { AccessToken = token });
                else
                    return NotFound();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "LogIn method");
                throw new Exception("Ocurrió un error.");
            }
        }
    }
}
