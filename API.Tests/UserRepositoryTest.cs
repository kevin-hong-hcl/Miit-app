using API.Data;
using API.DTO;
using API.Entities;
using API.Extensions;
using API.Helpers;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace API.Tests
{
    public class UserRepositoryTest : BaseTest
    {
        public UserRepositoryTest() : base(
            new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options
            )
        {
        }

        [Fact]
        public async void Can_Get_All_Users()
        {
            using (var context = new DataContext(ContextOptions))
            {
                var mockMapper = new Mock<IMapper>();
                mockMapper.Setup(m => m.ConfigurationProvider).Returns(() =>
                    new MapperConfiguration(cfg =>
                    {
                        cfg.CreateMap<AppUser, MemberDto>()
                            .ForMember(dest => dest.PhotoUrl, opt => opt.MapFrom(src =>
                                src.Photos.FirstOrDefault(x => x.IsMain).Url))
                            .ForMember(dest => dest.Age, opt => opt.MapFrom(src =>
                                src.DateOfBirth.CalculateAge()));
                        cfg.CreateMap<Photo, PhotoDto>();
                    })
                );
                var userRepo = new UserRepository(context, mockMapper.Object);

                var paginatedResult = await userRepo.GetMembersAsync(new UserParams());
                var usernames = paginatedResult.ToList<MemberDto>().Select(c => c.Username);

                Assert.Equal(3, paginatedResult.TotalCount);
                Assert.Contains("Matthew", usernames);
                Assert.Contains("Hudson", usernames);
                Assert.Contains("Kevin", usernames);
            }
        }
    }
}
