# Space Shuttle System with realistic aerodynamics

My personal fork of SpaceODY's Space-Shuttle-System-Expanded with my own modifications. Intended for Realism Overhaul. Tested in 1.12.3

## All new releases should be considered craft- and save-breaking!

## Updated October 2024

|<img src="https://github.com/giuliodondi/Space-Shuttle-System-realistic-aerodynamics/blob/master/GameData/SPACE_SHUTTLE_SYSTEM/Screenshots/shuttle-system-plume.gif" width="400" /><br>Reworked Waterfall effects|<img src="https://github.com/giuliodondi/Space-Shuttle-System-realistic-aerodynamics/blob/master/GameData/SPACE_SHUTTLE_SYSTEM/Screenshots/shuttle-system-screen2.png" width="500" /><br>Texture overhaul and new liveries|
|:-:|:-:|

|<img src="https://github.com/giuliodondi/Space-Shuttle-System-realistic-aerodynamics/blob/master/GameData/SPACE_SHUTTLE_SYSTEM/Screenshots/shuttle-system-screen6.png" width="500" /><br>Reentry heating glow|<img src="https://github.com/giuliodondi/Space-Shuttle-System-realistic-aerodynamics/blob/master/GameData/SPACE_SHUTTLE_SYSTEM/Screenshots/shuttle-system-screen5.png" width="500" /><br>Custom aerodynamic model - working rudder airbrake|
|:-:|:-:|

|<img src="https://github.com/giuliodondi/Space-Shuttle-System-realistic-aerodynamics/blob/master/GameData/SPACE_SHUTTLE_SYSTEM/Screenshots/shuttle-system-screen7.png" width="500" /><br>Mirrored SRB decouplers|<img src="https://github.com/giuliodondi/Space-Shuttle-System-realistic-aerodynamics/blob/master/GameData/SPACE_SHUTTLE_SYSTEM/Screenshots/shuttle-system-srb-sep.gif" width="500" /><br>Fixed SRB separation motors|
|:-:|:-:|

|<img src="https://github.com/giuliodondi/Space-Shuttle-System-realistic-aerodynamics/blob/master/GameData/SPACE_SHUTTLE_SYSTEM/Screenshots/shuttle-system-screen3.png" width="500" /><br>Payload bay lighting|<img src="https://github.com/giuliodondi/Space-Shuttle-System-realistic-aerodynamics/blob/master/GameData/SPACE_SHUTTLE_SYSTEM/Screenshots/shuttle-system-screen8.png" width="500" /><br>Tailcone option|
|:-:|:-:|



<b>Special thanks to FlandreScarlet1 for modelling work on the cockpit and rudder</b>

## DEPENDENCIES (install the latest compatible version):
- ASET Consolidated Avionics Pack (for the cockpit props)
- B9 Parts Switch
- KSPWheel
- Community Resource Pack
- Realism Overhaul and all the dependencies
- Textures Unlimited
- TAC Life Support or Kerbalism, both are supported (Kerbalism is WIP)
- Waterfall
- [My personal fork of Ferram Aerospace Research for the custom aerodynamic model](https://github.com/giuliodondi/Ferram-Aerospace-Research-modded)

## INSTALLATION

- Install all the requirements
- **Delete any exsting Space Shuttle RO config in the folder GameData/RealismOverhaul/RO_SuggestedMods/SpaceShuttleSystem**
- **Check if you already have a GameData/SPACE_SHUTTLE_SYSTEM folder, if so delete it**
- Copy the contents of **GameData** in your KSP GameData, overwriting if asked
- You can ignore the **FARShuttleAerodynamicModel** folder, it's the source code for the custom aerodynamic module
- Every time in the future that you update RO, the Shuttle configs will be overwritten. You must restore them manually of the mod will be broken

## Assembling the Shuttle stack

### If you just updated the mod you will need to re-assemble from scratch the Shuttle in the SPH/VAB and save it anew
This is because some part modules like control surfaces get stored as part of the craft file and, as such, do not inherit the part modifications.

- If you use a joystick, I strongly advise to go to the KSP Control settings and wire wheel stering to the same axis you use for yaw control, else you might have a hard time tracking the runway on landing
- The root part is the pre-assembled Orbiter, of which you have a variant with cockpit gauges and one without
- The mod comes with its own version of the RS-25 and AJ10-190 engines, if you use RO engines these will have been overwritten
- The Orbiter and OMS pods have toggleable RCS covers. They do not impede the actual RCS if you leave them on, but they can't be removed in flight
- The Shuttle control surfaces come pre-configured to be compatible with my reentry script, if you use other scripts you might want to re-configure them yourself.
- The External Tank has placement nodes to snap the SRB decouplers in the right place
- Likewise, the decouplers have snap nodes for the SRBs themselves
- Take care to place the left/right SRB aft separation motors on the correct side
- The SRB forward cones should be facing the Orbiter side and be rotated just a little bit towards the Orbiter for correct separation
- Do not place launch clamps on the side oppsite the Orbiter, the stack will translate in that direction at liftoff and you may have a collision
- Set the fuel flow priority higher on the External Tank and lower on the Orbiter or you may lose the propellants for the fuel cells

## Known Issues
- The split rudder, when fully open, can induce yaw instability. Up to about 3/4 it's fine
- Cockpit internal pending rework
- Wings no longer have colliders - required by the aero model to work correctly

# History of changes

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
- To be able to maintain reasonable stability on landing and high-AoA on reentry, the Cl movement calculations as a function of Mach were brutally disabled. The model is now adjusted to require little flap trim on Entry in normal conditions and be within flap trim limits when very tail-heavy (like a TAL abort)
- **The Shuttle is meant to be slightly pitch-unstable when subsonic, please take a look at the Aerodynamic data book, section 4.1.1.3. Pitch trim and responsive, continuous control inputs are needed for a stable landing**
- I had to split the main landing gear bogey into two separate parts while keeping the nose gear as part of the Orbiter part, to get the deploy animations right  
- <s>The cabin hatch is no longer functional. Some animation names clash in the assembled parts, and something had to give</s>
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

**Textures and part modifications:**
- complete part rework with model fixed all around
- heat shield glow emissive effect
- SRB separation motors fixed
- custom ET textures
- New SRB texture variants for booster and nosecone, added lighter Filament-Wound booster variant 
- Cabin texture reworked, ejection seat panels for Enterprise and old Columbia, custom tile patterns around the side hatch, unique tiles on the forward RCS block
- Cargo bay and engine mount texture reworked
- Fictional Enterprise orbital version and Moonraker
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
