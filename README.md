# Space Shuttle System Expanded

My personal fork of SpaceODY's Space-Shuttle-System-Expanded with my own modifications. Tested in 1.12.3

## DEPENDENCIES (install the latest compatible version):
- ASET Consolidated Avionics Pack (for the cockpit props)
- B9 Parts Switch
- KSPWheel
- Community Resource Pack
- Textures Unlimited
- TAC Life Support or Kerbalism, both are supported (Kerbalism is WIP)
- [My personal fork of Ferram Aerospace Research for the custom aerodynamic model](https://github.com/giuliodondi/Ferram-Aerospace-Research-modded)

I also advise you to take care of the following:
- If you use any of the experimental tail control surface parts, you **MUST** go to your KSP settings and wire up the wheel steering to the same controls you use for rudder, otherwise you **WILL** careen off the runway on landing.


## Realistic Aerodynamics (experimental)

### I seem to have found a way to give the Space Shuttle realistic aerodynamics, at long last.

The mod now comes with a single 'Orbiter' part, which is the assembly of the cabin, fuselage, engine mount and both wings.  
Having a single part that comprises most of the orbiter allows me to slap a custom module that implements the realistic Shuttle lift and drag coefficients [taken from the NASA technical documents](https://archive.org/details/nasa_techdoc_19810067693).

The module config is exactly the same as the Ferram wing aerodynamic module, as it must inherid directly from that module class to work in Ferram:

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

## Other changes:

**Tail / Rudder (Experimental)**
I created a new part which is the assembly of the tail and the split rudder. This should now be a functional split airbrake part, with two panel transforms that can be intependently controlled.
To achieve this I had to re-arrange the part transforms in Unity and also remove the Ferram configs for the control surface, since only one can be put in a .cfg file at one time and it controls only one transform at a time.  
The rudder uses two stock control surface modules for now

**Split rudder panel  (Experimental)**
This is another new part, which is just one of the panels that make up the Shuttle rudder control surface. You can place manually two of these on the standard tail fin and control the mindependently as Ferram control ssurfaces. This is not as draggy as I'd like

**Textures and part variants:**
- custom ET textures
- New SRB texture variants for booster and nosecone, added lighter Filament-Wound booster variant 
- Cabin texture reworked, ejection seat panels for Enterprise and old Columbia, custom tile patterns around the side hatch, unique tiles on the forward RCS block
- Fictional Enterprise orbital version
- New tail texture for Challenger and Columbia SILTS pod
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
- CL adjustments for the cargo bay 
- control surface surface Ferram control settings come pre-configured for my Shuttle Entry script

---
