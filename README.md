# Space Shuttle System Expanded

My personal fork of SpaceODY's Space-Shuttle-System-Expanded with my own modifications. Tested in 1.12.3

## DEPENDENCIES (install the latest compatible version):
- ASET Consolidated Avionics Pack (for the cockpit props)
- B9 Parts Switch
- KSPWheel
- Community Resource Pack
- Textures Unlimited
- TAC Life Support (TACLS)


Non-exhaustive list of changes:  
**Textures and part variants:**
- custom ET textures
- New SRB texture variants for booster and nosecone, added lighter Filament-Wound booster variant 
- Cabin texture reworked, ejection seat panels for Enterprise and old Columbia, custom tile patterns around the side hatch, unique tiles on the forward RCS block
- New tail texture for Challenger
- New elevon variant with original Columbia tile pattern
- New normal maps for all part variants
- OMS pod variant without black tiles
- New "empty cockpit" cabin variant with only the IVA shell and no gauges, lighter on the FPS as I like to land IVA but have no use for the gauges

**Waterfall**
- new effects for the SRBs, made a custom smoke-only Smokescreen config for the smoke column
- rework of the Waterfall OMS and SSME configs with minor tweaks

**RO configs**
- higher SRB decoupler force so that they will separate cleanly without the aft separation motors (broken)
- lower wheel braking force
- thermal configs taken from the updated configs in the main RO release, bit higher tolerance on the nose cabin
- CL adjustments for the cargo bay 
- elevon and tail surface Ferram control settings come pre-configured for my Shuttle Entry script


# Realistic Aerodynamics branch (experimental)

The *orbiter_mono* branch contains my promising attempt at giving the Space Shuttle realistic aerodynamics

The Shuttle Orbiter now comes with a single 'Orbiter' part, which is the assembly of the cabin, fuselage, engine mount and both wings.  
Having a single part that comprises most of the orbiter allowed me to define a **FARSpaceShuttleAerodynamicModel** that implements the realistic Shuttle lift and drag coefficients taken from [taken from the NASA technical documents](https://archive.org/details/nasa_techdoc_19810067693).

The module config is exactly the same as the Ferram wing aerodynamic module:

%MODULE[FARSpaceShuttleAerodynamicModel]  
	{  
		%MAC = 15.43  
		%MidChordSweep = 45  
		%b_2 = 23.79  
		%TaperRatio = 0.185  
		%rootMidChordOffsetFromOrig = 9.175, 3.0, 0  
		%massOverride = 0  
	}

these configs are tailored and balanced to the Shuttle's dimensions.

**Caveats / Issues**

- the aerodynamic model is only realistic on the range of angles of attack where I could find data. Outside of it (e.g. if you lose control and spin around) it will behave wildly   
- I had to split the main landing gear bogey into two separate parts while keeping the nose gear as part of the Orbiter part, to get the deploy animations right  
- The cabin hatch is no longer functional. Some animation names clash in the assembled parts, and something had to give
- There appears to be a long-standing bug in KSP where if the root part of your ship is a cargo bay, anything inside of it will have drag calculated even if it should be occluded.



# Split airbrake branch (experimental)

The *splitbrake* branch contains a new part I made in Blender from the original DECQ rudder.
It is an individual rudder panel, a pair of these should be surface-attched to the Shuttle tailfin. Each has their own Ferram configuration so they can act as spoilers individually.  
Functionally it works fine, but the open airbrakes generate way too little drag, much less than a B9 procedural surface of the same size
