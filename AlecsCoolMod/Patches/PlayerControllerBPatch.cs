using GameNetcodeStuff;
using HarmonyLib;

namespace AlecsCoolMod.Patches
{
    // Define the target of what is going to be patched
    // can also use HarmonyPatch(nameof(PlayerControllerB.Update)) if the 
    // method was public, but it is private so use a string instead.
    [HarmonyPatch("Update")]
    [HarmonyPatch(typeof(PlayerControllerB))]
    internal class PlayerControllerBPatch
    {

        // patches the Update() function which is called every frame.
        // This is a post-fix, so it happens after the update() function.
        //
        // ___sprintMeter is a reference. 
        [HarmonyPostfix]
        static void InfiniteSprintPatch(ref float ___sprintMeter)
        {   
            // set the sprint meter to 1
            ___sprintMeter = 1f;
        }
    }
}
