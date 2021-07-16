# Akitacore

**Akitacore** is a set of scripts that provides a broad foundation for structuring code in Unity.

Akitacore provides a template for solving common game development tasks, such as:

**Character Ability Modularity** - Akitacore defines a template for Abilities, which are any action that could be triggered by player input. The ability system defines a standardized interface from mapping inputs to actions, allowing for modular and composable code for any game action.

**NPC Logic** - Akitacore provides a graphical state machine editor for defining NPC behaviors. A StateMachineController allows these state machines to trigger the same Ability scripts that define player actions.

**Dialogue** - Akitacore includes and extends [Yarnspinner](https://yarnspinner.dev/), hooking it into many other systems. YarnSpinner lets you easily draft branching dialogue trees, Akitacore adds functionality that lets you trigger dialogue portraits and game events directly from your dialogue script. 

**Audio** - Unity's default audio systems are great, but they don't work on every platform. Akitacore defines a custom "Radio" interface to serve as a centerpiece of all your audio needs, with full cross-platform compatibility.

**Scene Transitions** - Akitacore uses ScriptableObject wrappers for common data types (bool, string, int, float) to allow easy communication between systems. This has the added benefit of persisting across scene transitions. Set up each of your scenes as a clean break, and use scriptabledata for persistent information, and you won't have to worry about communicating information across scene breaks.

**Saving and Loading** - Since all vital persistent information is saved as ScriptableData, there's an easy library of everything you'd need to save or load on game exit. Axitacore provides ScriptableSaveLoad, an interface for converting your scriptabledata library into a text document format.


## Intended Use


Unity is fairly unopinionated, but also fairly bare bones. Often times the easiest way to do a thing in unity is also bad practice.

Akitacore is intended to be a template for programming. The goal is to provide enough in the way of templates, interfaces, and editor features, that making code the easy way turns into a reasonably well formatted scaleable codebase.

Akitacore is not intended to be a turnkey solution that requires no coding, though in practice, it may work this way for some very simple games.

Akitacore is intended to be useable for any genre of game. The base project is a simple 2d rpg format, but these systems should adapt well for a 3d adventure game, a first person shooter, or a VR adventure. Akitacore is intended to be opinionated in terms of how stuff is made, not what is made with it (though naturally certain features will not apply to some games.)

Akitacore is not intended to be The Best way of structuring a game, this is just a system that makes sense and works well for me. I'm releasing it publically in hopes that it may work well for other folks, too.

I'm a solo developer, not a big company. So Akitacore focuses on optimising for programmer effort. This is hopefully an easy way to make games. This is probably not the most computationally efficient way of making games. My design phiolosophy is, write reasonable code, then optimize when there's a performance bottleneck. Optimizations will be included when I run into issues myself.

## Getting Started


If you want to use Akitacore in your game, follow the instructions for [Installing a unitypackage from a Git URL](https://docs.unity3d.com/Manual/upm-ui-giturl.html).

If you want to be able to edit the akitacore scripts, download akitacore, unzip the package, and place the files in your game's Assets directory. 


## Documentation

Documentation of Akitacore is provided [Here.](https://www.axiakita.com/documentation/akitacore)

This includes descriptions of the various systems, as well as guides on accomplishing common tasks, like writing dialogue for the akitacore system.

I'm still catching up on documenting everything. For the time being, it may be necessary to refer to the code itself to understand how something works.

## License


Akitacore is availiable under the MIT License. This means that you can use it in any commercial or noncommercial project. The only requirement is that you need to include attribution in your game's docs. A credit would be appreciated, but isn't required.

Akitacore makes use of several other open source projects:

[YarnSpinner](https://github.com/YarnSpinnerTool/YarnSpinner-Unity)- [MIT License](https://github.com/YarnSpinnerTool/YarnSpinner-Unity/blob/main/LICENSE.md)
-Yarnspinner is used as the backbone of the dialogue system.

[Serializable Callback](https://github.com/Siccity/SerializableCallback)- [MIT License](https://github.com/Siccity/SerializableCallback/blob/master/LICENSE.md)
-The Serializable Callback inspector code was heavily referenced in writing the inspector for AkitacoreVariables.


## Development

Akitacore is developed by me, [Axi](https://www.axiakita.com). Fundamentally, this is intended as a core set of scripts for my own use, to make it easier to standardize my work and carry code and results across multiple game projects. It's also a great starting point for working with my colleagues.

This is a solo endeavour, and a pretty big project, so I'm afraid I can't provide active support to users. However, bug reports are a huge help, and I'll do my best to address issues that comes up.

