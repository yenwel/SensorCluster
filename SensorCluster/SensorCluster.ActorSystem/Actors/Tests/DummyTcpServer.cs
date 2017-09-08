using Akka.Actor;
using Akka.Event;
using Akka.IO;
using System;
using System.Net;

namespace SensorCluster.SensorSystem.Actors.Tests
{
   public class DummyTcpServer : UntypedActor
    {

        private readonly ILoggingAdapter _log = Logging.GetLogger(Context);

        public DummyTcpServer(int port)
        {
            Context.System.Tcp().Tell(new Tcp.Bind(Self, new IPEndPoint(IPAddress.Any, port)));
        }

        protected override void OnReceive(object message)
        {
            if (message is Tcp.Bound)
            {
                var bound = message as Tcp.Bound;
                _log.Debug("Listening on {0}", bound.LocalAddress);
            }
            else if (message is Tcp.Connected)
            {
                var connection = Context.ActorOf(Props.Create(() => new DummyTcpConnection(Sender)));
                Sender.Tell(new Tcp.Register(connection));
            }
            else Unhandled(message);
        }
    }

}
