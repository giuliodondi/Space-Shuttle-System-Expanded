# Space Shuttle System Expanded

## Updated October 2023

My personal fork of SpaceODY's Space-Shuttle-System-Expanded with my own modifications. Tested in 1.12.3

## DEPENDENCIES (install the latest compatible version):
- ASET Consolidated Avionics Pack (for the cockpit props)
- B9 Parts Switch
- KSPWheel
- Community Resource Pack
- Textures Unlimited
- TAC Life Support or Kerbalism, both are supported (Kerbalism is WIP)
- Waterfall
- [My personal fork of Ferram Aerospace Research for the custom aerodynamic model](https://github.com/giuliodondi/Ferram-Aerospace-Research-modded)

## INSTALLATION

- **Check if you already have a GameData/SPACE_SHUTTLE_SYSTEM folder, if so delete it**
- Copy the contents of **GameData** in your KSP GameData, overwriting if asked
- You can ignore the **FARShuttleAerodynamicModel** folder, it's the source code for the custom aerodynamic module
- If you use a joystick, I strongly advise to go to the KSP Control settings and wire wheel stering to the same axis you use for yaw control, else you might have a hard time tracking the runway on landing
- The Shuttle control surfaces come pre-configured to be compatible with my reentry script, if you use other scripts you might want to re-configure them yourself.

## Realistic Aerodynamics (experimental)

### I seem to have found a way to give the Space Shuttle realistic aerodynamics, at long last.

The mod now comes with a single 'Orbiter' part, which is the assembly of the cabin, fuselage, engine mount and both wings.  
Having a single part that comprises most of the orbiter allows me to slap a custom module that implements the realistic Shuttle lift and drag coefficients [taken from the NASA technical documents](https://archive.org/details/nasa_techdoc_19810067693).

The module config is exactly the same as the Ferram wing aerodynamic module, as it must inherid directly from that module class to work in Ferram. For example:

%MODULE[FARSpaceShuttleAerodynamicModel]  
	{  
		%MAC = 15.43  
		%MidChordSweep = 45  
		%b_2 = 23.79  
		%TaperRatio = 0.185  
		%rootMidChordOffsetFromOrig = 9.175, 3.0, 0  
		%massOverride = 0  
	}

The configs are tailored and balanced to the Shuttle's dimensions.

**Caveats / Issues**

- The aerodynamic model is only realistic on the range of angles of attack where I could find data. Outside of it (e.g. if you lose control and spin around) it will behave wildly
- To be able to maintain stability on landing and high-AoA on reentry, the Cl movement calculations as a function of Mach were brutally disabled. The model is now adjusted to require little flap trim on Entry and be on the edge of longitudinal stability on landing
- I had to split the main landing gear bogey into two separate parts while keeping the nose gear as part of the Orbiter part, to get the deploy animations right  
- The cabin hatch is no longer functional. Some animation names clash in the assembled parts, and something had to give
- There appears to be a long-standing bug in KSP where if the root part of your ship is a cargo bay, anything inside of it will have drag calculated even if it should be occluded.

## Functioning split rudder airbrake (Experimental)

I expperimented two different ways to get a functioning rudder airbrake, as the old DECQ part is just a useless animation

**Tail control surface**
I re-baked the model transforms in the Tail control surface part so that they are independently controllable. The part is now configured using stock KSP control surface modules even in RO, because FAR will not accept more than one control surface module on any part.  

The rudder is functional as a speedbrake with some caveats:

- The airbrake sits high up from the centre of mass, as such there is a large pitch up moment when it opens. To trim this out, you need to also activate the body flap as a spoiler part with some negative deflection.
  This is a totally realistic effect, the real Shuttle digital FCS also used the body flap as a trim device during late reentry
- When the airbrake is fully deployed there seems to be some control reversal effect, meaning that if you command yaw right, the Orbiter will yaw to the left, of course this disappears when the airbrake is closed. I'm not really sure if this is a realistic effect after all.


**Split rudder panel**
This is a new part, which is just one of the panels that make up the Shuttle rudder control surface. You can place manually two of these on the standard tail fin and control the mindependently as Ferram control surfaces.  
I do not recommend using this part, as it doesn't generate nearly as much drag as I'd like.



## Other changes:

**Textures and part variants:**
- custom ET textures
- New SRB texture variants for booster and nosecone, added lighter Filament-Wound booster variant 
- Cabin texture reworked, ejection seat panels for Enterprise and old Columbia, custom tile patterns around the side hatch, unique tiles on the forward RCS block
- Cargo bay and engine mount texture reworked
- Fictional Enterprise orbital version
- Tail texture reworked for Challenger and Columbia SILTS pod
- New elevon variant with original Columbia tile pattern
- New normal maps for all part variants
- OMS pod variant without black tiles
- New "empty cockpit" variant with only the IVA shell and no gauges, lighter on the FPS as I like to land IVA but have no use for the gauges

**Waterfall**
- new effects for the SRBs, made a custom smoke-only Smokescreen config for the smoke column
- rework of the Waterfall OMS and SSME configs with minor tweaks

**RO configs**
- higher SRB decoupler force so that they will separate cleanly without the aft separation motors (broken)
- lower wheel braking force
- thermal configs taken from the updated configs in the main RO release, bit higher tolerance on the nose cabin
- Custom aerodynamics module
- Control surfaces come pre-configured for my Shuttle Entry script

---
