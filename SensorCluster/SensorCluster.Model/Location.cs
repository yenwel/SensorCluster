using System;
using System.Collections.Generic;

namespace SensorCluster.Model
{
    public class Location : IExternalInterface
    {
        public Guid Id { get; set; }
        public Coordinate Coordinate { get; set; }
        public Dictionary<string, string> ExternalId { get; set; }
    }
}
