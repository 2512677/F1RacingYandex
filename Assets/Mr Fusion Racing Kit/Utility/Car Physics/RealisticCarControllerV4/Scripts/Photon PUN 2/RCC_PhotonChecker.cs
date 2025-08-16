using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;  // Make sure you have 'Photon.Pun' included

public class RCC_PhotonChecker : MonoBehaviour {

    // The name of your Main Menu scene. Make sure it matches the exact name in your Build Settings.
    public string lobbySceneName = "RCC_Lobby_Photon_PUN2";

    private void Start() {

        // Check if we are connected to Photon
        if (!PhotonNetwork.IsConnected) {

            // If not connected, load the Main Menu scene
            SceneManager.LoadScene(lobbySceneName);

        }

    }

}
