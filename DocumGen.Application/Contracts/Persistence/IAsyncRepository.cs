using DocumGen.Application.Contracts.Pagination;
using System;
using System.Threading.Tasks;

namespace DocumGen.Application.Contracts.Persistence
{
    public interface IAsyncRepository<T> where T : class
    {
        Task<T> GetByIdAsync(Guid id);
        Task<PageResult<T>> ListAsync(PageQuery pageQuery);
        Task<T> AddAsync(T entity);
        Task<bool> UpdateAsync(T entity);
        Task<bool> DeleteAsync(Guid id);
    }
}
