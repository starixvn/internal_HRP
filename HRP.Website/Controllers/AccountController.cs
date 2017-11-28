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
    using HRP.Core.Services;
    using HRP.Services;
    using HRP.Models;    
    
    [Route("api/[controller]")]
    public class AccountController : Controller
    {
        private readonly IUserService _userService;
        private readonly ITokenService _tokenService;
        private readonly ISessionService _sessionService;

        public AccountController(
            IUserService userService,
            ITokenService tokenService,
            ISessionService sessionService)
        {
            _tokenService = tokenService;
            _userService = userService;
            _sessionService = sessionService;
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

                    _sessionService.UserId = user.UserId;
                    _sessionService.Username = user.Username;
                    _sessionService.MenuItems = user.MenuItems;
                    _sessionService.Functions = user.Functions;

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
