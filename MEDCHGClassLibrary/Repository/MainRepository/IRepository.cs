using MEDCHGClassLibrary.DbContext;
using System.Linq.Expressions;

namespace MEDCHGClassLibrary.Repository
{
    public interface IRepository<T> where T : class
    {
        public MedChgContext _dbContext { get; set; }
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> GetByIdAsync(object parentId, Expression<Func<T, object>> navigationProperty);
        Task<T> GetInfoByWhere(Expression<Func<T, bool>> where);
        Task<T> AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<T>> GetAsync(Expression<Func<T, bool>> Singlefilter, IEnumerable<Expression<Func<T, bool>>> filters = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
                                                                                                                string includeProperties = "");
        Task UpdateHospitalByIdAsync(int id, bool fav);
        Task<IEnumerable<T>> GetBySql(string sql);
    }
}
