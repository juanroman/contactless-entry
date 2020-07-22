using AutoMapper;
using ContactlessEntry.Cloud.Models.DataTransfer;
using ContactlessEntry.Cloud.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Net.Mime;
using System.Threading.Tasks;

namespace ContactlessEntry.Cloud.Controllers
{
    [Authorize]
    [ApiController]
    [Route("v1/[controller]")]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    public class AccessController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IAccessManager _accessManager;
        private readonly ILogger<AccessController> _logger;

        public AccessController(
            IMapper mapper,
            IAccessManager accessManager,
            ILogger<AccessController> logger)
        {
            _logger = logger;
            _mapper = mapper;
            _accessManager = accessManager;
        }

        /// <summary>
        /// Handles a request access operation.
        /// </summary>
        /// <param name="dto">The <c>Access</c> DTO.</param>
        /// <response code="200">When the request is handled successfully.</response>
        /// <response code="400">When the DTO is invalid.</response>
        /// <response code="400">When the request is not properly authenticated.</response>
        /// <response code="500">When an error occurs in the service.</response>
        [HttpPost("request")]
        [ProducesResponseType(typeof(AccessDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> RequestAccessAsync([FromBody] RequestAccessDto dto)
        {
            if (null == dto)
            {
                return BadRequest(dto);
            }

            _logger.LogDebug("Processing RequestAccessAsync");
            var access = await _accessManager.RequestAccessAsync(dto.DoorId, dto.PersonId, dto.Temperature);

            return Ok(_mapper.Map<AccessDto>(access));
        }
    }
}
