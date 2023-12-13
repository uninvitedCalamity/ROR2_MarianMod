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
![OutOfRange](https://github.com/uninvitedCalamity/ROR2_MarianMod/blob/main/Images/20231204195618_1.jpg)

### Ice bomb
Marian throws/fires an Ice bomb, freezing enemies for 4.6 seconds

## Known issues
- Grapple "UI" appears when Goobu jr's use Grapple.
- Marian can grapple onto herself
- Config does not show description of colour blindnessmode/Display value, put in value name instead
- Missile UI appears on other players screens
- When using sturdy mug, client player gets infinite grenades at 10 stacks

## Objectives
- Mastery skin
- Create custom muzzle flash
- Maybe set up the mod for emotes???

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
