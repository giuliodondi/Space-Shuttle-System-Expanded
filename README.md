# Space Shuttle System Expanded

My personal fork of SpaceODY's Space-Shuttle-System-Expanded. Tested in 1.10

Includes some texture reworks (ET, SRB and elevons) and RO config tweaks.  
In addition there is a new cabin variant with an empty IVA cockpit, only the shell and no switches/gauges, a lot lighter on FPS when landing IVA.


The xrange_increase branch (**which is EXPERIMENTAL**) gives a great boost to the Cargo Bay lift to increase crossrange.  
Two downsides:
- The extra crossrange requires high L/D, higher than I was able to achieve even with these configs. I have found good results when flying at 20° AoA, where L/D is highest at hypersonic speeds.  
This is unrealistic as the real Shuttle would have probably violated some thermal constraint at such a low AoA, but on the other hand the real Shuttle was able to generate high L/D at much higher AoA.
- The pitch stability margin is MUCH narrower, it tends to be nose-heavy at hypersonic speeds and tail-heavy (unstable) at low speeds. Using RCS as ballast or the body flap as a negative-AoA control surface helps somewhat
