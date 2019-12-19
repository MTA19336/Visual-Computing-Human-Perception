Visual Computing-Human Perception

Tabletop Game Augmenting Software
This software assumes you have a set of printed ArUco markers from the included dictionary(4x4_DICT)

It is recommended that you have an external camera, while running this, to properly inspect the 3D projections. However, a built in webcam in a laptop would also suffice.

The printed marker can replace any tabletop game pieces you desire

Here is a step-by-step guide on how to use this software, it is important to note, that in this state the prototype is an unbuild Unity project.

1. Open the Unity project
2. Make sure you are in the MainScene in the Unity Editor
3. Press play
4. Use a camera connected to the computer running the prototype to inspect any printed marker
4a. If the models projected markers are offset by a couple of centimeters, it means you have to adjust the FOV (Field Of View) in the MarkerObjects Camera. This object can be found inside the hierarchy. The FOV adjustment slide can be found in the inspector of the object. Default FOV will be 40. Change this value while the program is running, till you find a satisfying value. OBS! The value will reset when the session stops.
4b. If the prototype is using the wrong webcam, disable that camera in device manager (in Windows).
5. Once the button on the right with the horse on it is clicked a model panel opens up.
5.a Pressing any model on top of a marker, will highlight it in green, and the model is now changeable. Clicking on a new model in the model panel while the green highlight is visible, will change the model to the newly clicked one. The green highlight can be pressed again to "unselect" and hide the highlight.

