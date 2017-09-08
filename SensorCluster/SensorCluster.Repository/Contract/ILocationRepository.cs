using SensorCluster.Model;
using SensorCluster.Repository.General;
using System;

namespace SensorCluster.Repository.Contract
{
    interface ILocationRepository : IRepository<Location, Guid> { }
}
