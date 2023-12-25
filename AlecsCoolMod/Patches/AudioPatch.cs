using HarmonyLib;
using LCSoundTool;
using System.Collections.Generic;
using UnityEngine;

namespace AlecsCoolMod.Patches
{
    internal class AudioPatch
    {
        // list of sound file names for the company intro speech
        private static List<string> soundFiles;

        // list of audio clips created from sound file names
        private static List<AudioClip> companySpeeches;

        // create custom sounds for random effects
        public static AudioClip customFallSound;
        public static AudioClip customTurretSound;

        // Load in the company speeches at the start of the round
        [HarmonyPrefix, HarmonyPatch(typeof(StartOfRound), "Awake")]
        static void LoadCustomCompanySpeeches()
        {
            if (soundFiles == null)
            {
                // initialize company speech sound files
                soundFiles = new List<string>
                {
                    "just_do_it.mp3",
                    "boys_are_back.wav",
                    "jg_wentworth.mp3",
                    "great_asset.mp3",
                    "little_einsteins.mp3"
                };

                // get the custom audio clips for sound effects
                customFallSound = SoundTool.GetAudioClip("Alec1017-AlecsCoolMod", "wilhelm_scream.wav");
                customTurretSound = SoundTool.GetAudioClip("Alec1017-AlecsCoolMod", "surprise.mp3");

                // initialize all the possible company intro speeches
                companySpeeches = soundFiles.ConvertAll(fileName => {
                    return SoundTool.GetAudioClip("Alec1017-AlecsCoolMod", fileName);
                });

                // Replace the intro company speech with sounds that have an equal chance of playing
                foreach (var clip in companySpeeches)
                {
                    SoundTool.ReplaceAudioClip("IntroCompanySpeech", clip, 1f / companySpeeches.Count);
                }

                // replace the standard sound effects
                SoundTool.ReplaceAudioClip("DieFromFallDamageSFX1", customFallSound);
                SoundTool.ReplaceAudioClip("TurretSeePlayer", customTurretSound);
            }
        }
    }
}
