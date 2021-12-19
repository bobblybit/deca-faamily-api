using ArticleService.Data.Database;
using ArticleService.Data.Models;
using ArticleService.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using static ArticleService.Extensions.LinqExtensions;

namespace ArticleService.Data.Repositories.Implementations
{
    public class ArticleRepository : IArticleRepository
    {
        private readonly DataContext _ctx;
        public int totalCount { get; set; }

        public ArticleRepository(DataContext ctx)
        {
            _ctx = ctx;
        }

        public async Task<bool> AddArticleLikeAsync(ArticleLike articleLike)
        {
            await _ctx.ArticleLikes.AddAsync(articleLike);
            return await _ctx.SaveChangesAsync() > 0;
        }

        private async Task<bool> SavedAsync()
        {
            var valueToReturned = false;
            if (await _ctx.SaveChangesAsync() > 0)
                valueToReturned = true;
            else
                valueToReturned = false;

            return valueToReturned;
        }

        public int GetCount()
        {
            return totalCount;
        }

        private async Task<IEnumerable<Article>> Paginate(IQueryable<Article> articles, int perPage, int page)
        {
            var result = await articles.Skip((page - 1) * perPage).Take(perPage).ToListAsync();
            return result;
        }

        public async Task<bool> AddArticleAsync(Article article)
        {
            await _ctx.Articles.AddAsync(article);
            return await SavedAsync();
        }

        public async Task<Article> GetArticleByIdAsync(string articleId)
        {
            return await _ctx.Articles.SingleOrDefaultAsync(x => x.Id == articleId && x.Approved == true);
        }

        public async Task<IEnumerable<Article>> GetRecentArticlesAsync(int perPage, int page)
        {
            var articlesFromDB = GetApprovedArticles().OrderByDescending(x => x.CreatedAt);
            totalCount = articlesFromDB.Count();
            var pagnatedResult = await Paginate(articlesFromDB, perPage, page);
            return pagnatedResult;
        }

        public async Task<IEnumerable<Article>> GetUnapprovedArticles(int perPage, int page) {
            var articlesFromDB = GetUnapprovedAricles().OrderByDescending(x => x.CreatedAt);
            totalCount = articlesFromDB.Count();
            var pagnatedResult = await Paginate(articlesFromDB, perPage, page);
            return pagnatedResult;
        }

        public async Task<IEnumerable<Article>> SearchArticles(string query, int perPage, int page)
        {
            var articlesFromDB = GetApprovedArticles()
                .Where(x => x.Title.ToLower().Contains(query.ToLower()) 
                || x.Tag.ToLower().Contains(query.ToLower()) 
                || x.Content.ToLower().Contains(query.ToLower()))
                .OrderByDescending(x => x.CreatedAt);
            totalCount = articlesFromDB.Count();
            var pagnatedResult = await Paginate(articlesFromDB, perPage, page);
            return pagnatedResult;
        }

        public Task<ArticleLike> GetArticleLikeByArticleIdAndLikerAsync(string articleId, string likerId)
        {
            return _ctx.ArticleLikes.SingleOrDefaultAsync(x => x.ArticleId == articleId && x.Liker == likerId);
        }

        public async Task<IEnumerable<Article>> GetArticlesByCategoryAsync(string categoryId, int perPage, int page)
        {
            var articlesFromDB = GetApprovedArticles().Where(x => x.CategoryId == categoryId);
            totalCount = articlesFromDB.Count();
            var pagnatedResult = await Paginate(articlesFromDB, perPage, page);
            return pagnatedResult;
        }

        public async Task<bool> UpdateArticleAsync(Article UpdatedArticle)
        {
            _ctx.Articles.Update(UpdatedArticle);
            return await SavedAsync();
        }

        public async Task<bool> DeleteArticleLikeAsync(ArticleLike articleLike)
        {
            _ctx.ArticleLikes.Remove(articleLike);
            return await _ctx.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeletedArticleAsync(Article articleToDelete)
        {
            _ctx.Articles.Remove(articleToDelete);
            return await SavedAsync();
        }

        public async Task<bool> AddCommentLikeAsync(CommentLike commentLike)
        {
            await _ctx.CommentLikes.AddAsync(commentLike);
            return await _ctx.SaveChangesAsync() > 0;
        }

        public Task<CommentLike> GetCommentLikeByCommentIdAndLikerAsync(string commentId, string likerId)
        {
            return _ctx.CommentLikes.SingleOrDefaultAsync(x => x.CommentId == commentId && x.Liker == likerId);
        }

        public async Task<bool> DeleteCommentLikeAsync(CommentLike commentLike)
        {
            _ctx.CommentLikes.Remove(commentLike);
            return await _ctx.SaveChangesAsync() > 0;
        }

        private IQueryable<Article> GetApprovedArticles()
        {
            return _ctx.Articles.Include(x => x.Comments)
                .Include(x => x.User)
                .Include(x => x.ArticleLikes)
                .Where(x => x.Approved == true);
        }

        private IQueryable<Article> GetUnapprovedAricles()
        {
            return _ctx.Articles.Include(x => x.Comments)
                .Include(x => x.User)
                .Include(x => x.ArticleLikes)
                .Where(x => x.Approved == false);
        }
    }
}
