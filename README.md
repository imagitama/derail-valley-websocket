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

| Name              | Unit       | Type     | Example  | Description                                                                                        |
| ----------------- | ---------- | -------- | -------- | -------------------------------------------------------------------------------------------------- |
| `CAR_SPEED`       | `kph`      | `float?` | `123.45` | The actual speed of the player's train car.<br />Null if not in a train car.                       |
| `CAR_SPEEDOMETER` | `kph`      | `float?` | `123.45` | The speed displayed on the speedometer in the player's train car.<br />Null if not in a train car. |
| `THROTTLE`        | `position` | `float?` | `0.123`  | The position of the throttle lever from 0 to 100%.                                                 |
| `TRAIN_BRAKE`     | `position` | `float?` | `0.456`  | The position of the train brake lever from 0 to 100%.                                              |
| `REVERSER`        | `position` | `float?` | `0.789`  | The position of the reverser lever from 0 to 100%.                                                 |

### Ports

You can also subscribe to a "standard port" var:

| Name                               | Unit     | Type     | Example             | Description            |
| ---------------------------------- | -------- | -------- | ------------------- | ---------------------- |
| `PORT_WHEELSPEEDKMH`               | `number` |          |                     |
| `PORT_TRACTIONMOTORAMPS`           | `number` |          |                     |
| `PORT_TRACTIONMOTORAMPLIMIT`       | `number` |          |                     |
| `PORT_TRACTIONMOTORAMPLIMITEFFECT` | `number` |          |                     |
| `PORT_TEMPERATURE`                 | `number` |          |                     |
| `PORT_TRACTIONMOTORAMPSMAX`        | `number` |          |                     |
| `PORT_ENGINERPM`                   | `number` | `float?` | `6000` for 6000 RPM | The RPM of the engine. |
| `PORT_ENGINERPMMAX`                | `number` |          |                     |
| `PORT_TURBINERPM`                  | `number` |          |                     |
| `PORT_TURBINERPMMAX`               | `number` |          |                     |
| `PORT_FUEL`                        | `number` |          |                     |
| `PORT_FUELMAX`                     | `number` |          |                     |
| `PORT_OIL`                         | `number` |          |                     |
| `PORT_OILMAX`                      | `number` |          |                     |
| `PORT_SAND`                        | `number` |          |                     |
| `PORT_SANDMAX`                     | `number` |          |                     |
| `PORT_ENGINEON`                    | `number` |          |                     |
| `PORT_FUELLAMPSTATE`               | `number` |          |                     |
| `PORT_OILLAMPSTATE`                | `number` |          |                     |
| `PORT_SANDLAMPSTATE`               | `number` |          |                     |
| `PORT_SANDERLAMPSTATE`             | `number` |          |                     |
| `PORT_WIPERSLAMPSTATE`             | `number` |          |                     |
| `PORT_HEADLIGHTFLAMPSTATE`         | `number` |          |                     |
| `PORT_HEADLIGHTRLAMPSTATE`         | `number` |          |                     |
| `PORT_CABLIGHTLAMPSTATE`           | `number` |          |                     |
| `PORT_ENGINERPMLAMPSTATE`          | `number` |          |                     |
| `PORT_AMPSLAMPSTATE`               | `number` |          |                     |

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
