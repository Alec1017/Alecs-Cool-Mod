using GameNetcodeStuff;
using HarmonyLib;

namespace AlecsCoolMod.Patches
{
    internal class PlayerControllerBPatch
    {

        // patches the Update() function which is called every frame.
        [HarmonyPostfix, HarmonyPatch(typeof(PlayerControllerB), "Update")]
        static void InfiniteSprintPatch(ref float ___sprintMeter)
        {   
            // set the sprint meter to 1 always
            ___sprintMeter = 1f;
        }
    }
}
