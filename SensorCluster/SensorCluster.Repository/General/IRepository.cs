using System.Collections.Generic;

namespace SensorCluster.Repository.General
{
    public interface IRepository<TEntity, in TKey> where TEntity : class
    {
        TEntity Get(TKey id);
        IEnumerable<TEntity> GetAll();
        void Save(TEntity entity);
        void Delete(TEntity entity);
        void Truncate();
    }
}
