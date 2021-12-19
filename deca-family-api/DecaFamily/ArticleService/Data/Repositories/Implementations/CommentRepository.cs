using ArticleService.Data.Database;
using ArticleService.Data.Models;
using ArticleService.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ArticleService.Data.Repositories.Implementations
{
    public class CommentRepository : ICommentRepository
    {
        private readonly DataContext _context;

        public CommentRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<bool> AddCommentAsync(Comment comment)
        {
            await _context.Comments.AddAsync(comment);
            var rowsAffected = await _context.SaveChangesAsync();

            return rowsAffected > 0;
        }

        public async Task<bool> DeleteCommentAsync(Comment comment)
        {
            _context.Comments.Remove(comment);
            int rowsAffected = await _context.SaveChangesAsync();
            return rowsAffected > 0;
        }

        public async Task<IEnumerable<Comment>> GetCommentByArticleId(string articleId)
        {
            return await _context.Comments.Where(x => x.ArticleId == articleId).ToListAsync();
        }

        public async Task<Comment> GetCommentByIdAsync(string commentId)
        {
            return await _context.Comments.Where(x => x.Id == commentId).SingleOrDefaultAsync();
        }

        public async Task<bool> UpdateCommentAsync(Comment comment)
        {
             _context.Comments.Update(comment);
            var rowsAffected = await _context.SaveChangesAsync();

            return rowsAffected > 0;
        }

        
    }
}
