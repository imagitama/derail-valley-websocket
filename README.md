# Derail Valley WebSocket mod

A mod for the game Derail Valley that starts a WebSocket server to expose data from the game for external apps.

Template from https://github.com/derail-valley-modding/template-umm

## Settings

Configure using UMM:

| Name | Type  | Default | Description     |
| ---- | ----- | ------- | --------------- |
| Port | `int` | 9450    | WebSocket port. |

## Messages

See `DerailValleyWebSocket/Messages.cs` for all messages.

After connection you must perform a handshake by sending an "init" message with whatever train car name you have. It will send an "init" message back with the latest car name (TODO only do this if name doesn't match).

Then you can subscribe to vars and events as you like.

## Vars

Var name casing doesn't matter.

| Name              | Unit  | Type     | Example  | Description                                                                                        |
| ----------------- | ----- | -------- | -------- | -------------------------------------------------------------------------------------------------- |
| `CAR_SPEED`       | `kph` | `float?` | `123.45` | The actual speed of the player's train car.<br />Null if not in a train car.                       |
| `CAR_SPEEDOMETER` | `kph` | `float?` | `123.45` | The speed displayed on the speedometer in the player's train car.<br />Null if not in a train car. |

## Events

| Name               | Type      | Example     | Description                                                                    |
| ------------------ | --------- | ----------- | ------------------------------------------------------------------------------ |
| `CAR_NAME_CHANGED` | `string?` | `"LocoDE2"` | The name of the player's train car has changed.<br />Null if not inside a car. |

### Car names

Any train car you can stand on including locomotives, flatbeds, tanks, etc.

Determined from `$type $parentType` (enum `TrainCarType` and `TrainCarLivery.id`).

| Name            | CarName                |
| --------------- | ---------------------- |
| DE2             | `LocoShunter LocoDE2`  |
| Flatbed         | `FlatbedEmpty Flatbed` |
| Blue Tank       | `TankBlue TankGas`     |
| Yellow Oil Tank | `TankYellow TankOil`   |

## Install

Download the zip and use Unity Mod Manager to install it.

## Development

Created in VSCode (with C# and C# Dev Kit extensions) and MSBuild.

1. Clone repo
2. Open `./DerailValleyWebSocket` in VSCode and make changes
3. Run `msbuild` in root
4. Copy `./dist/tmp` to mods folder

## Publishing

1. Run `.\package.ps1`
