using System;

namespace SensorCluster.Repository.General
{
    public interface IUnitOfWork : IDisposable
    {
        void SaveChanges();
    }
}
