using Blog.Domain.Entities;
using Blog.Domain.Enums;
using Blog.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Blog.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TestController(BlogDbContext context) : ControllerBase
{
    [HttpPost("seed")]
    public async Task<IActionResult> SeedTestData()
    {
        try
        {
            var testUser = new User
            {
                Username = "testuser",
                Email = "test@example.com",
                PasswordHash = "hashedpassword", // In real app, use proper hashing
                Salt = "saltvalue",
                Role = UserRole.User,
                Gender = Gender.Male,
                Age = 30,
                IsActive = true
            };

            context.Users.Add(testUser);
            await context.SaveChangesAsync();

            var testArticle = new Article
            {
                Title = "Test Article",
                Content = "This is a test article content",
                Summary = "Test summary",
                Status = ArticleStatus.Published,
                AuthorId = testUser.Id
            };

            context.Articles.Add(testArticle);
            await context.SaveChangesAsync();

            return Ok(new
            {
                userId = testUser.Id,
                articleId = testArticle.Id,
                message = "Test data seeded successfully"
            });
        }
        catch (Exception e)
        {
            return BadRequest(new
            {
                message = "Error seeding test data",
                error = e.Message
            });
        }
    }

    [HttpGet("users")]
    public async Task<IActionResult> GetUsers()
    {
        var users = await context.Users
            .Select(u => new
            {
                u.Username,
                u.Email,
                u.PasswordHash,
                u.Salt,
                u.Role,
                u.Gender,
                u.Age,
                u.IsActive
            })
            .ToListAsync();
        return Ok(users);
    }

    [HttpGet("articles")]
    public async Task<IActionResult> GetArticles()
    {
        var articles = await context.Articles
            .Include(a => a.Author)
            .Select(a => new
            {
                a.Id,
                a.Title,
                a.Content,
                a.Summary,
                a.Status,
                Author = a.Author.Username,
            })
            .ToListAsync();

        return Ok(articles);
    }
}
