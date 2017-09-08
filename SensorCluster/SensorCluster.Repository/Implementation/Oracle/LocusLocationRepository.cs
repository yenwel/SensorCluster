using System;
using SensorCluster.Model;
using SensorCluster.Repository.Contract;
using System.Data;
using System.Linq;
using System.Collections.Generic;
using SensorCluster.Repository.DAO;

namespace SensorCluster.Repository.Implementation.Oracle
{
    public class LocusLocationRepository : ILocationRepository
    {
        private PetaPoco.Database db = new PetaPoco.Database("LocusOracle");

        public const string LOC_LOCID = "LOC.LOCID";

        public void Delete(Location entity)
        {
            throw new NotImplementedException();
        }

        public Location Get(Guid id)
        {
            var result = db.SingleOrDefault<aalocguid>("SELECT id, loclocid FROM aalocguid WHERE id = @0", id.ToString("N"));
            if (result != null)
            {
                return aalocguidtoLocation(result);
            }
            else
            {
                return null;
            }
        }

        public static Location aalocguidtoLocation(aalocguid aalocguid)
        {
            var location =
                      new Location
                      {
                          Id = Guid.Parse(aalocguid.id),
                          ExternalId = new Dictionary<string, string>() { { LOC_LOCID, aalocguid.loclocid.ToString() } }
                      };
            return location;
        }

        public void Save(Location entity)
        {
            if (entity.Id != null)
            {                
                var location = Get(entity.Id);
                if (location == null)
                {
                    if (entity.ExternalId != null && entity.ExternalId.ContainsKey(LOC_LOCID))
                    {
                        int LOCID;
                        int.TryParse(entity.ExternalId[LOC_LOCID], out LOCID);
                        db.Insert(new aalocguid { id = entity.Id.ToString("N"), loclocid = LOCID });
                    }
                }
            }
        }

        public IEnumerable<Location> GetAll()
        {
            
            var result = db.Fetch<aalocguid>("SELECT id, loclocid FROM aalocguid");
            if (result != null)
            {
                return result.Select( x => aalocguidtoLocation(x));
            }
            else
            {
                return null;
            }
        }

        public void Truncate()
        {            
            db.Execute("ALTER TABLE aasensor DISABLE CONSTRAINT aasensor_locid");
            db.Execute("TRUNCATE TABLE aalocguid");
            db.Execute("ALTER TABLE aasensor ENABLE CONSTRAINT aasensor_locid");
        }
    }
}
