using Akka.Actor;
using Akka.IO;
using Akka.TestKit.NUnit;
using NUnit.Framework;
using SensorCluster.Model;
using SensorCluster.SensorSystem;
using SensorCluster.SensorSystem.Actors;
using System;

namespace SensorCluster.Tests
{
    [TestFixture]
    public class ParsingTest : TestKit
    {
        private Sensor dummySensor = new Sensor() { Id = Guid.NewGuid(), Type = SensorType.SCALE, Uri = new Uri("tcp://127.0.0.1:9001") };
        [Test]
        public void ResponseHanderlActor_should_parse_correct_string_minus1000()
        {
            var handler = Sys.ActorOf(Props.Create(() => new ResponseHandlerActor(dummySensor)));
            var subscriber = CreateTestProbe();
            Sys.EventStream.Subscribe(subscriber.Ref, typeof(SensorEvent));
            var correctByteString0 = ByteString.FromString("000101N    -1000,0 kg ");
            handler.Tell(correctByteString0);
            var result = subscriber.ExpectMsg<SensorEvent>();
            Assert.AreEqual(result.Value, -1000.0);
        }

        [Test]
        public void ResponseHanderlActor_should_parse_correct_string_zero()
        {
            var handler = Sys.ActorOf(Props.Create(() => new ResponseHandlerActor(dummySensor)));
            var subscriber = CreateTestProbe();
            Sys.EventStream.Subscribe(subscriber.Ref, typeof(SensorEvent));
            var correctByteString0 = ByteString.FromString("000101N      000,0 kg ");
            handler.Tell(correctByteString0);
            var result = subscriber.ExpectMsg<SensorEvent>();
            Assert.AreEqual(result.Value, 0.0);
        }

        [Test]
        public void ResponseHanderlActor_should_not_parse()
        {
            var handler = Sys.ActorOf(Props.Create(() => new ResponseHandlerActor(dummySensor)));
            var subscriber = CreateTestProbe();
            Sys.EventStream.Subscribe(subscriber.Ref, typeof(SensorEvent));
            var wrongByteString0 = ByteString.FromString("bla 12335a1351 ");
            handler.Tell(wrongByteString0);
            var result = ExpectMsg<FormatException>();
        }
    }
}
