#define TRACE

using System;
using System.Numerics;
using System.Diagnostics;
using K4AdotNet;
using K4AdotNet.Record;
using K4AdotNet.Sensor;
using K4AdotNet.BodyTracking;

namespace KinectSim
{
    class KinectPlayback
    {
        static void Main(string[] args)
        {
            // set up error logs
            Trace.Listeners.Add(new TextWriterTraceListener("traceErrors.log"));
            Trace.AutoFlush = true;


            // initialize server at port 12345
            KinectZMQ.ZMQServer server = new KinectZMQ.ZMQServer(12345);

            // get the name of the recording from the console
            Console.WriteLine("Enter the name of the recording: ");
            string filename = Console.ReadLine();

            try
            {
                // open playback
                using (Playback playback = new Playback(filename))
                {
                    var length = playback.RecordLength;

                    // setup tracker based on recording
                    TrackerConfiguration trackerConfiguration = new TrackerConfiguration();
                    playback.GetCalibration(out Calibration calibration);

                    using (Tracker tracker = new Tracker(calibration, trackerConfiguration))
                    {
                        // while frame still available
                        while (playback.TryGetNextCapture(out Capture capture))
                        {
                            // add sensor capture to tracker queue with infinite timeout
                            using (capture)
                            {
                                tracker.EnqueueCapture(capture);
                            }

                            // get latest tracker frame with infinite timeout
                            using (BodyFrame frame = tracker.PopResult())
                            {
                                if (frame.BodyCount != 1)
                                {
                                    continue;
                                }

                                frame.GetBodySkeleton(0, out Skeleton skeleton);

                                Joint eyeRight = skeleton.EyeRight;
                                Joint eyeLeft = skeleton.EyeLeft;
                                // convert K4AdotNet.Quaternion to System.Numerics.Quaternion
                                System.Numerics.Quaternion quaternion = new System.Numerics.Quaternion(
                                    eyeRight.Orientation.X,
                                    eyeRight.Orientation.Y,
                                    eyeRight.Orientation.Z,
                                    eyeRight.Orientation.W
                                );
                                server.PublishData(Vector3(eyeRight.PositionMm), quaternion);
                            }
                        }

                        Console.WriteLine("End of file.");
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("-----START OF ERROR LOG-----");
                Trace.WriteLine(ex);
            }
        }

        private static Vector3 Vector3(Float3 positionMm)
        {
            throw new NotImplementedException();
        }
    }
}
