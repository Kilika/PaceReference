# PaceRef – Enhanced Scoreboard

What it does:
The mod enhances Distance's pause menu by adding leaderboard comparisondata.

Key Features:
1. Global Best Time - Shows the world record time for the current level
1. Top Players Average - Displays the average time of the top players (configurable sample size)

What players see:
When they pause during a level, below the normal information (Personal Best, Medal,Difficulty), they now see:
- Global Best: 01:07.28 (world record time)
- Top X Avg: 01:08.06 (average of top X players, where X is configurable)

But why?
- Pace Reference - Players can instantly see how they compare to the world's best
- Skill Assessment - Shows both the absolute best (world record) and what "verygood" looks like (top players average)
- And a nice: "what the hell how long is this workshop map" - information.

## Features

When opening the pause menu during a level, PaceRef fetches the global Steam leaderboard and displays two additional stats below the existing scoreboard panel:

- **Global Best** – the world record time (or score in Stunt mode)
- **Top N Average** – the average across the top N entries, configurable via BepInEx config

Both time-based modes and Stunt mode (eV scoring) are supported.

## Installation

1. Install [BepInEx 5](https://github.com/BepInEx/BepInEx/releases) for Distance
1. Drop `PaceRef.dll` into `BepInEx/plugins/Mods/`

Or use any thunderstore mod manager of your choice, e.g. [r2modman](https://thunderstore.io/package/ebkr/r2modman/)

## Configuration

| Key | Default | Description |
|---|---|---|
| `Sample Size` | `100` | Number of leaderboard entries used to calculate the top-N average (range: 10–1000) |

The config file is generated on first launch, but the values can be edited in ingame options.

## License

[MIT](LICENSE)
