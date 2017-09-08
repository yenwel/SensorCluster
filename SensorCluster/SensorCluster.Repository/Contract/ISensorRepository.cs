using SensorCluster.Model;
using SensorCluster.Repository.General;
using System;

namespace SensorCluster.Repository.Contract
{
    interface ISensorRepository : IRepository<Sensor,Guid>
    {
        void SetValue(Guid id, SensorValue value);

    }
}
