using ContactlessEntry.Cloud.Models;
using ContactlessEntry.Cloud.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace ContactlessEntry.Cloud.Controllers
{
    [Authorize]
    [ApiController]
    [Route("v1/[controller]")]
    [Consumes(MediaTypeNames.Application.Octet)]
    [Produces(MediaTypeNames.Application.Json)]
    public sealed class FaceRecognitionController : ControllerBase
    {
        private readonly IFaceClientService _faceClientService;
        private readonly ILogger<FaceRecognitionController> _logger;

        public FaceRecognitionController(
            IFaceClientService faceClientService,
            ILogger<FaceRecognitionController> logger)
        {
            _faceClientService = faceClientService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult<IList<RecognizedCandidate>>> RecognizeWithStreamAsync([FromBody] Stream image)
        {
            try
            {
                if (null == image)
                {
                    return BadRequest(image);
                }

                var list = await _faceClientService.RecognizeWithStreamAsync(image);
                if (null != list && list.Any())
                {
                    return Ok(list
                        .OrderByDescending(rc => rc.Confidence)
                        .ToList());
                }
                else
                {
                    return Ok(new List<RecognizedCandidate>());
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Failed to recognize stream using cognitive servcices.");
                throw;
            }
        }
    }
}
