
InputState is a WIP and not ready for use.


Currently, inputs are defined from unity's input system, and mapped to abilities by a PlayerController.

This doesn't handle certain issues well. For example, 
-what if you need 6 directional input for a flight controller?
-what if you need input chords (up-b to uppercut) rather than simple directional or button inputs?
-How do you remap inputs, either live or in-editor, without changing them in every scene?



So the idea is to create some sort of input scriptableobject.

Playercontrollers will thus map InputObjects to Abilities.
InputObjects will need:
-Some public function that tells the controller whether the input exists or not.
-some functions for getting the specific input data for the Ability to use.