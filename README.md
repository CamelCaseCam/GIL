# GIL
A collection of tools designed to simplify genetic engineering 
**This is still early in development, the future of this project is unknown and built versions may be unstable**. 
If you want to contribute or see a preview of upcoming releases, check out the developer branch. 

We've got documentation! Check it out at https://geneticil.readthedocs.io/en/latest/

Features:
* Converts a sequence of amino acids to a DNA sequence optimized for the target organism

Features currently in development:
* Automatically generates RNAi to block specified metabolic pathways in the target organism (80% done)
* Multiple optimization settings to control which genes get blocked (not started)

Planned features:
* User-created libraries

# How to use it
## Option 1: bulding from source
Download the source code and use .net 5.0 to build.
Copy the CompilationTargets and Templates folders into the bin\Debug\net5.0 folder.

I reccomend adding the bin\Debug\net5.0 folder to PATH so you can access the GIL compiler from anywhere. You'll need to restart VSCode for this to work. 

### Commands:
* compile - compiles the .gil file with the same name as the current directory (not fully implemented)
* new - creates a new GIL project
* build-pathway - converts a textual representation of a metabolic pathway into serialized Pathway object to reduce performance cost with large pathways
* test - used for testing, will be removed later
