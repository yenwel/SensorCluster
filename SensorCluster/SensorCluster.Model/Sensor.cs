using System;
using System.Collections.Generic;

namespace SensorCluster.Model
{
    public class Sensor : IExternalInterface
    {
        public Guid Id { get; set; }
        public Uri Uri { get; set; }
        public SensorType Type { get; set; }
        public Location Location { get; set; }
        public Dictionary<string, string> ExternalId { get; set; }
        public SensorValue LastValue { get; set; }
        public bool DummyTest { get; set; }
    }
}
