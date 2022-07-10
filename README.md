# Space Shuttle System Expanded

My personal fork of SpaceODY's Space-Shuttle-System-Expanded. Tested in 1.10

Includes some texture reworks (ET, SRB and elevons) and RO config tweaks.  
In addition there is a new cabin variant with an empty IVA cockpit, only the shell and no switches/gauges, a lot lighter on FPS when landing IVA.

# Increased crossrange branch (experimental)

The *xrange_increase* branch gives a Ferram lifting surface config to the Cargo Bay lift to increase hypersonic L/D and boost reentry crossrange.  
Downsides:
- The extra crossrange benefit is really highest at low AoA (20°) where L/D is highest at hypersonic speeds. This is unrealistic as the real Shuttle would have probably violated some thermal constraint at such a low AoA.
- At subsonic speeds, the Shuttle becomes a bit of an infini-glider
- It has pitch stability issues. At hypersonic speeds it's far too nose-heavy, requiring nearly full upwards flap trim commanded by my Entry script. At Mach <2 the centre of lift moves forward and it becomes unstable, requiring flap trim down to maintain stability.


# Split airbrake branch (experimental)

The *splitbrake* branch contains a new part I made in Blender from the original DECQ rudder.
It is an individual rudder panel, a pair of these should be surface-attched to the Shuttle tailfin. Each has their own Ferram configuration so they can act as spoilers individually.  
Functionally it works fine, but the open airbrakes generate way too little drag, much less than a B9 procedural surface of the same size
