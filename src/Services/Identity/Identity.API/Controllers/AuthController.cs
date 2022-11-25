using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Identity.API.Services;
using Identity.Common.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Identity.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ITokenService _tokenService;

        public AuthController(IUserService userService, ITokenService tokenService)
        {
            _userService = userService;
            _tokenService = tokenService;
        }

        [HttpPost("/register")]
        public ActionResult<string> RegisterUser(UserDto userDto)
        {
            var id = _userService.RegisterUser(userDto);
            var scopes = _userService.GetScopesOfUser(id);
            return _tokenService.GenerateToken(userDto.Username, scopes);
        }
    }
}
