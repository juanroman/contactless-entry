using ContactlessEntry.Cloud.Configuration;
using ContactlessEntry.Cloud.Models.DataTransfer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Mime;
using System.Text;

namespace ContactlessEntry.Cloud.Controllers
{
    [ApiController]
    [Route("v1/[controller]")]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    public sealed class AuthenticateController : ControllerBase
    {
        private readonly IMicroserviceSettings _microserviceSettings;

        public AuthenticateController(IMicroserviceSettings microserviceSettings) => _microserviceSettings = microserviceSettings;

        /// <summary>
        /// Authenticates a set of API Credentials and generates a JWT Token.
        /// </summary>
        /// <param name="dto">The <c>API Credentials</c> DTO.</param>
        /// <response code="200">When the request is handled successfully.</response>
        /// <response code="400">When the DTO is invalid.</response>
        /// <response code="401">When the request is not properly authenticated.</response>
        [AllowAnonymous]
        [HttpPost]
        [ProducesResponseType(typeof(AuthenticateResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult Authenticate([FromBody] ApiCredentialsDto dto)
        {
            if (null == dto)
            {
                return BadRequest();
            }

            if (string.IsNullOrWhiteSpace(dto.ApiKey) || string.IsNullOrWhiteSpace(dto.ApiSecret))
            {
                return Unauthorized();
            }

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_microserviceSettings.JwtKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(_microserviceSettings.JwtIssuer, _microserviceSettings.JwtIssuer, null, expires: DateTime.Now.AddMinutes(90), signingCredentials: credentials);

            return Ok(new AuthenticateResponseDto
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token)
            });
        }
    }
}
