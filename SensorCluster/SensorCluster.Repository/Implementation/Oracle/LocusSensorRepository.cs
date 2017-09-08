using System;
using System.Collections.Generic;
using SensorCluster.Model;
using SensorCluster.Repository.Contract;
using System.Data;
using System.Linq;
using SensorCluster.Repository.DAO.database;

namespace SensorCluster.Repository.Implementation.Oracle
{
    public class LocusSensorRepository : ISensorRepository
    {
        private PetaPoco.Database db = new PetaPoco.Database("LocusOracle");

        public void Delete(Sensor entity)
        {
            throw new NotImplementedException();
        }

        public Sensor Get(Guid id)
        {
            var sensorResult = db.SingleOrDefault<aasensor>("SELECT id, uri, type, lastval, time, locguid FROM aasensor WHERE id = @0",id.ToString("N"));
            if (sensorResult != null)
            {
                return aasensortoSensor(sensorResult);
            }
            else
            {
                return null;
            }
            
        }

        public Sensor aasensortoSensor(aasensor sensor)
        {
            var locationrepo = new LocusLocationRepository();
            return new Sensor
            {
                Id = Guid.Parse(sensor.id),
                Uri = new Uri(sensor.uri),
                Type = (SensorType)Enum.Parse(typeof(SensorType), sensor.type),
                Location = locationrepo.Get(Guid.Parse(sensor.locguid)),
                LastValue = new SensorValue { Value =sensor.lastval, Time = sensor.time }
            };
        }

        public IEnumerable<Sensor> GetAll()
        {            
            var result = db.Fetch<aasensor>("SELECT id, uri, type, lastval, time, locguid FROM aasensor");
            if (result != null)
            {
                return result.Select(x => aasensortoSensor(x)).ToList();
            }
            else
            {
                return null;
            }
        }

        public void Save(Sensor entity)
        {           
            db.Insert(new aasensor { id = entity.Id.ToString("N"), uri = entity.Uri.ToString(), type = entity.Type.ToString(), locguid = entity.Location.Id.ToString("N") });
        }

        public void SetValue(Guid id, SensorValue value)
        {
            db.Execute("UPDATE aasensor SET lastval = @0, time = @1 WHERE id = @2", value.Value, value.Time, id.ToString("N"));
        }

        public void Truncate()
        {
           var result = db.Execute("TRUNCATE TABLE aasensor");
        }
    }
}
