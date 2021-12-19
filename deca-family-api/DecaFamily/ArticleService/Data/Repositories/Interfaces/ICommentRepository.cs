using ArticleService.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ArticleService.Data.Repositories.Interfaces
{
    public interface ICommentRepository
    {
        Task<IEnumerable<Comment>> GetCommentByArticleId(string articleId);
        Task<bool> AddCommentAsync(Comment comment);

        Task<bool> UpdateCommentAsync(Comment comment);

        Task<bool> DeleteCommentAsync(Comment comment);

        Task<Comment> GetCommentByIdAsync(string commentId);
    }
}
