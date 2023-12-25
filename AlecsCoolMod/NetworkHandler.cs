using System;
using Unity.Netcode;

namespace AlecsCoolMod
{
    // Houses the RPCs, Network Variables, and any other methods allowing
    // information to be passed over the network
    internal class NetworkHandler : NetworkBehaviour
    {
        // Singleton instance which has public getters
        public static NetworkHandler Instance { get; private set; }

        // create an event for when the seed changes
        public static event Action SeedChanged;

        public override void OnNetworkSpawn()
        {
            // Make sure the event is null when the network is spawned, so that an 
            // event cant be subscribed to multiple times.
            SeedChanged = null;

            // Remove any previously existing game object, if it exists. This can only be
            // done by the host
            if (Instance && (NetworkManager.Singleton.IsHost || NetworkManager.Singleton.IsServer))
            {
                Instance.gameObject.GetComponent<NetworkObject>().Despawn();
            }

            // Set the instance
            Instance = this;
        }

        // Receives a seed value from the server
        [ClientRpc]
        public void ReceiveSeedClientRpc(int seed)
        {
            // Initialize the seed for the same randomness between clients
            UnityEngine.Random.InitState(seed);

            // If the event has subscribers, then invoke it
            SeedChanged?.Invoke();
        }

        // Sends a seed value to the server
        [ServerRpc(RequireOwnership = false)]
        public void SendSeedServerRpc(int seed)
        {
            // Dont call a server RPC method if not the host
            if (!IsHost)
                return;

            // Send the seed value to the client RPC method
            ReceiveSeedClientRpc(seed);
        }
    }
}
