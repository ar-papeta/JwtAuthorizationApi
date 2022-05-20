using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories;

public interface IRepository<TEntity> where TEntity : class
{
    public IEnumerable<TEntity> Get(Expression<Func<TEntity, bool>>? filter = null);
    public TEntity? GetByID(Guid? id);
    public void Insert(TEntity entity);
    public void Delete(Guid id);
    public void Delete(TEntity entityToDelete);
    public void Update(TEntity entityToUpdate);
}

