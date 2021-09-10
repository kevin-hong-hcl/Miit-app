using API.Data;
using API.DTO;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using EntityFrameworkCoreMock;
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

        public DbContextOptions<TestDbContext> DummyOptions { get; } = new DbContextOptionsBuilder<TestDbContext>().Options;

        //[Fact]
        public async void Test_GetPhotoById()
        {
            var user1 = new AppUser { Id=1, UserName="Hudson" };
            var user2 = new AppUser { Id=2, UserName="Kevin" };
            var user3 = new AppUser { Id=3, UserName="Matthew" };
            var data = new[] { user1,user2,user3 };

            var photoData = new List<Photo>
            {
                new Photo { Id = 1, Url="test1", IsApproved=true, AppUser=user1, AppUserId=1, IsMain=true },
                new Photo { Id = 2, Url="test2", IsApproved=true, AppUser=user2, AppUserId=2, IsMain=true },
                new Photo { Id = 3, Url="test3", IsApproved=true, AppUser=user3, AppUserId=3, IsMain=true }
            }.AsQueryable();

            var dbContextMock = new DbContextMock<TestDbContext>(DummyOptions);
            //var usersDbSetMock = dbContextMock.CreateDbSetMock(x => x.Users, data);
            var photoDbSetMock = dbContextMock.CreateDbSetMock(x => x.Photos, photoData);
            dbContextMock.Setup(c => c.Photos).Returns(photoDbSetMock.Object);
            //photoDbSetMock.Setup(c => c.IgnoreQueryFilters()).Returns(photoData.AsQueryable());


            var photoRepo = new PhotoRepository(dbContextMock.Object);
            var photoResult1 = await photoRepo.GetPhotoById(1);
            var photoResult = await dbContextMock.Object.Photos.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == 1);

            Assert.True(photoResult1.Url == "test1");
        }

        //[Fact]
        public async void Test_GetAllMembers()
        {
            var user1 = new AppUser { Id = 1, UserName = "Hudson" };
            var user2 = new AppUser { Id = 2, UserName = "Kevin" };
            var user3 = new AppUser { Id = 3, UserName = "Matthew" };
            var data = new[] { user1, user2, user3 }.AsQueryable();

            var mockUserSet = new Mock<DbSet<AppUser>>();
            mockUserSet.As<IQueryable<AppUser>>().Setup(m => m.Provider).Returns(data.Provider);
            mockUserSet.As<IQueryable<AppUser>>().Setup(m => m.Expression).Returns(data.Expression);
            mockUserSet.As<IQueryable<AppUser>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockUserSet.As<IQueryable<AppUser>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

            var mockOptions = new Mock<DbContextOptions>();
            var mockContext = new Mock<DataContext>(mockOptions.Object);
            mockContext.Setup(c => c.Users).Returns(mockUserSet.Object);

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
