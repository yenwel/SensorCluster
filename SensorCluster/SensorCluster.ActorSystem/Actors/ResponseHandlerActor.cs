using Akka.Actor;
using Akka.Event;
using Akka.IO;
using SensorCluster.General;
using SensorCluster.Model;
using SensorCluster.SensorSystem.Events;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace SensorCluster.SensorSystem.Actors
{
    public class ResponseHandlerActor : ReceiveActor
    {
        private Sensor _sensor { get; set; }
        private readonly ILoggingAdapter _log = Logging.GetLogger(Context);

        public ResponseHandlerActor(Sensor sensor)
        {
            _sensor = sensor;
            Receive<ByteString>(b =>
            {
                if (b.Count == 1 && b[0] == 21)
                {
                    //received NAK
                    Sender.Tell(new NotAcknowledgedEvent());
                }
                else
                {
                    try
                    {
                        foreach (var ev in parseResponse(b as ByteString))
                        {
                            Context.System.EventStream.Publish(ev);
                        }
                    }
                    catch (System.FormatException fe)
                    {
                        Sender.Tell(fe);
                        throw fe;
                    }
                }
            });
        }

        public IEnumerable<SensorEvent> parseResponse(ByteString data)
        {
            var time = DateTime.Now;
            foreach (var chunk in (data.ToList()).Chunk(22))
            {
                //skip to the N character and then take 12 next bytes
                var valueBytes = chunk.SkipWhile(x => (Encoding.ASCII.GetChars(new[] { x }).First()) != 'N').Skip(1).Take(12).ToArray();
                var value = double.Parse(Encoding.ASCII.GetString(valueBytes), NumberStyles.Any, CultureInfo.GetCultureInfo("de-DE").NumberFormat);
                yield return new SensorEvent { Value = value, Time = time, Sensor = _sensor.Id, host = _sensor.Uri.ToString() };
            }
        }
    }
}
