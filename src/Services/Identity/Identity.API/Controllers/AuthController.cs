using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Identity.API.Repositories;
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
        private readonly IUserRepository _userRepository;
        private readonly IUserService _userService;
        private readonly ITokenService _tokenService;

        public AuthController(IUserRepository userRepository, IUserService userService, ITokenService tokenService)
        {
            _userRepository = userRepository;
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

        [HttpPost("/login")]
        public ActionResult<string> LoginUser(UserDto userDto)
        {
            if (!_userService.ValidateUser(userDto)) return BadRequest();
            var user = _userRepository.GetUserByUsername(userDto.Username);
            if (user is null) return BadRequest();
            return Ok(_tokenService.GenerateToken(user.Username, user.Scopes));
        }
    }
}
