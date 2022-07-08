# Space Shuttle System Expanded

My personal fork of SpaceODY's Space-Shuttle-System-Expanded. Tested in 1.10

Includes some texture reworks (ET, SRB and elevons) and RO config tweaks.  
In addition there is a new cabin variant with an empty IVA cockpit, only the shell and no switches/gauges, a lot lighter on FPS when landing IVA.

# Increased crossrange branch (experimental

The *xrange_increase* branch gives a Ferram lifting surface config to the Cargo Bay lift to boost reentry crossrange.  
Two downsides:
- The extra crossrange requires high L/D, higher than I was able to achieve even with these configs. I have found good results when flying at 20° AoA, where L/D is highest at hypersonic speeds.  
This is unrealistic as the real Shuttle would have probably violated some thermal constraint at such a low AoA, but on the other hand the real Shuttle was able to generate high L/D at much higher AoA.
- The pitch stability margin is MUCH narrower, it tends to be nose-heavy at hypersonic speeds and tail-heavy (unstable) at low speeds. Using RCS as ballast or the body flap as a negative-AoA control surface helps somewhat

# Split airbrake branch (experimental)

The *splitbrake* branch contains a new part I made in Blender from the original DECQ rudder.
It is an individual rudder panel, a pair of these should be surface-attched to the Shuttle tailfin. Each has their own Ferram configuration so they can act as spoilers individually.  
Functionally it works fine, but the open airbrakes generate way too little drag, much less than a B9 procedural surface of the same size
