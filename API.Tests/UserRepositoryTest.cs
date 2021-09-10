using API.Data;
using API.DTO;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace API.Tests
{
    public class UserRepositoryTest
    {
        public Mock<IUserRepository> _userRepo = new Mock<IUserRepository>();

        [Fact]
        public async void Test_GetAllMembersAsync()
        {
            var data = new List<AppUser>
            {
                new AppUser { UserName = "Hudson" },
                new AppUser { UserName = "Kevin" },
                new AppUser { UserName = "Matthew" }
            }.AsQueryable();

            var mockSet = new Mock<DbSet<AppUser>>();
            mockSet.As<IQueryable<AppUser>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<AppUser>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<AppUser>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<AppUser>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

            var mockOptions = new Mock<DbContextOptions>();
            var mockContext = new Mock<DataContext>(mockOptions.Object);
            mockContext.Setup(c => c.Users).Returns(mockSet.Object);

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(m => m.ConfigurationProvider).Returns(() =>
                new MapperConfiguration(cfg => 
                { 
                    cfg.CreateMap<AppUser, MemberDto>()
                        .ForMember(dest => dest.PhotoUrl, opt => opt.MapFrom(src =>
                            src.Photos.FirstOrDefault(x => x.IsMain).Url))
                        .ForMember(dest => dest.Age, opt => opt.MapFrom(src =>
                            src.DateOfBirth.CalculateAge()));
                })
            );

            var userRepo = new UserRepository(mockContext.Object, mockMapper.Object);

            var result = await userRepo.GetMembersAsync(new UserParams());

            Assert.True(result.Select(m => m.Username == "Kevin").Any());
        }
    }
}
