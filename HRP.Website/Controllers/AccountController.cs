namespace HRP_Website.Controllers
{
    using System.Diagnostics;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Cors;
    using Newtonsoft.Json.Linq;
    using HRP.Services;
    using HRP.Models;    
    
    [Route("api/[controller]")]
    public class AccountController : Controller
    {
        private readonly IUserService _userService;
        private readonly ITokenService _tokenService;

        public AccountController(
            IUserService userService,
            ITokenService tokenService)
        {
            _tokenService = tokenService;
            _userService = userService;
        }

        [AllowAnonymous]
        [HttpPost("[action]")]
        public IActionResult Login([FromBody]LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _userService.GetUserByUsername(model.Username);

                if (user != null)
                {
                    var accessToken = _tokenService.CreateJwtTokens(user);
                    user.Token = accessToken;

                    return Ok(user);
                }
            }

            return Unauthorized();
        }

        [AllowAnonymous]
        [HttpGet("[action]"), HttpPost("[action]")]
        public bool Logout()
        {
            return true;
        }
    }
}
