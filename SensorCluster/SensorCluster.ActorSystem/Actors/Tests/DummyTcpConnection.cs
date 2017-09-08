using Akka.Actor;
using Akka.Event;
using Akka.IO;
using System;

namespace SensorCluster.SensorSystem.Actors.Tests
{
    public class DummyTcpConnection : UntypedActor
    {
        private readonly IActorRef _connection;
        private readonly Random _random;
        private readonly ILoggingAdapter _log = Logging.GetLogger(Context);

        public DummyTcpConnection(IActorRef connection)
        {
            _connection = connection;
            _random = new Random();
        }

        protected override void OnReceive(object message)
        {
            if (message is Tcp.Received)
            {
                var received = message as Tcp.Received;
                if (received.Data.Head == 'x')
                    Context.Stop(Self);
                else
                {
                    //var value = $"000101N{(_random.NextDouble() * 10).ToString("000.000", CultureInfo.GetCultureInfo("de-DE").NumberFormat).PadLeft(12)} kg ";
                    _connection.Tell(Tcp.Write.Create(ByteString.FromString("000101N      100,0 kg ")));
                }
            }
            else Unhandled(message);
        }
    }
}
