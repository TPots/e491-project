using UnityEngine;
using System;
using NetMQ;
using NetMQ.Sockets;
using System.Collections;
using System.Collections.Generic;

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


        private Queue<Orientation> orientation_queue;
        private int orientation_max_size = 3;
        private Orientation queue_average;
         
        public GameObject setupObj;
        public CaveSetup setupScr;
        // Start is called before the first frame update
        void Start()
        {
            string host = "127.0.0.1";
            string port = "12345";
            orientation_queue = new Queue<Orientation>();
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

        private void remove_value_from_average(Orientation value)
        {
            queue_average.pos.x -= value.pos.x / orientation_max_size;
            queue_average.pos.y -= value.pos.y / orientation_max_size;
            queue_average.pos.z -= value.pos.z / orientation_max_size;

            queue_average.quat.x -= value.quat.x / orientation_max_size;
            queue_average.quat.y -= value.quat.y / orientation_max_size;
            queue_average.quat.z -= value.quat.z / orientation_max_size;
            queue_average.quat.w -= value.quat.w / orientation_max_size;
        }

        private void add_value_to_average(Orientation value)
        {
            queue_average.pos.x += value.pos.x / orientation_max_size;
            queue_average.pos.y += value.pos.y / orientation_max_size;
            queue_average.pos.z += value.pos.z / orientation_max_size;

            queue_average.quat.x += value.quat.x / orientation_max_size;
            queue_average.quat.y += value.quat.y / orientation_max_size;
            queue_average.quat.z += value.quat.z / orientation_max_size;
            queue_average.quat.w += value.quat.w / orientation_max_size;
        }

        // Update is called once per frame
        private void Update()
        {
            if (subSocket.TryReceiveFrameString(out var message)){
                    Debug.Log("Recieved Data");
                    Debug.Log(message);
                    try{ 
                        orientation = JsonUtility.FromJson<Orientation>(message);
                    
                    
                    if (orientation_queue.Count == orientation_max_size)
                    {
                        Orientation removed_value = orientation_queue.Dequeue();
                        remove_value_from_average(removed_value);
                    }
            
                    
                    
                    orientation_queue.Enqueue(orientation);
                    add_value_to_average(orientation);
                        user_pos.x = queue_average.pos.x/factor;
                        user_pos.y = queue_average.pos.y/-factor;
                        user_pos.z = queue_average.pos.z/-factor;
                        user_rot.x = queue_average.quat.x/factor;
                        user_rot.y = queue_average.quat.y/factor;
                        user_rot.z = queue_average.quat.z/factor;
                        user_rot.w = queue_average.quat.w/factor;
                    Debug.Log(queue_average.pos.x);
                    Debug.Log(queue_average.pos.y);
                    Debug.Log(queue_average.pos.z);
                }
                    catch{
                        Debug.Log($"Failed to Parse Message: {message}");
                    }
            setupScr.UpdateFromZMQ(user_pos, Vector3.zero);

            //gameObject.transform.SetPositionAndRotation( user_pos,user_rot);


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
