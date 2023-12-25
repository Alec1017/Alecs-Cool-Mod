using AlecsCoolMod.Patches;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AlecsCoolMod
{
    [BepInPlugin(modGUID, modName, modVersion)]
    public class TestModBase : BaseUnityPlugin
    {
        // define some constant values for the mod
        private const string modGUID = "AlecsCoolMod";
        private const string modName = "Alecs Cool Mod";
        private const string modVersion = "1.0.2";

        // Define the main asset bundle
        public static AssetBundle MainAssets;

        // create a harmony instance
        private static readonly Harmony harmony = new Harmony(modGUID);

        // creates a static reference to this mod
        public static TestModBase Instance;

        // create a logging source
        internal ManualLogSource logger;

        private void Awake()
        {
            // Set the static reference to the instance
            if (Instance == null) Instance = this;

            // instantiate the logging source
            logger = BepInEx.Logging.Logger.CreateLogSource(modGUID);

            // Load in the asset bundle
            MainAssets = AssetBundle.LoadFromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Assets/assetbundle"));

            // Netcode patcher stuff
            NetcodePatcher();

            // log that the mod has installed properly
            logger.LogInfo("Alec's cool mod mod has awakened");
        }

        private void Start()
        {
            // Initiate all patches
            harmony.PatchAll(typeof(NetworkManagerPatch));
            harmony.PatchAll(typeof(PlayerControllerBPatch));
            harmony.PatchAll(typeof(AudioPatch));

            // Subscribe to the scene loaded event
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDestroy()
        {
            // Unsubscribe from the scene loaded event
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            // Need an active instance of the test mod first
            if (Instance == null)
                return;

            // When a scene is loaded that contains a network manager, make sure all players
            // are using the same unity random seed
            if (NetworkHandler.Instance)
            {
                // generate a unique seed
                int newSeed = (int)DateTime.Now.Ticks;

                // send the seed to the clients
                NetworkHandler.Instance.SendSeedServerRpc(newSeed);
            }
        }

        // Netcode Patcher
        private static void NetcodePatcher()
        {
            var types = Assembly.GetExecutingAssembly().GetTypes();
            foreach (var type in types)
            {
                var methods = type.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
                foreach (var method in methods)
                {
                    var attributes = method.GetCustomAttributes(typeof(RuntimeInitializeOnLoadMethodAttribute), false);
                    if (attributes.Length > 0)
                    {
                        method.Invoke(null, null);
                    }
                }
            }
        }
    }
}
