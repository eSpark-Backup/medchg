using MEDCHGClassLibrary.DbContext;
using MEDCHGClassLibrary.Utilities.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Linq.Expressions;


namespace MEDCHGClassLibrary.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {

        private DbSet<T> table = null;

        public MedChgContext _dbContext { get; set; }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await table.ToListAsync();
        }


        /* IQueryable<T> query = table;

         return await table.FindAsync(id);*/


        public async Task<T> AddAsync(T entity)
        {
            try
            {

                var res = await table.AddAsync(entity);
                var t = await _dbContext.SaveChangesAsync();
                return res.Entity;

            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task UpdateAsync(T entity)
        {
            try
            {
                _dbContext.Entry(entity).State = EntityState.Modified;

                var navigationProperties = _dbContext.Entry(entity).Navigations;

                List<object> newEntities = new List<object>();

                // Iterate over the navigation properties
                foreach (var navigationProperty in navigationProperties)
                {
                    if (navigationProperty != null)
                    {
                        // Check if the navigation property is a collection of child entities
                        if (navigationProperty is CollectionEntry collectionEntry)
                        {
                            var currentValues = collectionEntry?.CurrentValue?.Cast<object>().ToList();
                            if (currentValues != null)
                            {
                                for (int i = 0; i < currentValues.Count; i++)
                                {
                                    var currentValue = currentValues[i];
                                    //_dbContext.Attach(currentValue);
                                    var id = _dbContext.Entry(currentValue).Property("Id").CurrentValue;
                                    //var existingEntity = await _dbContext.FindAsync(currentValue.GetType(), _dbContext.Entry(currentValue).Property("Id").CurrentValue);

                                    if (Convert.ToInt32(id) > 0)
                                    {
                                        _dbContext.Entry(currentValue).CurrentValues.SetValues(currentValue);
                                        _dbContext.Entry(currentValue).Property("UpdatedAt").CurrentValue = TimeHelper.GetTexasTime();
                                    }

                                    else
                                    {
                                        newEntities.Add(currentValue);
                                    }

                                }
                            }
                        }
                    }
                }

                foreach (var newEntity in newEntities)
                {
                    _dbContext.Add(newEntity);
                }

                try
                {
                    await _dbContext.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    // Handle the concurrency exception
                    var entry = ex.Entries.Single();
                    var originalValues = entry.GetDatabaseValues();
                    if (originalValues != null)
                    {
                        entry.OriginalValues.SetValues(originalValues);
                        await _dbContext.SaveChangesAsync();
                    }
                    else
                    {
                        throw; // The entity was deleted since it was fetched, handle as needed
                    }
                }
            }
            catch (Exception ex)
            {
                ex.Message.ToString();
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var entity = await table.FindAsync(id);
                if (entity != null)
                {
                    table.Remove(entity);
                    await _dbContext.SaveChangesAsync();
                }
                return true;
            }
            catch (Exception)
            {
                return false;
                throw;
            }
        }

        public async Task<T> GetInfoByWhere(Expression<Func<T, bool>> where)
        {
            return await table.Where(where).FirstOrDefaultAsync();
        }


        public async Task<T> GetByIdAsync(object parentId, Expression<Func<T, object>> navigationProperty)
        {
            try
            {
                return await table.FindAsync(parentId);
            }
            catch (Exception)
            {
                throw;
            }
        }

        async Task<IEnumerable<T>> IRepository<T>.GetAsync(Expression<Func<T, bool>> Singlefilter, IEnumerable<Expression<Func<T, bool>>> filters, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy, string includeProperties)
        {
            IQueryable<T> query = table;


            if (Singlefilter != null)
            {

                query = query.Where(Singlefilter);

            }
            if (filters != null)
            {
                foreach (var filter in filters)
                {
                    query = query.Where(filter);
                }
            }

            if (!string.IsNullOrEmpty(includeProperties))
            {
                foreach (var includeProperty in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProperty);
                }
            }
            return query.ToList();
        }
        protected virtual void FormatDateTimeProperties(IEnumerable<T> entities)
        {
            foreach (var entity in entities)
            {
                foreach (var property in entity.GetType().GetProperties().Where(p => p.PropertyType == typeof(DateTime)))
                {
                    var value = (DateTime)property.GetValue(entity);
                    property.SetValue(entity, value.ToUniversalTime());
                }
            }
        }

        async Task<IEnumerable<T>> IRepository<T>.GetBySql(string sql)
        {
            return await _dbContext.Set<T>()
                         .FromSqlRaw(sql)
                         .ToListAsync();
        }

        public async Task UpdateHospitalByIdAsync(int id, bool fav)
        {
            try
            {
                var Hospital = _dbContext.Hospitals.First(x => x.Id == id);
                Hospital.UpdatedAt = TimeHelper.GetTexasTime();
                Hospital.UpdatedById = 1;
                Hospital.IsFavourite = fav;
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception)
            {
                // Handle the exception or log it
            }
        }
    }
}
