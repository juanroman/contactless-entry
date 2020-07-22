using ContactlessEntry.Cloud.Configuration;
using ContactlessEntry.Cloud.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace ContactlessEntry.Cloud.Services
{
    public class AccessManager : IAccessManager
    {
        private readonly ILogger<AccessManager> _logger;
        private readonly IOpenDoorService _openDoorService;
        private readonly IAccessRepository _accessRepository;
        private readonly IMicroserviceSettings _applicationSettings;

        public AccessManager(
            ILogger<AccessManager> logger,
            IOpenDoorService openDoorService,
            IAccessRepository accessRepository,
            IMicroserviceSettings applicationSettings)
        {
            _logger = logger;
            _openDoorService = openDoorService;
            _accessRepository = accessRepository;
            _applicationSettings = applicationSettings;
        }

        public Task<Access> RequestAccessAsync(string doorId, string personId, double temperature)
        {
            if (string.IsNullOrWhiteSpace(doorId))
            {
                throw new ArgumentNullException(nameof(doorId));
            }

            if (string.IsNullOrWhiteSpace(personId))
            {
                throw new ArgumentNullException(nameof(personId));
            }

            return RequestAccessAsyncImplementation(doorId, personId, temperature);
        }

        private async Task<Access> RequestAccessAsyncImplementation(string doorId, string personId, double temperature)
        {
            bool granted = GrantAccess(temperature);

            _logger.LogDebug("Storing access in database.");
            var access = await RecordInRepository(doorId, personId, granted, temperature);

            if (granted)
            {
                _logger.LogDebug("Storing access in database.");
                await _openDoorService.OpenDoorAsync(doorId, personId);
            }
            else
            {
                _logger.LogDebug("Access not granted due to temperature check.");
            }

            return access;
        }

        private bool GrantAccess(double temperature) => temperature <= _applicationSettings.MaxAllowedTemperature;

        private Task<Access> RecordInRepository(string doorId, string personId, bool granted, double temperature)
        {
            try
            {
                var newAccess = new Access
                {
                    DoorId = doorId,
                    Granted = granted,
                    PersonId = personId,
                    Temperature = temperature,
                    Timestamp = DateTime.UtcNow
                };

                return _accessRepository.CreateAccessAsync(newAccess);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Failed to record access in data repository.");
                throw;
            }
        }
    }
}
