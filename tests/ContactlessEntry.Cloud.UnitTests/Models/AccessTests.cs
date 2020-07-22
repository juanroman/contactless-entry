using AutoMapper;
using ContactlessEntry.Cloud.Models;
using ContactlessEntry.Cloud.Models.DataTransfer;
using ContactlessEntry.Cloud.Utilities;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace ContactlessEntry.Cloud.UnitTests.Models
{
    public class AccessTests
    {
        private readonly IMapper _mapper;

        public AccessTests()
        {
            var mapperConfiguration = new MapperConfiguration(mapperConfiguration =>
            {
                mapperConfiguration.AddProfile(new AutoMapperProfile());
            });

            _mapper = mapperConfiguration.CreateMapper();
        }

        [Fact]
        public void Access_FullyQualified_EqualsToDto()
        {
            var access = new Access
            {
                DoorId = $"{Guid.NewGuid()}",
                Granted = true,
                PersonId = $"{Guid.NewGuid()}",
                Temperature = 35.35,
                Timestamp = DateTime.UtcNow
            };

            var dto = _mapper.Map<AccessDto>(access);
            dto.DoorId.Should().BeEquivalentTo(access.DoorId);
            dto.Granted.Should().Be(access.Granted);
            dto.PersonId.Should().BeEquivalentTo(access.PersonId);
            dto.Temperature.Should().Be(access.Temperature);
            dto.Timestamp.Should().Be(access.Timestamp);
        }
    }
}
