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
            var dateTime = DateTime.Now;
            Trace.WriteLine("-----START OF ERROR LOG" + dateTime.ToString() + "-----");

            // set flags
            bool verbose = false;

            // initialize server at port 12345
            KinectZMQ.ZMQServer server = new KinectZMQ.ZMQServer(12345);

            // check Body Tracking runtime was included properly
            if(!Sdk.IsBodyTrackingRuntimeAvailable(out string message))
            {
                Trace.Write("Body Tracking runtime is not available.");
                Trace.Write(message);
                return;
            }

            // get the name of the recording from the console
            Console.WriteLine("Enter the name of the recording: ");
            string filename = Console.ReadLine();

            try
            {
                // open playback
                using (Playback playback = new Playback(filename))
                {
                    Console.WriteLine("Recording opened for playback.");
                    var length = playback.RecordLength;

                    // setup tracker based on recording
                    // By default, uses GPU to process tracker
                    TrackerConfiguration trackerConfiguration = new TrackerConfiguration();
                    playback.GetCalibration(out Calibration calibration);
                    if (verbose) { Console.WriteLine($"Recording frame rate: ")}

                    using (Tracker tracker = new Tracker(calibration, trackerConfiguration))
                    {
                        Console.WriteLine("Tracker initialized.");

                        // Start timing
                        double totalFrameCount = 0.0;
                        Stopwatch stopwatch = Stopwatch.StartNew();

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
                                // add to frame count regardless of number of bodies
                                totalFrameCount++;

                                // only process if one body in frame
                                if (frame.BodyCount != 1)
                                {
                                    continue;
                                }

                                frame.GetBodySkeleton(0, out Skeleton skeleton);

                                Joint eyeRight = skeleton.EyeRight;
                                Joint eyeLeft = skeleton.EyeLeft;
                                
                                var positionData = new Vector3(eyeRight.PositionMm.X, eyeRight.PositionMm.Y, eyeRight.PositionMm.Z);
                                server.PublishData(
                                    positionData,
                                    // convert K4AdotNet.Quaternion to System.Numerics.Quaternion
                                    new System.Numerics.Quaternion(
                                        eyeRight.Orientation.X,
                                        eyeRight.Orientation.Y,
                                        eyeRight.Orientation.Z,
                                        eyeRight.Orientation.W
                                    )
                                );

                                // print position to console if verbose
                                if (verbose) { Console.WriteLine($"Position: {positionData}"); }
                            }
                        }

                        stopwatch.Stop();

                        if (stopwatch.Elapsed.TotalSeconds > 0)
                        {
                            Console.WriteLine("End of file.");

                            string elapsedTime = $"Total elapsed time: {stopwatch.Elapsed.TotalSeconds} seconds";
                            Trace.WriteLine(elapsedTime);
                            Console.WriteLine(elapsedTime);

                            var fps = totalFrameCount / stopwatch.Elapsed.TotalSeconds;
                            string trackingSpeed = $"Tracking Speed: {fps} FPS";
                            Trace.WriteLine(trackingSpeed);
                            Console.WriteLine(trackingSpeed);

                            Console.WriteLine("Press enter to exit.");
                            Console.ReadLine();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
            }
        }

        private bool IsTimeInStartEndInterval(TimeSpan time)
        {
            return false;
        }
    }
}
