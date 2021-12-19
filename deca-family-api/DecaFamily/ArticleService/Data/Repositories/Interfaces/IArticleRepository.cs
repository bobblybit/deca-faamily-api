using ArticleService.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ArticleService.Data.Repositories.Interfaces
{
    public interface IArticleRepository
    {
        Task<bool> AddArticleAsync(Article article);
        Task<Article> GetArticleByIdAsync(string articleId);
        Task<IEnumerable<Article>> GetArticlesByCategoryAsync(string categoryId, int perPage, int page);
        Task<IEnumerable<Article>> GetRecentArticlesAsync(int perPage, int page);
        Task<bool> UpdateArticleAsync(Article UpdatedArticle);
        Task<bool> DeletedArticleAsync(Article articleToDelete);
        public int GetCount();
        Task<bool> AddArticleLikeAsync(ArticleLike articleLike);
        Task<ArticleLike> GetArticleLikeByArticleIdAndLikerAsync(string articleId, string likerId);
        Task<bool> DeleteArticleLikeAsync(ArticleLike articleLike);
        Task<bool> AddCommentLikeAsync(CommentLike commentLike);
        Task<CommentLike> GetCommentLikeByCommentIdAndLikerAsync(string commentId, string likerId);
        Task<bool> DeleteCommentLikeAsync(CommentLike commentLike);
        Task<IEnumerable<Article>> GetUnapprovedArticles(int perPage, int page);
        Task<IEnumerable<Article>> SearchArticles(string query, int perPage, int page);
    }
}
