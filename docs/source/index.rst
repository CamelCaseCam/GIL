.. GIL documentation master file, created by
   sphinx-quickstart on Wed Jun  2 08:15:23 2021.
   You can adapt this file completely to your liking, but it should at least
   contain the root `toctree` directive.
.. To build, run command "sphinx-build -b html source build"

Welcome to GIL's documentation!
===============================
GIL is a collection of tools designed to simplify genetic engineering. 
**This is still early in development, the future of this project is unknown and built versions may be unstable.**

Features
--------
* Converts a sequence of amino acids to a DNA sequence optimized for the target organism
* Converts a DNA sequence between two different organisms

Contribute
----------
- Issue Tracker: https://github.com/CamelCaseCam/GIL/issues
- Source Code: https://github.com/CamelCaseCam/GIL

To see the latest features, check out the developer branch on github

Support
-------
If you're having problems, create an issue on github and/or email me at CamKDev@gmail.com

License
-------
GIL is licensed under the GNU GPL-3.0 license. 



Installation
============
Option 1: bulding from source
-----------------------------
Download the source code and use .net 5.0 to build.

Option 2: Just download the compiled folder
-------------------------------------------
Download the Compiled folder if on windows. If you're on another platform, you'll have to compile the source code for your OS.

I recommend adding the bin\Debug\net5.0 folder to PATH so you can access the GIL compiler from anywhere. You'll need to restart VSCode for this to work.


.. toctree::
   :maxdepth: 2
   :caption: Contents:
   
   GIL/AminoSequence
   GIL/Comments
   GIL/From
   GIL/Regions
   GIL/Target organisms
   What I'm working on



Indices and tables
==================

* :ref:`genindex`
* :ref:`search`
