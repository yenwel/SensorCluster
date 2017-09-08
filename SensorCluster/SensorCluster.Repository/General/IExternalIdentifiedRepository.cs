using SensorCluster.Model;

namespace SensorCluster.Repository.General
{
    interface IExternalIdentified<TEntity,Tkey> : IRepository<TEntity,Tkey>
        where TEntity : class, IExternalInterface
    {

    }
}
