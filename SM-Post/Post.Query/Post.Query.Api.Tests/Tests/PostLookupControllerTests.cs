using FluentAssertions;
using FluentAssertions.Extensions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Post.Query.Api.Database;
using Post.Query.Api.Database.Entities;
using System.Net.Http.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Post.Query.Api.Tests.Tests
{
    [Collection("PostgresTestCollection")]
    public class PostLookupControllerTests
    {
        const string _controllerUrl = "/api/v1/lookup-posts";

        private readonly HttpClient _client;
        private readonly PostQueryWebApiFactory _appFactory;


        public PostLookupControllerTests(PostgresDatabaseFixture fixture)
        {
            _appFactory = fixture.AppFactory;
            _client = _appFactory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });
        }

        [Fact]
        public async Task GetPostById_ReturnsOk()
        {
            using (var scope = _appFactory.Services.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var dbContext = scopedServices.GetRequiredService<DatabaseContext>();
                var expectedPost = new PostEntity
                {
                    Id = Guid.NewGuid(),
                    Author = "Pepe Martinez",
                    CreatedDate = DateTime.UtcNow,
                    Likes = 1,
                    Message = "This is the post text.",
                    Comments = new HashSet<CommentEntity>()
                };

                dbContext.RemoveRange(dbContext.Posts);
                dbContext.Posts.Add(expectedPost);
                await dbContext.SaveChangesAsync();


                var response = await _client.GetAsync($"{_controllerUrl}/{expectedPost.Id}");
                response.IsSuccessStatusCode.Should().BeTrue();
                var content = await response.Content.ReadFromJsonAsync<List<PostEntity>>();
                content.Should().NotBeNull();
                content.Should().HaveCount(1);
                var actualPost = content[0];
                actualPost.Should().BeEquivalentTo(expectedPost, options => options
                    .Using<DateTime>(ac => ac.Subject.Should().BeCloseTo(ac.Expectation, 1.Seconds()))
                    .WhenTypeIs<DateTime>());
            }
        }

        [Fact]
        public async Task GetByAuthor_Works()
        {
            using (var scope = _appFactory.Services.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var dbContext = scopedServices.GetRequiredService<DatabaseContext>();
                var commonAuthor = "Pepe Martinez";

                List<PostEntity> posts = [new PostEntity
                {
                    Id = Guid.NewGuid(),
                    Author = commonAuthor,
                    CreatedDate = DateTime.UtcNow,
                    Likes = 1,
                    Message = "This is the post text 1.",
                    Comments = new HashSet<CommentEntity>()
                },new PostEntity
                {
                    Id = Guid.NewGuid(),
                    Author = commonAuthor,
                    CreatedDate = DateTime.UtcNow,
                    Likes = 1,
                    Message = "This is the post text 2.",
                    Comments = new HashSet<CommentEntity>()
                },new PostEntity
                {
                    Id = Guid.NewGuid(),
                    Author = "Jon Jones",
                    CreatedDate = DateTime.UtcNow,
                    Likes = 1,
                    Message = "This is the post text 3.",
                    Comments = new HashSet<CommentEntity>()
                }];

                dbContext.RemoveRange(dbContext.Posts);
                dbContext.Posts.AddRange(posts);
                await dbContext.SaveChangesAsync();

                var response = await _client.GetAsync($"{_controllerUrl}/by-author?author={commonAuthor}");
                response.IsSuccessStatusCode.Should().BeTrue();
                var actualPosts = await response.Content.ReadFromJsonAsync<List<PostEntity>>();
                actualPosts.Should().NotBeNull();
                actualPosts.Should().HaveCount(2);
                var expectedPosts = posts.Take(2);
                actualPosts.Should().BeEquivalentTo(expectedPosts, options => options
                    .Using<DateTime>(ac => ac.Subject.Should().BeCloseTo(ac.Expectation, 1.Seconds()))
                    .WhenTypeIs<DateTime>());
            }
        }
    }
}
