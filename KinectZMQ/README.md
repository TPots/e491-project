#  Kinect Tracker ZMQ Server

## Introduction

Tracks user eye position and publishes data to a ZMQ socket.
* initializes the Kinect sensor and body tracker,
* passes frames from the sensor to the tracker,
* pops results from the tracker:
  * Finds User Eye
  * Publishes data to localhost port ZMQ

## Data Format
The data is serialized into a JSON String containing user eye position and rotation (quaternion). Here is an example JSON Message:
```{"pos":{"x":-151.277161,"y":-360.512054,"z":519.8081},"quat":{"w":0.298143238,"x":0.0766123,"y":0.943669438,"z":-0.121363983}}```

## Usage Info

USAGE: KinectZMQ.exe
* there are no arguments, just run the executable. Server publishes data to port ```12345```.
