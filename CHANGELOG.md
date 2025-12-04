# 1.3.0

- added standard simport support:\
   var: `port_EngineRPM`\
   unit: `number`
- renamed display name

# 1.2.0

- added var `train_brake` (`position`)

# 1.1.0

- added var `throttle` (`position`)

# 1.0.3

- add car type into name
- added port setting

# 1.0.2

- fixed emit pausing on loading a new game
- fixed crash on return to main menu
- fixed `System.InvalidOperationException: Collection was modified; enumeration operation may not execute.` error
- improved WebSocket error handling and logging

# 1.0.1

- changed car speedo to use UI if available, otherwise use port, otherwise use absolute speed

# 1.0.0

Initial version
