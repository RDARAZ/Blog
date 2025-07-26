using Blog.Domain.Enums;

namespace Blog.Domain.Entities;

public class Article : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public int AuthorId { get; set; }
    public ArticleStatus Status { get; set; } = ArticleStatus.Draft;
    public DateTime? ScheduledPublishDate { get; set; }
    public int ViewCount { get; set; } = 0;
    public virtual User Author { get; set; } = null!;

    public bool IsPublished() => Status == ArticleStatus.Published;
    public bool CanBePublished() => !string.IsNullOrEmpty(Title) && !string.IsNullOrEmpty(Content);
    public void Publish()
    {
        if(CanBePublished())
        {
            Status = ArticleStatus.Published;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
