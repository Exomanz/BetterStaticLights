# BetterStaticLights
Beat Saber mod that lets you choose which lights you want active when "Static Lights" is on.

## Dependencies:
- BSIPA v4.2.0+
- BeatSaberMarkupLanguage v1.5.4+

## Config Options:

**_Option_** | Description
-- | --
**_BackTop_** | Contrary to its name, these are going to be your bottom or top sets of lights
**_RingLights_** | Track Ring Lights
**_LeftLasers_** | Left Laser Set
**_RightLasers_** | Right Laser Set
**_BottomBackSide_** | These are going to be your arrow/logo/focal point lasers, as well as the ones **on or near** the track
**_Off_** | Turns off a set of lights
**_Use Left Saber Color_** | Allows a set of lights to use the left saber color

## Roadpath:
- Fix backwards functionality for `BackTop` light set
  - This is *partially* fixed to the point where it works as expected, but I never changed any base-game behaviour which I'm pretty sure is what causes this to happen.
