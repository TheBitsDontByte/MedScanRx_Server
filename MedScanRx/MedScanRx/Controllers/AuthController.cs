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

		private const string UserNameClaim = "userName";
		private const string PatientIdClaim = "patientId";

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
							new Claim(UserNameClaim, loginRequest.UserName),
							new Claim(ClaimTypes.Role, "MedScanRx_Admin")
					};

					return Ok(new { token = CreateToken(claims, DateTime.Now.AddMinutes(90)) });
				}

				return Unauthorized();
			}
			catch
			{
				return StatusCode(500, new { errorMessage = "Something went wrong logging in" });
			}
		}

		[HttpGet]
		[Authorize(Roles = "MedScanRx_Patient")]
		[Route("Patient/Refresh")]
		public IActionResult RefreshToken(string fcmToken)
		{
			try
			{

				_bll.UpdateFcmToken(fcmToken);
				var patientId = this.User.Claims.First(c => c.Type == "patientId").Value;
				var email = this.User.Claims.First(c => c.Type == ClaimTypes.Email).Value;

				var claims = new[]
				{
					new Claim(ClaimTypes.Email, email),
					new Claim(PatientIdClaim, patientId),
					new Claim(ClaimTypes.Role, "MedScanRx_Patient")
				};

				return Ok(new { token = CreateToken(claims, DateTime.Now.AddDays(60)) });

			}
			catch (Exception ex)
			{
				return StatusCode(500, new { errorMessage = "Something went wrong refreshing authentication" });

			}
		}

		[HttpPost]
		[AllowAnonymous]
		[Route("Patient/Login")]
		public IActionResult Patient([FromBody] LoginRequest loginRequest)
		{
			try
			{
				if (_bll.AuthenticatePatient(loginRequest))
				{
					_bll.UpdateFcmToken(loginRequest.FcmToken);
					int patientId = _bll.GetPatientId(loginRequest);
					var claims = new[]
					{
							new Claim(ClaimTypes.Email, loginRequest.UserName),
							new Claim(PatientIdClaim, patientId.ToString()),
							new Claim(ClaimTypes.Role, "MedScanRx_Patient")
					};

					return Ok(new { token = CreateToken(claims, DateTime.Now.AddDays(60)) });
				}

				return Unauthorized();
			}
			catch
			{
				return StatusCode(500, new { errorMessage = "Something went wrong logging in" });
			}
		}

		[Authorize(Roles = "MedScanRx_Admin, MedScanRX_Patient")]
		[Route("Test")]
		public IActionResult test()
		{
			return Ok();
		}


		//Move this somewhere ? Seems weird for a controller
		private string CreateToken(Claim[] claims, DateTime expirationTime)
		{

			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["SecurityKey"]));
			var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

			var token = new JwtSecurityToken(
					issuer: "medscanrx.com",
					audience: "medscanrx.com",
					claims: claims,
					expires: expirationTime,
					signingCredentials: creds);

			return new JwtSecurityTokenHandler().WriteToken(token);
		}

	}
}