using HarmonyLib;
using LCSoundTool;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace AlecsCoolMod.Patches
{
    internal class AudioPatch
    {
        // Whether the audio patch has been initialized already
        private static bool initialized;

        // list of sound file names for the company intro speech
        private static List<string> soundFiles;

        // list of audio clips created from sound file names
        private static List<AudioClip> companySpeeches;

        // create custom sounds for random effects
        public static AudioClip customFallSound;
        public static AudioClip customTurretSound;

        /////////////////////////////////////////////////////////////////////////////////
        //                                 Patches                                     //
        /////////////////////////////////////////////////////////////////////////////////

        // Load the audio at the start of a round
        [HarmonyPrefix, HarmonyPatch(typeof(StartOfRound), "Awake")]
        static void LoadCustomAudio()
        {
            if (!initialized)
            {
                // Load in the company speeches
                LoadCustomSpeech();

                // Load in the custom sound effects
                LoadCustomSoundEffects();

                // Mark the patch as initialized
                initialized = true;
            }
        }

        // Syncs a random seed with all clients when the first day animation is started.
        // This allows all players to hear a randomized, but synced, audio clip
        [HarmonyPostfix, HarmonyPatch(typeof(StartOfRound), "PlayFirstDayShipAnimation")]
        static void ResetSeedOnFirstDayAnimation()
        {
            // Make sure an instance of the network handler exists
            if (NetworkHandler.Instance)
            {
                // generate a unique seed
                int newSeed = (int)DateTime.Now.Ticks;

                // send the seed to the clients
                NetworkHandler.Instance.SendSeedServerRpc(newSeed);
            }
        }

        /////////////////////////////////////////////////////////////////////////////////
        //                             Helper Functions                                //
        /////////////////////////////////////////////////////////////////////////////////

        // Load in the company speeches at the start of the round
        static void LoadCustomSpeech()
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

            // initialize all the possible company intro speeches
            companySpeeches = soundFiles.ConvertAll(fileName => {
                return SoundTool.GetAudioClip("Alec1017-AlecsCoolMod", fileName);
            });

            // Replace the intro company speech with sounds that have an equal chance of playing
            foreach (var clip in companySpeeches)
            {
                SoundTool.ReplaceAudioClip("IntroCompanySpeech", clip, 1f / companySpeeches.Count);
            }
        }

        // Load in the audio effects at the start of the round
        static void LoadCustomSoundEffects()
        {
            // get the custom audio clips for sound effects
            customFallSound = SoundTool.GetAudioClip("Alec1017-AlecsCoolMod", "wilhelm_scream.wav");
            customTurretSound = SoundTool.GetAudioClip("Alec1017-AlecsCoolMod", "surprise.mp3");

            // replace the standard sound effects
            SoundTool.ReplaceAudioClip("DieFromFallDamageSFX1", customFallSound);
            SoundTool.ReplaceAudioClip("TurretSeePlayer", customTurretSound);
        }
    }
}
