using SensorCluster.Repository.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SensorCluster.Model;

namespace SensorCluster.Repository.Implementation.Config
{
    class LocalLocationRepository : ILocationRepository
    {
        public void Delete(Location entity)
        {
            throw new NotImplementedException();
        }

        public Location Get(Guid id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Location> GetAll()
        {
            throw new NotImplementedException();
        }

        public void Save(Location entity)
        {
            throw new NotImplementedException();
        }

        public void Truncate()
        {
            throw new NotImplementedException();
        }
    }
}
