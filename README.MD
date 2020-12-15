# Emperor: Battle for Dune - reverse engineering #
This repository is split up into the following projects:

## LibEmperor ##
The main goal of this project is to provide the logic to parse the game assets for Emperor: Battle for Dune.
This is helpful for modding tool development, modding the game itself or developing a remake.

The current state of the project is:
- Audio: Mostly working, but compressed files have a continuous noise.
- Maps: Unimplemented
- Missions: Unimplemented
- Fonts: Unimplemented
- Models: All data is parsed. The vertex animation part however is unfinished.

## EbfdExtractor ##
Extracts the contents of RFH/RFD and BAG files.
**Re-Compression is not yet supported.**

## XbfViewer ##
An example xbf viewer using LibEmperor to read and view Xbf models.
Besides of being a viewer, its setup it the perfect kickstart for modding utilities or a remake.
**Vertex animations not yet supported.**
