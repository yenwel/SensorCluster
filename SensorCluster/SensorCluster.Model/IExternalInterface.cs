using System.Collections.Generic;

namespace SensorCluster.Model
{
    public interface IExternalInterface
    {
        Dictionary<string, string> ExternalId { get; set; }
    }
}
