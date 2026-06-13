# Escaping an Abandoned Research Facility
You have awoken in an unknown place, having no memory of the past. You explore the underground facillity to try to find a way out. However, you notice some unusual things happening around the facility - objects are enlarged. You salvage as much important research documents and specimens and escape as fast as possible to hopdfully sell them for money. Will you escape this uncertain place or will you fall along with it?

## General Information
### Game Controls
- Keys WASD to move
- Shift key to sprint
- Space key to jump

### Collectibles
- 30 Documents 
    - 20 Points Each
- 8 Test Tube R (Red) 
    - 35 Points Each
- 10 Test Tube G (Green) 
    - 27 Points Each
- 5 Test Tube B (Blue) 
    - 50 Points Each
- 7 Batteries
    - Used to power doors that require batteries.
- 3 Keycards
    - Used to open doors which require higher card clearance.
- 23 Medkits
    - Recover 20 HP. HP cannot go over 100. Unable to use when 100 is at 100.

### Hazards
- Green Substance - Deals 5 damage every 0.1 seconds.
- Red Substance - Deals 10 damage every 1 second, after leaving the red substance, continues to deal 10 damage every 1 second for 3 times.
- Laser - Deals 2 damage and knocks back the player. Continuously running into in to force walking through the laser will deplete HP very fast.

## Location of Collectibles
### Key Collectibles
- After clearing the first jumping obstacle, the room on the right and after jumping through the hole in the wall has **1 keycard** (Behind the experiment pods)
- The room at the right of the long hallway contains **1 keycard** and **1 battery**. (Under a table and Beside a table)
- The room at the left of the long hallway contains **1 battery**. Proceed deeper to reach the laser room with **1 battery**. (At the entrance of the door and behind the terminal of the laser room)

After powering the battery door...
    - On the top right of the room has **1 battery**. (Parkour on the experiment pods in the green substance and jump over the long platform at the second battery powered door)
    - The tables in front of the reactor has **1 keycard**. (Beside a chair)
    - The room on the right of the entrance has **1 battery**. (On the second floor at the stairs area behind a table)
    - On the right side of the room at the red substance, there is a plaform with **1 battery**.
    At the second battery powered door at the fart side of the entrance has **1 battery**. (On the floor)

### Other Collectibles
The other collectibles are scattered all over the level and upon entering each room, if there is a collectible, the user interface will state how many there are.

## Game Hacks
### Cheats
- Scores of each collectible can be adjusted in the inspector.
- Number of batteries in which is already in the battery powered door can be preset. (Click on a battery powered door and under DoorScript > Batteries)
- Player battery count, keycard clearance level, hitpoints can be adjusted in the inspector. (Click on NestedParent_Unpack > PlayerCapsule > PlayerScript)

### Pro tips
1. Collect batteries for the first battery powered door but do not power the first door. Instead head to the laser room and parkour to the top platform. 
2. Push the white box down the stairs to easily get back up if you drop down.
3. Head to the second battery powered door and pick up the battery on the floor and instantly unlock the door without unlocking the first door and finding extra batteries.
4. Level completion speed plays a part in final score. A minumum of 50 and maximum of 300 bonus points if players escape successfully. Dying will not give bonus points.

## Device Specifications
- Minimum: Windows 10, Intel i3, 4 GB RAM, integrated graphics.
- Recommended: Windows 10/11, Intel i5, 8 GB RAM, GTX 1050 or better.

## Limitations and Bugs
- Pushing multiple medkits at the medkit platform will cause them to instead of displaying the highlight when looking at them, hide them from the view of the player.
- Closing doors while standing in the middle of the door frame will cause it to crush you and make you unable to move or open the door again.
- Sometimes doors will not open. This bug is rarely encountered but when the game is reloaded, it should be fine.
- Clipping of game camera into walls to see outside the level. Not that much but it breaks the immersion.
- Certain areas in the level will have the player unable to jump or have a shorter jump height. This is due to mesh colliders being less accurate. Box colliders have been added to important areas which needs jumping.
- Jumping on stairs in the final room only works if standing at the edge of the stairs. Due to mesh collider again.
- In the room below the level with many beds, jumping while close to the ceiling will cause the player to float until the ground is far away or the roof is open. (Does not affect gameplay much)

## Credits
- Larson, A. (n.d.). *Gidole*. Google Fonts. [https://fonts.google.com/specimen/Gidole](https://fonts.google.com/specimen/Gidole).
- vecses. (n.d.). *mc hurt*. Myinstants. [https://www.myinstants.com/en/instant/mc-hurt-49627/](https://www.myinstants.com/en/instant/mc-hurt-49627/).
- The PolynestLab. (2026, May 27). *FREE Demo: Low Poly Sci-Fi Laboratory 2 / Cosmic Retro*. Unity Asset Store. [https://assetstore.unity.com/packages/3d/environments/sci-fi/free-demo-low-poly-sci-fi-laboratory-2-cosmic-retro-378678](https://assetstore.unity.com/packages/3d/environments/sci-fi/free-demo-low-poly-sci-fi-laboratory-2-cosmic-retro-378678).
- The PolynestLab. (2026, April 20). *FREE Demo: Low Poly Sci-Fi Laboratory / Cosmic Retro*. Unity Asset Store. [https://assetstore.unity.com/packages/3d/environments/sci-fi/free-demo-low-poly-sci-fi-laboratory-cosmic-retro-346024](https://assetstore.unity.com/packages/3d/environments/sci-fi/free-demo-low-poly-sci-fi-laboratory-cosmic-retro-346024).
- The PolynestLab. (2026, April 20). *FREE Demo: Low Poly Sci-Fi Hangar / Cosmic Retro*. Unity Asset Store. [https://assetstore.unity.com/packages/3d/environments/sci-fi/free-demo-low-poly-sci-fi-hangar-cosmic-retro-361516](https://assetstore.unity.com/packages/3d/environments/sci-fi/free-demo-low-poly-sci-fi-hangar-cosmic-retro-361516).
