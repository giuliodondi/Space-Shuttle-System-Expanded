# Space Shuttle System Expanded

My personal fork of SpaceODY's Space-Shuttle-System-Expanded with my own modifications. Tested in 1.12.3

## DEPENDENCIES (install the latest compatible version):
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



# Increased crossrange branch (experimental, not recommended)

The *xrange_increase* branch gives a Ferram lifting surface config to the Cargo Bay lift to increase hypersonic L/D and boost reentry crossrange.  
Caveats:
- The extra crossrange only comes into play at low AoA (20°) where L/D is higher at hypersonic speeds. This is unrealistic as the real Shuttle would have probably violated some thermal constraint at such a low AoA.
- At subsonic speeds, the Shuttle glides way better than it should
- It has pitch stability issues. At high Mach and high AoA it has a strong pitch-down tendency which maxes out the flap trimming mechanism in my entry script, although it's still well within the RCS pitch authority. At low Mach the centre of lift apparently shifts forwards and it becomes pitch-unstable.  
At low speeds the Ferram AoA control feedback functionality is critical to maintain control, with a positive setting on elevon and body flap so that they will always move the nose towards the prograde direction. But it must not be enabled during hypersonic entry or the Shuttle won't be able to maintain high AoA.  
~~My entry script now automatically takes care of this during approach guidance.~~ It's very unreliable and required too many ad-hoc modifications than I liked.



# Split airbrake branch (experimental)

The *splitbrake* branch contains a new part I made in Blender from the original DECQ rudder.
It is an individual rudder panel, a pair of these should be surface-attched to the Shuttle tailfin. Each has their own Ferram configuration so they can act as spoilers individually.  
Functionally it works fine, but the open airbrakes generate way too little drag, much less than a B9 procedural surface of the same size
