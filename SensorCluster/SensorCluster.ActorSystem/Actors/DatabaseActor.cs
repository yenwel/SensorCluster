using Akka.Actor;
using Akka.Event;
using SensorCluster.Model;
using SensorCluster.Repository.Implementation.Oracle;

namespace SensorCluster.SensorSystem.Actors
{
    public class DatabaseActor : ReceiveActor
    {
        private LocusSensorRepository sensorRepository;
        private readonly ILoggingAdapter _log = Logging.GetLogger(Context);

        public DatabaseActor()
        {
            sensorRepository = new LocusSensorRepository();
            Context.System.EventStream.Subscribe(Self, typeof(SensorEvent));
            Receive<SensorEvent>
                (
                e =>
                sensorRepository.SetValue(e.Sensor, new SensorValue { Time = e.Time, Value = e.Value })
                );
        }
    }
}
