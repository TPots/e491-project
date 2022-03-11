using UnityEngine;
using System;
using NetMQ;
using NetMQ.Sockets;

namespace PubSub {
    public class UserTracker : MonoBehaviour
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
        [SerializeField] private string host;
        [SerializeField] private string port;
        [SerializeField] public float factor = 100; 

        private Orientation orientation;
        private Vector3 user_pos;
        private Quaternion user_rot;
        private SubscriberSocket subSocket;

        // Start is called before the first frame update
        void Start()
        {
            user_pos = new Vector3(0,0,0);
            user_rot = new Quaternion(0,0,0,0);
            // Needed to handle socket exception on succesful socket connection
            AsyncIO.ForceDotNet.Force();
            subSocket = new SubscriberSocket();
            subSocket.Options.ReceiveHighWatermark = 1000; 
            Debug.Log($"Connecting Sub Socket to address: tcp://{host}:{port}");       
            subSocket.Connect($"tcp://{host}:{port}");
            subSocket.SubscribeToAnyTopic();
        }

        // Update is called once per frame
        private void Update()
        {
            if (subSocket.TryReceiveFrameString(out var message)){
                    // Debug.Log(message);
                    try{ 
                        orientation = JsonUtility.FromJson<Orientation>(message);
                        int factor = 100;
                        user_pos.x = orientation.pos.x/factor;
                        user_pos.y = orientation.pos.y/factor;
                        user_pos.z = orientation.pos.z/factor;
                        user_rot.x = orientation.quat.x/factor;
                        user_rot.y = orientation.quat.y/factor;
                        user_rot.z = orientation.quat.z/factor;
                        user_rot.w = orientation.quat.w/factor;
                    }
                    catch{
                        Debug.Log($"Failed to Parse Message: {message}");
                    }
            gameObject.transform.SetPositionAndRotation(user_pos,user_rot);


}
        }

    void OnApplicationQuit()
    {
        subSocket.Close();
        NetMQConfig.Cleanup();
        Debug.Log("Application ending after " + Time.time + " seconds");
    }
    }
}
