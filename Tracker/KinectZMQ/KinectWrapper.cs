using Microsoft.Azure.Kinect.BodyTracking;
using Microsoft.Azure.Kinect.Sensor;
using System.Diagnostics;
using System;


namespace KinectZMQ
{
    class KinectWrapper
    {
        static void Main()
        {
             // set up error logs
            Trace.Listeners.Add(new TextWriterTraceListener("traceErrors.log"));
            Trace.AutoFlush = true;
            var dateTime = DateTime.Now;
            Trace.WriteLine("-----START OF ERROR LOG" + dateTime.ToString() + "-----");

            ZMQServer server = new ZMQServer(12345);

            // check Body Tracking runtime was included properly
            // if(!Sdk.IsBodyTrackingRuntimeAvailable(out string message))
            // {
            //     Trace.Write("Body Tracking runtime is not available.");
            //     Trace.Write(message);
            //     return;
            // }

            // Open device.
            try
            {
                using (Device device = Device.Open())
                {
                    device.StartCameras(new DeviceConfiguration()
                    {
                        CameraFPS = FPS.FPS30,
                        ColorResolution = ColorResolution.Off,
                        DepthMode = DepthMode.NFOV_Unbinned,
                        WiredSyncMode = WiredSyncMode.Standalone,
                    });

                    var deviceCalibration = device.GetCalibration();

                    using (Tracker tracker = Tracker.Create(deviceCalibration, new TrackerConfiguration() { ProcessingMode = TrackerProcessingMode.Gpu, SensorOrientation = SensorOrientation.Default }))
                    {
                        while (true)
                        {
                            using (Capture sensorCapture = device.GetCapture())
                            {
                                // Queue latest frame from the sensor.
                                tracker.EnqueueCapture(sensorCapture);
                            }

                            // Try getting latest tracker frame.
                            using (Frame frame = tracker.PopResult(TimeSpan.Zero, throwOnTimeout: false))
                            {
                                if (frame != null)
                                {
                                    if (frame.NumberOfBodies != 1)
                                    {
                                        continue;
                                    }
                                    Skeleton skeleton = frame.GetBodySkeleton(0);

                                    // Find Right Eye Of User - can later me modified to left eye or
                                    // midpoint of left and right can also be calculated
                                    Joint eye = skeleton.GetJoint(JointId.EyeRight);
                                    server.PublishData(eye.Position, eye.Quaternion);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
            }
        }
    }
}