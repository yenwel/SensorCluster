using System;

namespace SensorCluster.SensorSystem
{
    public struct SensorEvent
    {
        public double Value { get; set; }
        public DateTime Time { get; set; }
        public Guid Sensor { get; set; }
        public string host { get; set; }
    }
}
