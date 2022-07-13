# Space Shuttle System Expanded

My personal fork of SpaceODY's Space-Shuttle-System-Expanded. Tested in 1.10

Includes some texture reworks (ET, SRB and elevons) and RO config tweaks.  
In addition there is a new cabin variant with an empty IVA cockpit, only the shell and no switches/gauges, a lot lighter on FPS when landing IVA.

# Increased crossrange branch (experimental)

The *xrange_increase* branch gives a Ferram lifting surface config to the Cargo Bay lift to increase hypersonic L/D and boost reentry crossrange.  
Caveats:
- The extra crossrange only comes into play at low AoA (20°) where L/D is higher at hypersonic speeds. This is unrealistic as the real Shuttle would have probably violated some thermal constraint at such a low AoA.
- At subsonic speeds, the Shuttle glides way better than it should
- It has pitch stability issues. At high Mach and high AoA it has a strong pitch-down tendency which maxes out the flap trimming mechanism in my entry script, although it's still well within the RCS pitch authority. At low Mach the centre of lift apparently shifts forwards and it becomes pitch-unstable.  
At low speeds the Ferram AoA control feedback functionality is critical to maintain control, with a positive setting on elevon and body flap so that they will always move the nose towards the prograde direction. But it must not be enabled durign hypersonic entry or the Shuttle won't be able to maintain high AoA. My entry script now automatically takes care of this during approach guidance.



# Split airbrake branch (experimental)

The *splitbrake* branch contains a new part I made in Blender from the original DECQ rudder.
It is an individual rudder panel, a pair of these should be surface-attched to the Shuttle tailfin. Each has their own Ferram configuration so they can act as spoilers individually.  
Functionally it works fine, but the open airbrakes generate way too little drag, much less than a B9 procedural surface of the same size
