using ContactlessEntry.Cloud.Models.DataTransfer;
using ContactlessEntry.Cloud.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Mime;
using System.Threading.Tasks;

namespace ContactlessEntry.Cloud.Controllers
{
    [Authorize]
    [ApiController]
    [Route("v1/[controller]")]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    public sealed class MaintenanceController : ControllerBase
    {
        private readonly IFaceClientService _faceClientService;
        private readonly ILogger<MaintenanceController> _logger;

        public MaintenanceController(
            IFaceClientService faceClientService,
            ILogger<MaintenanceController> logger)
        {
            _faceClientService = faceClientService;
            _logger = logger;
        }

        [HttpPost("person")]
        public async Task<IActionResult> AddPersonAsync(NewPersonDto dto)
        {
            try
            {
                if (null == dto)
                {
                    return BadRequest(dto);
                }

                var person = await _faceClientService.CreatePersonAsync(dto.Name, dto.FaceUrl);
                return Ok(person);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Failed to add person to cognitive servcices.");
                throw;
            }
        }

        [HttpDelete("person/{personId}")]
        public async Task<IActionResult> RemovePersonAsync(string personId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(personId))
                {
                    return BadRequest();
                }

                await _faceClientService.DeletePersonAsync(personId);

                return NoContent();
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Failed to remove person from cognitive servcices.");
                throw;
            }
        }

        [HttpPost("person/{personId}/faces")]
        public async Task<IActionResult> AddFaceAsync(string personId, [FromBody] FaceDto dto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(personId))
                {
                    return BadRequest();
                }

                if (null == dto)
                {
                    return BadRequest(dto);
                }

                var face = await _faceClientService.AddFaceAsync(personId, dto.FaceUrl);

                return Ok(face);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Failed to add face to cognitive servcices.");
                throw;
            }
        }

        [HttpDelete("person/{personId}/faces/{faceId}")]
        public async Task<IActionResult> RemoveFaceAsync(string personId, string faceId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(personId) || string.IsNullOrWhiteSpace(faceId))
                {
                    return BadRequest();
                }

                await _faceClientService.RemoveFaceAsync(personId, faceId);

                return NoContent();
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Failed to remove face from cognitive servcices.");
                throw;
            }
        }

        [HttpPost("train")]
        public async Task<IActionResult> BeginTrainingAsync()
        {
            try
            {
                await _faceClientService.BeginTrainingAsync();

                return Ok();
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Failed to begin training within cognitive servcices.");
                throw;
            }
        }

        [HttpGet("training")]
        [ProducesResponseType(typeof(TrainingStatus), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetTrainingStatusAsync()
        {
            try
            {
                var trainingStatus = await _faceClientService.GetTrainingStatusAsync();

                return Ok(trainingStatus);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Failed to get training status from cognitive servcices.");
                throw;
            }
        }
    }
}
