# Marian.

Adds Marian to ROR2, a cyborg snake lady.
Multiplayer compatible, if you have any feedback please @ me (uninvitedCalamity) on the ROR2 modding discord or dm me at uninvitedCalamity#1334.
Colour blindness UI modifiers added, please let me know they're not good enough.

![Lobby](https://raw.githubusercontent.com/uninvitedCalamity/ROR2_MarianMod/main/Images/20231204190434_1.jpg?token=GHSAT0AAAAAACLFR6QDVL6CULB2GEYSHXSKZLOFUIQ)

## Overview

Marian is a Manuverable close range survivor with a mid-range option.

### Scatter gun
Scatter gun is a full auto shotgun

### Smart Missiles
Smart missiles Lock onto upto 5 enemies on screen, afflicting blight on impact.
Always fires 5 missiles, tries to evenly distribute missiles across enemies.
![TargetLockon](https://raw.githubusercontent.com/uninvitedCalamity/ROR2_MarianMod/main/Images/20231204190516_1.jpg?token=GHSAT0AAAAAACLFR6QCMXN63PE4CZK2PXOGZLOFVAQ)

### Grapple
Grapple changes Marian's primary skill.
Grapple primary will pull Marian towards Grappled point, Grapple will gradually slow Marian down when close to grapple point (Unless attached to an enemy), will deal damage and stun to grappled enemies.
Jumping increases velocity.
Grapple will pull until skill 1 is released, you jump, or grappled enemy is contacted.
Range is indicated on the "UI"
Close range:
![InRangeClose](https://raw.githubusercontent.com/uninvitedCalamity/ROR2_MarianMod/main/Images/20231204195612_1.jpg)
Long range:
![InRangeLong](https://raw.githubusercontent.com/uninvitedCalamity/ROR2_MarianMod/main/Images/20231204195610_1.jpg)
Out of range:
![OutOfRange](https://raw.githubusercontent.com/uninvitedCalamity/ROR2_MarianMod/main/Images/20231204195618_1.jpg)

### Ice bomb
Marian throws/fires an Ice bomb, freezing enemies for 4.6 seconds

## Known issues
- Config does not show description of colour blindnessmode/Display value, put in value name instead
- Grapple does not automatically cancel on miss if the raycast finds something

## Objectives
- Maybe set up the mod for emotes???

## Version 1.2.2
- Poison bomb and Icemissiles were not overiting damage type on client, now overite used projectile

## Version 1.2.1
- Added alt Grenade and Missiles
- Re-added shapekey anims for blinking and flinching
- Changed Grapple UI

## Version 1.2.0
- Smart missile ping stops pinging after third charge, until targets are lost
- Smart missile Timing fixed and adjusted
- Smart missile Buff managed entirely by player, Client or Host, by spawning an additional projectile that manages how much of the buff should be active
- Changed charge effects, different one for each stage
- Removed logs from Smart missile
- Grapple re-casts from hit point if it hits Marian
- Grapple Auto exits and restocks if grapple is out of range but in raycast

## Version 1.1.3
- UI for Grapple and Missiles only instantiates on Player characters with base authority

## Version 1.1.2
- UI scaler was treating config values below 100 as 0

## Version 1.1.1
- Increased minimum size of smart missile UI, Interpolates between two set values-
- Missiles charge in sets of three
- Add UI scale option to Config
- Mastery skin is darker

## Version 1.1.0
- Added mastery skin, Model and rig had to be re-imported for all skins, if there are any issues let me know
- Changed Icebomb cool down to 13 seconds
- Grapple switch restores stock on cancel or miss
- Smart missile increases missiles over time while it is targeting enemies, up to 50
- Smart missile Targeting ui is smaller

## Version 1.0.12
- Grapple switch exits when skill 1 held and out of range
- Adjusted animation layers and Gun parent to allow bending up and down when aiming grapple

## Version 1.0.11
- No longer double plays exit animation when attempting to grapple out of range or exiting manually

## Version 1.0.10
- Marian no longer gets stuck in grapple state when frozen

## Version 1.0.9
- Icebomb is no longer infinite with sturdy mug on client, however no longer functions.

## Version 1.0.8
- Missiles restock if there are no enemies to target, does not fire
- Grapple animation for switch ends before player can fire
- Changed sound of Icebomb Detonation
- Removed commended aim Override request from Grapple switch code
- Muzzle flashes appear on gun
- Animations are networked sort of, The Grapple switch exit animation doesn't 100% lineup but whatevs
- Added seconds to grenade description

## Version 1.0.7
- Grapple switch ends after kicking an enemy, or when attempting to grapple outside grapple range.

## Version 1.0.6
- Added animations for Grapple and Icebomb
- Icebomb speed no longer effected by attackspeed
- Added toggle for item displays in config
- Added Config settings for Colour blindness
- Updated descriptions for primary and secondary to show 5*<damage>
- Updated parenting of Roll of pennies
- Can now fire immediately out of jumping in Grapple

## Version 1.0.5
- Changed Icebomb proc, radius and recharge time (17 sec)
- Removed Henry's throw bomb skill
- Added Marian skills to states.cs
- Adjusted animator, Gun is now effected by animations
- Primary uses Base aimray origin for angle anf firepoint calc
- Ice bomb now uses it's own Explosion effect
- Grapple skill no-longer gets stuck if quick tapped
- Primary now makes sound on other player's games

## Version 1.0.4
- Taking off your shorts in the lobby nolonger causes index out of bounds error
- Increased size of "UI"

## Version 1.0.3
- Add compatibility with Auto sprint
- Primary is no longer canceled by sprint, but still cancels sprinting (You could tap sprint to interrupt the firecycle and fire faster)

## Version 1.0.2
- Remove / mute logs in Primary, Missile and Grapple Switch

## Version 1.0.1
- Fixed issue with Icebombs not spawning from client players
- Check for Null Target in marian_grapple_simple

## Version 1.0.0
- Initial upload

# Credit
- Model, Animations and Abilities made by Uninvited Calamity (Me :D, Blend file can be downloaded from https://github.com/uninvitedCalamity/ROR2_MarianMod/tree/main/BlendFile/GitCopy).
- Character built using the Henry Example by ArcPh1r3.
- Lodington for the Cheats for testing and sinai-dev for Unity explorer.
- HIFU for informing me of the one line needed to fix the icebomb issue.
- Kessmonster for telling me about the auto-run issues.
- James for telling me about issues with Tinkers satchel, and pointing out how Glaring the Mastery skin's white is.
- stevarian for telling me about the grapple stuck on freeze glitch.
- drinksenoughcoffeetoascend for feedback on Grapple, Icebomb and Smart Missile.
