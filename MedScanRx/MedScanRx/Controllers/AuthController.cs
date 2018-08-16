using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using MedScanRx.BLL;
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
        private IConfiguration _configuration;

        Auth_BLL _bll;
        public AuthController(IConfiguration configuration)
        {
            _configuration = configuration;
            _bll = new Auth_BLL(configuration.GetConnectionString("MedScanRx_AWS"));
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("Admin/Login")]
        public IActionResult Admin([FromBody] LoginRequest loginRequest)
        {
            try
            {
                if (_bll.AuthenticateAdmin(loginRequest))
                {

                    var claims = new[]
                    {
                        new Claim(ClaimTypes.Name, loginRequest.UserName),
                        new Claim("userName", loginRequest.UserName),
                        new Claim(ClaimTypes.Role, "MedScanRx_Admin")
                    };

                    return Ok(new { token = CreateToken(claims) });
                }

                return Unauthorized();
            }
            catch
            {
                return StatusCode(500, new { errorMessage = "Something went wrong logging in" });
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("Patient/Login")]
        public IActionResult Patient([FromBody] LoginRequest loginRequest)
        {
            try
            {
                if (_bll.AuthenticateAdmin(loginRequest))
                {

                    var claims = new[]
                    {
                        new Claim(ClaimTypes.Name, loginRequest.UserName),
                        new Claim(ClaimTypes.Role, "MedScanRx_Patient")
                    };

                    return Ok(new { token = CreateToken(claims) });
                }

                return Unauthorized();
            }
            catch
            {
                return StatusCode(500, new { errorMessage = "Something went wrong logging in" });
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("Token")]
        //DONT USE
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
                    expires: DateTime.UtcNow.AddMinutes(30),
                    signingCredentials: creds);

                return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token) });
            }

            return BadRequest(new { error = "Invalid login request" });
        }

        [Authorize(Roles = "MedScanRx_Admin, MedScanRX_Patient")]
        [Route("Test")]
        public IActionResult test()
        {
            return Ok();
        }

        private string CreateToken(Claim[] claims)
        {

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["SecurityKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "medscanrx.com",
                audience: "medscanrx.com",
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}