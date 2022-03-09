// VRPN Server tutorial

// by Sebastien Kuntz, for the VR Geeks (http://www.vrgeeks.org)

// August 2011

#include <stdio.h>

#include <tchar.h>

#include <math.h>

#include "vrpn_Text.h"

#include "vrpn_Tracker.h"

#include "vrpn_Connection.h"

#include <iostream>

#include <k4a/k4a.h>
#include <k4abt.h>
#define VERIFY(result, error)                                                                            \
    if(result != K4A_RESULT_SUCCEEDED)                                                                   \
    {                                                                                                    \
        printf("%s \n - (File: %s, Function: %s, Line: %d)\n", error, __FILE__, __FUNCTION__, __LINE__); \
        exit(1);                                                                                         \
    }                                                                                                    \


using namespace std;

/////////////////////// TRACKER /////////////////////////////

// your tracker class must inherit from the vrpn_Tracker class

class myTracker : public vrpn_Tracker

{

public:

    myTracker(vrpn_Connection* c = 0);

    virtual ~myTracker() {};

    virtual void mainloop();

protected:

    struct timeval _timestamp;
    k4a_device_t _device;
    k4abt_tracker_t _tracker;

};

myTracker::myTracker(vrpn_Connection* c /*= 0 */) :

    vrpn_Tracker("Tracker0", c)

{
    k4a_device_t device = NULL;
    VERIFY(k4a_device_open(0, &_device), "Open K4A Device failed!");
    // Start camera. Make sure depth camera is enabled.
    k4a_device_configuration_t deviceConfig = K4A_DEVICE_CONFIG_INIT_DISABLE_ALL;
    deviceConfig.depth_mode = K4A_DEPTH_MODE_NFOV_UNBINNED;
    deviceConfig.color_resolution = K4A_COLOR_RESOLUTION_OFF;
    VERIFY(k4a_device_start_cameras(_device, &deviceConfig), "Start K4A cameras failed!");

    k4a_calibration_t sensor_calibration;
    VERIFY(k4a_device_get_calibration(_device, deviceConfig.depth_mode, deviceConfig.color_resolution, &sensor_calibration),
        "Get depth camera calibration failed!");

    k4abt_tracker_configuration_t tracker_config = K4ABT_TRACKER_CONFIG_DEFAULT;
    VERIFY(k4abt_tracker_create(&sensor_calibration, tracker_config, &_tracker), "Body tracker initialization failed!");
}

void

myTracker::mainloop()

{
    k4a_capture_t sensor_capture;
    k4a_wait_result_t get_capture_result = k4a_device_get_capture(_device, &sensor_capture, K4A_WAIT_INFINITE);
    if (get_capture_result == K4A_WAIT_RESULT_SUCCEEDED)
    {
        frame_count++;
        k4a_wait_result_t queue_capture_result = k4abt_tracker_enqueue_capture(_tracker, sensor_capture, K4A_WAIT_INFINITE);
        k4a_capture_release(sensor_capture); // Remember to release the sensor capture once you finish using it
        if (queue_capture_result == K4A_WAIT_RESULT_TIMEOUT)
        {
            // It should never hit timeout when K4A_WAIT_INFINITE is set.
            printf("Error! Add capture to tracker process queue timeout!\n");
            return;
        }
        else if (queue_capture_result == K4A_WAIT_RESULT_FAILED)
        {
            printf("Error! Add capture to tracker process queue failed!\n");
            return;
        }

        k4abt_frame_t body_frame = NULL;
        k4a_wait_result_t pop_frame_result = k4abt_tracker_pop_result(_tracker, &body_frame, K4A_WAIT_INFINITE);
        if (pop_frame_result == K4A_WAIT_RESULT_SUCCEEDED)
        {
            // Successfully popped the body tracking result. Start your processing

            size_t num_bodies = k4abt_frame_get_num_bodies(body_frame);
            // Only want to track a single body
            if (num_bodies != 1) {
                return;
            }
            k4abt_skeleton_t skeleton;
            k4a_result_t skeleton_result = k4abt_frame_get_body_skeleton(body_frame, 0, &skeleton);
            if (skeleton_result == K4A_RESULT_FAILED) {
                printf("Failed to get body skeleton for index: %i\n", 0);
                return;
            }
            k4abt_joint_t eye_right = skeleton.joints[K4ABT_JOINT_EYE_RIGHT];
            //printf("Right Eye Position: %f,%f,%f\n", eye_right.position.v[0], eye_right.position.v[1], eye_right.position.v[2]);
            
            vrpn_gettimeofday(&_timestamp, NULL);
            vrpn_Tracker::timestamp = _timestamp;
            // We will just put a fake data in the position of our tracker
            static float angle = 0; angle += 0.001f;

            // the pos array contains the position value of the tracker
            pos[0] = eye_right.position.v[0];
            pos[1] = eye_right.position.v[1];
            pos[2] = eye_right.position.v[2];

            // the d_quat array contains the orientation value of the tracker, stored as a quaternion
            d_quat[0] = eye_right.orientation.v[0];
            d_quat[1] = eye_right.orientation.v[1];
            d_quat[2] = eye_right.orientation.v[2];
            d_quat[3] = eye_right.orientation.v[3];

            char msgbuf[1000];

            d_sensor = 0;

            int  len = vrpn_Tracker::encode_to(msgbuf);

            if (d_connection->pack_message(len, _timestamp, position_m_id, d_sender_id, msgbuf,

                vrpn_CONNECTION_LOW_LATENCY))

            {

                fprintf(stderr, "can't write message: tossing\n");

            }

            server_mainloop();





            k4abt_frame_release(body_frame); // Remember to release the body frame once you finish using it
        }
        else if (pop_frame_result == K4A_WAIT_RESULT_TIMEOUT)
        {
            //  It should never hit timeout when K4A_WAIT_INFINITE is set.
            printf("Error! Pop body frame result timeout!\n");
            return;
        }
        else
        {
            printf("Pop body frame result failed!\n");
            return;
        }
    }
    else if (get_capture_result == K4A_WAIT_RESULT_TIMEOUT)
    {
        // It should never hit time out when K4A_WAIT_INFINITE is set.
        printf("Error! Get depth frame time out!\n");
        return;
    }
    else
    {
        printf("Get depth capture returned error: %d\n", get_capture_result);
        return;
    }
    
}

////////////// MAIN ///////////////////

int _tmain(int argc, _TCHAR* argv[])

{

    // Creating the network server

    vrpn_Connection_IP* m_Connection = new vrpn_Connection_IP();

    // Creating the tracker

    myTracker* serverTracker = new myTracker(m_Connection);

    cout << "Created VRPN server." << endl;

    while (true)

    {

        serverTracker->mainloop();
        m_Connection->mainloop();

        // Calling Sleep to let the CPU breathe.

        SleepEx(1, FALSE);

    }

}