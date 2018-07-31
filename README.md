# Unity Test Scenario - UR5/ManusVR/HTC VIVE
Author: **Esteban Segarra**
Primary mantainer: **Esteban Segarra**

# Introduction/Background
This Unity assets folder was built as a result of developing the Manus VR API for C#. This project was intended as a way of observing the consequences and advantages of using alternate virtual reality controls for manipulating virtual scenes for human-robot interaction (HRI). Through applying the ManusVR gloves, observing the results from such operations we can determine how easy was it to manipulate a real-world UR5 robotic arm as well as how easy is it to use the HTC VIVE controllers for similar control 

## Required Hardware
- HTC VIVE Regular or Pro

## Optional but possible equipment 
- UR5 Robotic Arm
- Manus VR Gloves

## Software Requirements
- Unity 3D 2017 2.1 or better (Not tested on newer versions yet). 

## Optional Software
- Manus VR SDK V1.1.1 (Optional if you intend to use the Manus VR Gloves) 
- Collaborative Robotics Programming Interface by National Institute of Standards and Technology (CRPI) communication/processing API
  https://github.com/usnistgov/CRPI

# Quick Start
Clone this repository and copy over the files onto a fresh Unity Project. Assume this repository to be an assets folder as in Unity. By copying over the files. Once installed, located the unity scene named **ManusVR - VIVE - UR5 Sim**. This file will contain most of the UR5 simulation scene built for Unity. This scene should be out-of-box ready to use. 

# Synopsis
The primary focus of this project was being able to simulate and control a UR5 robot using the virtual controls. The form to control the robot more or less equivalent to being able to move around the final end point of the robot around the space around it. The joint angles that the robot would require to move around the world will be calculated through inverse kinematics. These calculations can then be transferred as an array to the simulated robot as well as the real-world robot. 

# Example usage
Start the Unity project. Locate the gameobject called handlepoint. This point is the gameobject **targetted** for the inverse kinematics to point towards. The handlepoint can be located anywhere except in locations that create singularity problems such as directly onto the Z axis, it's own base, or an impossible configuration. 

Care must be taken to avoid sending data to the real-life UR5 about impossible configurations. If you would like to understand use, start by moving the sim UR5 toolpoint around the world with a HTC VIVE and undersand it's behaviour. One special thing to note about the UR5 and CRPI is that CRPI will tend arrive to a location through a motion within 360 degrees. For example if the tool point is moved from the 1st quadrant to the 4th quadrant, the robot will move through the 2nd and 3rd quadrant to rotate to the final point. This is due limitations imposed by the inverse kinematics to avoid damage to the base. 

## Special Notes
Some minor errors to note include the following: 
- Unity has a rotational error of -42.18 degrees on the base joint of the UR5 robot.This is due to the cartersian system of Unity being radically different from that of robotic convention (ZXY vs. XYZ). This cartersian difference is noted through a XYZ ruler adjusted to reflect the robotic coordinate system paired to the real-life UR5 added to the Unity scene. 
- Subtle control of the simulated robot is equivalent to that of the real UR5 robot. However, since the UR5's scale is much smaller than that of the real-life robot in Unity, the controls will appear to be missing fine-tuning control due to the scaling issue. This problem could be mitigated by increating the scale of the UR5 (Not tested, but a minor danger could be throwing off the inverse kinematics algorithm due to a difference in scaling.) 
- The Manus VR Gloves use bluethooth technologies to transmit data back and forth. The developers of the Manus VR however have not adapted shielding to the reciever to avoid interference. As a result some data could be obscured with noise. A Kalhman filter is planned to be incoporated in the future.

## TODO 
- A Kalhman filter for Manus VR data
- Issue when building-> Dependency mismatch 
- Improve Manus VR rotational control over objects
- ~HTC VIVE controllers bug out of being able to control UR5 Tool point after rotating it (Most likely a script issue)~
- Optimization (At preset is ok, but could be better) 

## Disclaimer
Certain commercial equipment, instruments, or materials are identified in this paper to foster understanding. Such identification does not imply recommendation or endorsement by the National Institute of Standards and Technology, nor does it imply that the materials or equipment identified are necessarily the best available for the purpose.

## Acknowledgements
- Megan Zimmerman for providing the initial code
- Manus VR
- The NIST SURF program
- Licensed Under Apache 3.0 
- Albert Hwang for test Scene
- SteamVR 
