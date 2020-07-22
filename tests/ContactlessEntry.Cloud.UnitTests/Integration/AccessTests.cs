using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace ContactlessEntry.Cloud.UnitTests.Integration
{
    public class AccessTests : IntegrationTestBase
    {
        public AccessTests(ITestOutputHelper outputHelper, WebApplicationFactory<Startup> factory) : base(outputHelper, factory)
        {
        }

        [Fact]
        public async Task AccessPost_WithHappyPath_ReturnsOk()
        {
            var dto = new
            {
                doorId = $"{Guid.NewGuid()}",
                personId = $"{Guid.NewGuid()}",
                temperature = 36.5
            };

            var token = await GetJwtTokenAsync();
            var response = await RequestService.PostAsync<object, AccessResponse>("/v1/access/request", dto, token);
            response.Should().NotBeNull();
            response.DoorId.Should().NotBeNullOrWhiteSpace();
            response.PersonId.Should().NotBeNullOrWhiteSpace();
            response.Timestamp.Should().BeAfter(DateTime.MinValue);
        }

        class AccessResponse
        {
            public string DoorId { get; set; }

            public string PersonId { get; set; }

            public double Temperature { get; set; }

            public bool Granted { get; set; }

            public DateTime Timestamp { get; set; }
        }
    }
}
