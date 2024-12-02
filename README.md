![FirstHand Banner](./Media/banner.png "FirstHand")

# First Hand

First Hand is an example of a full game experience using *[Interaction SDK](https://developer.oculus.com/documentation/unity/unity-isdk-interaction-sdk-overview/)* for interactions. It is designed to be used primarily with handtracking but supports controllers throughout.

# Meta Quest Store
You can find the full version of the First Hand app on the Meta Quest Store:

https://www.meta.com/experiences/first-hand/5030224183773255/

## Licenses
The majority of *First Hand* is licensed under [MIT LICENSE](./LICENSE), however files from ThirdParty[Assets/ThirdParty], are licensed under their respective licensing terms.

## Getting started

First, ensure you have Git LFS installed by running this command:
```sh
git lfs install
```

Then, clone this repo using the "Code" button above, or this command:
```sh
git clone https://github.com/oculus-samples/Unity-FirstHand.git
```

All of the actual project files are in Assets/Project. This folder includes all scripts and assets to run the sample, excluding those that are part of the Interaction SDK.
The project includes v68 of the Oculus SDK, including the Interaction SDK.

To run the sample, open the project folder in *Unity 2022.3.22f1* or newer and load the [Assets/Project/Scenes/Level/FirstLoad](Assets/Project/Scenes/Level/FirstLoad.unity) scene.

## Scenes

### Clock Tower

The clock tower scene demonstrates a variety of Hand Tracking interactions using ISDK. To run the scene in the edtor load the [DevClocktower](Assets/Project/Scenes/Level/DevClocktower.unity) scene and [ClockTower](Assets/Project/Scenes/Art/ClockTower.unity) scene. The scene contains several interactable objects.

*Lift Control* - Demonstrates "Hand Grab" and "Poke" interactions.

*Keypad* - Demonstrates "Poke" interactions.

*Glove Schematic Pieces* - Demonstrates "Hand Grab" with "Two Grab Free Transformer" interactions.

*Glove Pieces* - Demonstrates "Touch Grab" interactions.

*Schematic UI* - Demonstrates "Poke" interactions with a Unity canvas.

*Palm Blast and Shield* - Demonstrates "Pose Detection" functionality.

For more information on these interactions please reference the *[Interaction SDK documentation](https://developer.oculus.com/documentation/unity/unity-isdk-interaction-sdk-overview/)*

### Street

The street scene demonstrates ISDK's Teleport Locomotion features. To run the scene in the edtor load the [DevStreet](Assets/Project/Scenes/Level/DevStreet.unity), [Street](Assets/Project/Scenes/Art/Street.unity) and [Street](Assets/Project/Scenes/Art/Street.unity) scenes.

*Hand gesture based locomotion* - Allows the user to locomote about the scene using hand gestures for teleportation

*Controller based locomotion* - Demonstrates standard teleport locomotion input using controllers

### Mixed Reality

The mixed reality scene demonstrates passthrough and Voice SDK features. Follow the instructions for setting up Voice SDK, refer to the [Voice SDK documentation](https://developers.meta.com/horizon/documentation/unity/voice-sdk-overview/).

To set up the wit.ai project:
1. Define an Entitie called "Object" and add the following Keywords: "center", "right side", "left side", "fuse", "broken fuse" and "box"
2. Define these Intents: "Pick_Up", "Move"
3. Use Understanding to assign the Object Entity to the Pick_Up and Move intents, using phrases like "fly to the left side", "fly to the right side" and "pick up the box"

To run the scene in the edtor load the [DevMixedReality](Assets/Project/Scenes/Level/DevMixedReality.unity) scene and the [MRPortals](Assets/Project/Scenes/Art/MRPortals.unity) scene. Once Voice SDK is set up the scene will respond to voice commands.

*Passthrough* - Seeing the device passthrough feed composited with the virtual content

*Scene Understanding Placement* - Virtual objects are placed on the user's walls when scene understanding is set up

*Drone Voice Commands* - Trigging gameplay sequences using the players voice

### Haptics

The haptics scene demonstrates the features of the Haptics SDK. To run the scene in the edtor load the [DevHaptics](Assets/Project/Scenes/Level/DevHaptics.unity) scene and the [MountainPeak](Assets/Project/Scenes/Art/MountainPeak.unity) scene.

*Kinesis Modules* - Demonstrate ISDK's Distance Grab feature

*Kite Controller* - Demonstrates modulating haptics at runtime
