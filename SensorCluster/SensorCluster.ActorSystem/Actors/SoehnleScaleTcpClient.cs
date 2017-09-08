using Akka.Actor;
using Akka.Event;
using Akka.IO;
using SensorCluster.Model;
using SensorCluster.SensorSystem.Commands;
using SensorCluster.SensorSystem.Events;
using System;
using System.Net;
using System.Text;

namespace SensorCluster.SensorSystem.Actors
{
    public class SoehnleScaleTcpClient : UntypedActor
    {
        private static ByteString weighingCommandA = ByteString.FromString("<A>\n");
        private Sensor _sensor { get; set; }
        private IPEndPoint _endpoint { get; set; }
        private readonly ILoggingAdapter _log = Logging.GetLogger(Context);

        public SoehnleScaleTcpClient(Sensor sensor)
        {
            _sensor = sensor;
            IPAddress ip;
            if (!IPAddress.TryParse(sensor.Uri.Host, out ip))
            {
                throw new FormatException("Invalid ip-adress");
            }
            _endpoint = new IPEndPoint(ip, sensor.Uri.Port);
            Self.Tell(new DoConnectCommand());   
        }

        public SoehnleScaleTcpClient() : base()
        {
        }

        protected override void OnReceive(object message)
        {
            if (message is DoConnectCommand)
            {
                var cancelReconnect = new Cancelable(Context.System.Scheduler, TimeSpan.FromDays(1));
                Context.System.Scheduler.ScheduleTellRepeatedly(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(5), Context.System.Tcp(), new Tcp.Connect(_endpoint), Self, cancelReconnect);
                Become(Connecting(cancelReconnect));
            }
            else Unhandled(message);
        }         

        private UntypedReceive Connecting(ICancelable cancelReconnect)
        {
            return message =>
            {
                if (message is Tcp.Connected)
                {
                    cancelReconnect.CancelIfNotNull();
                    var connected = message as Tcp.Connected;
                    _log.Debug("Connected to {0}", connected.RemoteAddress);
                    // Register self as connection handler
                    Sender.Tell(new Tcp.Register(Self));
                    var cancelCommand = new Cancelable(Context.System.Scheduler);
                    var parsingActor = Context.ActorOf(Props.Create(() => new ResponseHandlerActor(_sensor)));
                    Become(Connected(Sender, parsingActor, cancelCommand));
                    // schedule itself every 0.01 second
                    Context.System.Scheduler.ScheduleTellRepeatedly(TimeSpan.FromSeconds(0.05), TimeSpan.FromSeconds(0.01), Self, new DoWeighingCommand(), Self, cancelCommand);
                }
                else if (message is Tcp.CommandFailed)
                {
                    var received = message as Tcp.CommandFailed;
                    _log.Debug("Connection failed");
                    Context.System.Tcp().Tell(new Tcp.CommandFailed(received.Cmd));
                    Become(OnReceive);
                    Self.Tell(new DoConnectCommand());
                }
                else if (message is Tcp.Received)
                {
                    var received = message as Tcp.Received;
                    if (received.Data.Count == 1 && received.Data[0] == 21)
                    {
                        //received NAK
                        _log.Debug("NAK received");
                        Context.System.Tcp().Tell(new Tcp.NoAck(_sensor.Uri.Host));
                        Become(OnReceive);
                        Self.Tell(new DoConnectCommand());
                    }
                }
                else if (message is DoWeighingCommand) { }
                else Unhandled(message);
            };
        }

        private UntypedReceive Connected(IActorRef connection, IActorRef parsingActor, ICancelable cancelCommand)
        {
            return message =>
            {
                if (message is DoWeighingCommand) //received command to do weighing
                {
                    connection.Tell(Tcp.Write.Create(weighingCommandA));
                }
                else if (message is Tcp.Received)  // data received from network
                {
                    var received = message as Tcp.Received;
                    _log.Debug(Encoding.ASCII.GetString(received.Data.ToArray()));
                    parsingActor.Tell(received.Data);                  
                }
                else if (message is Tcp.PeerClosed) //try reconnect itself!
                {
                    _log.Debug("Connection closed");
                    cancelCommand.CancelIfNotNull();
                    Context.System.Tcp().Tell(new Tcp.ConnectionClosed());
                    Become(OnReceive);
                    Self.Tell(new DoConnectCommand());
                }
                else if (message is NotAcknowledgedEvent)
                {
                    _log.Debug("NAK received");
                    cancelCommand.CancelIfNotNull();
                    Context.System.Tcp().Tell(new Tcp.NoAck(_sensor.Uri.Host));
                    Become(OnReceive);
                    Self.Tell(new DoConnectCommand());
                }
                else Unhandled(message);
            };
        }    
    }
}
