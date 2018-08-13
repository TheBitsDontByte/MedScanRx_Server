using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using MedScanRx.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace MedScanRx.Controllers
{
    [Produces("application/json")]
    [Route("api/Auth/")]
    public class AuthController : Controller
    {
        IConfiguration _configuration;

        public AuthController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("Token")]
        public IActionResult Token([FromBody]LoginRequest request)
        {
            if (request.UserName == "Chris" && request.Password == "wow")
            {
                var claims = new[]
                {
                    new Claim(ClaimTypes.Name, request.UserName),
                    new Claim("AdminClaimTest", "")
                };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["SecurityKey"]));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                    issuer: "medscanrx.com",
                    audience: "medscanrx.com",
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(30),
                    signingCredentials: creds);

                return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token) });
            }

            return BadRequest(new { error = "Invalid login request" });
        }

        [Authorize]
        [HttpGet]
        [Route("TestAuth")]
        public IActionResult TestAuth()
        {
            return Ok("Testing this is authorized");
        }

        [Authorize(Policy = "Admin")]
        [HttpGet]
        [Route("TestAdmin")]
        public IActionResult TestAdmin()
        {
            var x = this;
            return Ok("Testing this is authorized");
        }

    }
}