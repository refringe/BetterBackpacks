# Better Backpacks

This is a mod for [Single Player Tarkov](https://www.sp-tarkov.com/).

Make backpacks bigger while staying in-line with vanilla gameplay balance based on price, rarity, and weight.

## Features:

- Increases backpack grid sizes across all tiers, with buffs scaled by rarity/availability.
- Boss backpack have been made significantly larger to fit their status.
- The backpack grid system has been modified to allow for multi-grid backpacks to be resized properly.

## To Install:

1. Decompress the contents of the download into your root SPT directory.
2. Start the game and enjoy better backpacks.

If you experience any problems, please [submit a detailed bug report](https://github.com/refringe/BetterBackpacks/issues).

## To Build Locally:

This project has two components built with different .NET SDKs:

- **Client** — A BepInEx plugin targeting `netstandard2.1`
- **Server** — An SPT server mod targeting `net9.0`

To build the project locally:

1. Clone the repository.
2. Run `.\build.ps1` in the project root.
3. The distributable zip will be created at the project root.

## Acknowledgements

Thanks to [Josh Mate](https://forge.sp-tarkov.com/user/21711/josh-mate) for the original mod that inspired this project.
