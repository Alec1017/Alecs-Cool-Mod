using HarmonyLib;
using LCSoundTool;
using System.Collections.Generic;
using UnityEngine;

namespace AlecsCoolMod.Patches
{
    [HarmonyPatch(typeof(StartOfRound))]
    internal class CompanySpeechPatch
    {
        // list of sound file names for the company intro speech
        private static List<string> soundFiles;

        // list of audio clips created from sound file names
        private static List<AudioClip> companySpeeches;

        // Load in the company speeches
        [HarmonyPrefix, HarmonyPatch(typeof(StartOfRound), "Start")]
        public static void LoadCustomCompanySpeeches()
        {
            if (soundFiles.Count == 0)
            {
                // initialize all sound files
                soundFiles = new List<string>
                {
                    "just_do_it.mp3",
                    "boys_are_back.wav",
                    "jg_wentworth.mp3"
                };

                // initialize all the possible intro speeches
                companySpeeches = soundFiles.ConvertAll<AudioClip>(fileName => {
                    return SoundTool.GetAudioClip("Alec1017-AlecsCoolMod", fileName);
                });
            }
        }

        // choose a random company speech at on the first day
        [HarmonyPrefix, HarmonyPatch(typeof(StartOfRound), "PlayFirstDayShipAnimation")]
        public static void SelectRandomCompanySpeech()
        {
            foreach(var clip in companySpeeches)
            {
                SoundTool.ReplaceAudioClip("IntroCompanySpeech", clip, 1f / companySpeeches.Count);
            }

        }
    }
}
