# PersistentStackTokens
A Risk of rain 2 mod that carries over (some) of your Bandit stack tokens from the desperado ability.

### What about [PersistentDesperado](https://thunderstore.io/package/OldFaithless/PersistentDesperado/)?
To be honest, I didn't even know about it until I preety much finished and since I was doing this to also
do some C#, I decided I'll do it anyways. The mods do differ in the balancing aspect, so its not just a duplicate.

### Config
Balance is addressed in form of a multiplier, which is applied at the end of each stage (stacking tokens * multiplier).
The multiplier works as one would expect it to (`0` - remove all tokens, `1` - keep all tokens, `between 0 and 1` - 
remove only some of the tokens, `above 1` - go nuts :) ).

The config should contain only one number which represents the multiplier. Suggested value: somewhere between `0.5` and `0.75`.
