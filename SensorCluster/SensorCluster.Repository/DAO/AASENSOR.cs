using System;

namespace SensorCluster.Repository.DAO.database
{
    [PetaPoco.TableName("aasensor")]
    [PetaPoco.PrimaryKey("id", AutoIncrement = false)]
    public class aasensor
    {
        public string id {get; set;}
        public string uri {get; set;}
        public string type {get; set;}
        public double lastval {get; set;}
        public DateTime time {get; set;}
        public string locguid { get; set; }
    }
}
