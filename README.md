# RectiPack
Another implementation of https://github.com/TeamHypersomnia/rectpack2D
I just ported the code to .Net and added a little bit on top.

RectiPack is a library that attempts to solve the [rectangle packing problem](https://en.wikipedia.org/wiki/Rectangle_packing):
Given an arbitrary amount of rectangles, the goal is to pack all rectangles into the smallest space possible without rotating the rectangles.

## Use cases
One of the most common use cases is the creation of spritesheets from single frames or combining textures into a single image.

## How to use this library
Just compile the RectiPack project (prefferably as a release build) and include the resulting **.dll** in your project.
An example program can be found inside the **Demo** folder.

## Credits
Credits go to the original authors of the algorithm [geneotech](https://github.com/geneotech) and [Longri](https://github.com/Longri)
