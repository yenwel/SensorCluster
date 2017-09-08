using System;
using System.Collections.Generic;
using SensorCluster.Model;
using SensorCluster.Repository.Contract;
using SimpleConfig;

namespace SensorCluster.Repository.Implementation.Config
{
    public class SensorSettings
    {
        public IEnumerable<Sensor> sensors { get; set; }
    }
    public class LocalSensorRepository : ISensorRepository
    {
        public void Delete(Sensor entity)
        {
            throw new NotImplementedException();
        }

        public Sensor Get(Guid id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Sensor> GetAll()
        {
            var config = Configuration.Load<SensorSettings>();
            return config.sensors;
        }

        public void Save(Sensor entity)
        {
            throw new NotImplementedException();
        }

        public void SetValue(Guid id, SensorValue value)
        {
            throw new NotImplementedException();
        }

        public void Truncate()
        {
            throw new NotImplementedException();
        }
    }
}
