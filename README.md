# Genetic-Intermediate-Language
A low-level genetic programming language designed to simplify genetic engineering
**This is still early in development, the future of this project is unknown and built versions may be unstable**

Features:
* Automatically optimizes codons for target organism

Planned features:
* Automatically adding promotors to output
* User-created libraries
* Importing genes from files

Possible features (further down the road, if ever):
* Built-in decompiler

# How to use it
## Option 1: bulding from source
Download the source code and use .net 5.0 to build.
Copy the CompilationTargets and Templates folders into the bin\Debug\net5.0 folder.

I reccomend adding the bin\Debug\net5.0 folder to PATH so you can access the GIL compiler from anywhere. You'll need to restart VSCode for this to work. 

### Commands:
* compile - compiles the .gil file with the same name as the current directory (not fully implemented)
* new - creates a new GIL project
* test - used for testing, will be removed later
