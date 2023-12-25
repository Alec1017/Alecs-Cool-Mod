using HarmonyLib;
using UnityEngine;
using Unity.Netcode;

namespace AlecsCoolMod.Patches
{
    [HarmonyPatch]
    internal class NetworkManagerPatch
    {
        // The game object network prefab
        public static GameObject networkPrefab;
   
        // Handles loading the network handler prefab and registering it with the network manager
        [HarmonyPostfix, HarmonyPatch(typeof(GameNetworkManager), "Start")]
        static void LoadNetworkHandlerPrefab()
        {   
            // Skip if the network prefab is already loaded
            if (networkPrefab != null)
                return;

            // Load the network handler prefab
            networkPrefab = TestModBase.MainAssets.LoadAsset<GameObject>("NetworkHandler.prefab");
            
            // Add the network handler component to the prefab
            networkPrefab.AddComponent<NetworkHandler>();

            // After the prefab receives the NetworkHandler component, its ready to be
            // given to the network manager
            NetworkManager.Singleton.AddNetworkPrefab(networkPrefab);
        }
        
        // Spawns the network handler into the game
        [HarmonyPostfix, HarmonyPatch(typeof(StartOfRound), "Awake")]
        static void SpawnNetworkHandler()
        {
            // Only the host is allowed to spawn the network object
            if (NetworkManager.Singleton.IsHost || NetworkManager.Singleton.IsServer)
            {
                // Instantiate the network prefab
                GameObject NetworkHandlerHost = Object.Instantiate(networkPrefab, Vector3.zero, Quaternion.identity);

                // Spawn the object
                NetworkHandlerHost.GetComponent<NetworkObject>().Spawn();
            }
        }
    }
}
