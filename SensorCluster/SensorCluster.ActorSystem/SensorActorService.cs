using System;
using System.Collections.Generic;
using System.Linq;
using Akka.Actor;
using SensorCluster.Repository.Implementation.Config;
using SensorCluster.Model;
using SensorCluster.Repository.Implementation.Oracle;
using SensorCluster.SensorSystem.Actors;
using SensorCluster.SensorSystem.Actors.Tests;

namespace SensorCluster.SensorSystem
{
    public class SensorActorService
    {
        private ActorSystem _mySystem;
        private Dictionary<IActorRef, Sensor> _sensorActors;
        private Dictionary<IActorRef, Sensor> _dummyTcpServerActors;
        private IActorRef _databaseActor = ActorRefs.Nobody;

        public void Start()
        {
            //this is where you setup your actor system and other things
            _mySystem = ActorSystem.Create("SensorSystem");
            _databaseActor = _mySystem.ActorOf<DatabaseActor>();
            _sensorActors = SynchronizeSensors().ToDictionary(x => _mySystem.ActorOf(Props.Create(() => new SoehnleScaleTcpClient(x))));
        }

        public void Stop()
        {
            //this is where you stop your actor system
            _mySystem.Terminate();
        }

        private IEnumerable<Sensor> SynchronizeSensors()
        {
            // get all sensors from app config
            var localSensors = new LocalSensorRepository().GetAll();
            // initialize all sensors and locations
            var localSensorsInit =
                localSensors.
                Select(x =>
                {
                    if (x.Id == Guid.Empty) x.Id = Guid.NewGuid();
                    return x;
                }).ToList();
            _dummyTcpServerActors = localSensorsInit.Where( s => s.DummyTest).ToDictionary( x => _mySystem.ActorOf(Props.Create(() => new DummyTcpServer(x.Uri.Port))));
            var localLocationInit =
                localSensorsInit.
                Select(x => x.Location).
                Distinct().
                Select(x =>
                {
                    if (x.Id == Guid.Empty) x.Id = Guid.NewGuid();
                    return x;
                }).ToList();
            // get instance of repo's
            var locusSensorRepository = new LocusSensorRepository();
            var locusLocationRepository = new LocusLocationRepository();
            // start of clean
            locusSensorRepository.Truncate();
            locusLocationRepository.Truncate();
            // save the entities on the locus DB
            foreach (var loc in localLocationInit)
            {
                locusLocationRepository.Save(loc);
            }
            foreach (var sensor in localSensorsInit)
            {
                if (sensor.Location != null && sensor.Location.ExternalId.Keys.Contains(LocusLocationRepository.LOC_LOCID))
                {
                    var loc = localLocationInit.Where(x => x.ExternalId[LocusLocationRepository.LOC_LOCID] == sensor.Location.ExternalId[LocusLocationRepository.LOC_LOCID]).FirstOrDefault();
                    sensor.Location = loc;
                }
                locusSensorRepository.Save(sensor);
            }
            // requery to get all data
            var mySensors = locusSensorRepository.GetAll();
            return mySensors;
        }
    }
}
