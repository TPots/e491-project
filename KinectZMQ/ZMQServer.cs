using System;
using System.Threading;
using NetMQ;
using NetMQ.Sockets;
using Quaternion = System.Numerics.Quaternion;
using Vector3 = System.Numerics.Vector3;

namespace KinectZMQ
{
    [Serializable]
    public struct Orientation
    {
        public Pos pos;
        public Quat quat;
    }
    [Serializable]
    public struct Pos
    {
        public float x;
        public float y;
        public float z;
    }
    [Serializable]
    public struct Quat
    {
        public float w;
        public float x;
        public float y;
        public float z;

    }


    class ZMQServer
    {
        private readonly PublisherSocket pubSocket;
        public ZMQServer(int port)
        {
            Console.WriteLine("Publisher socket binding...");
            this.pubSocket = new PublisherSocket();
            this.pubSocket.Options.SendHighWatermark = 1000;
            this.pubSocket.Bind($"tcp://*:{port}");
        }
        public void PublishData(Vector3 position, Quaternion quaternion)
        {
            Pos pos = new Pos { x = position.X , y = position.Y, z = position.Z };
            Quat quat = new Quat { x = quaternion.X, y = quaternion.Y, z = quaternion.Z, w= quaternion.W };
            Orientation orient = new Orientation {pos = pos, quat = quat};

            string msg = Newtonsoft.Json.JsonConvert.SerializeObject(orient);
            this.pubSocket.SendFrame(msg);
            Thread.Sleep(1);
        }
    }
}
