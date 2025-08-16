//----------------------------------------------
//            Realistic Car Controller
//
// Copyright © 2014 - 2024 BoneCracker Games
// https://www.bonecrackergames.com
// https://www.bonecrackergames.com
// Ekrem Bugra Ozdoganlar
//
//----------------------------------------------

#if RCC_PHOTON && PHOTON_UNITY_NETWORKING

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

/// <summary>
/// Manages Photon connections, room creation/joining, and UI updates for the RCC (Realistic Car Controller).
/// </summary>
public class RCC_PhotonManager : MonoBehaviourPunCallbacks {

    /// <summary>
    /// Singleton instance of the RCC_PhotonManager.
    /// </summary>
    public static RCC_PhotonManager Instance;

    /// <summary>
    /// Name of the gameplay scene to load after joining/creating a room.
    /// </summary>
    public string gameplaySceneName = "Gameplay Scene Name";

    [Header("UI InputFields")]
    /// <summary>
    /// Input field for nickname in the UI.
    /// </summary>
    public TMP_InputField nickPanel;

    [Header("UI Menus")]
    /// <summary>
    /// Panel for browsing available rooms.
    /// </summary>
    public GameObject browseRoomsPanel;
    /// <summary>
    /// Content parent transform where individual room entries are instantiated.
    /// </summary>
    public GameObject roomsContent;
    /// <summary>
    /// Panel that holds the chat messages in the room.
    /// </summary>
    public GameObject chatLinesPanel;
    /// <summary>
    /// Content parent transform where chat lines are instantiated.
    /// </summary>
    public GameObject chatLinesContent;
    /// <summary>
    /// UI element shown when there are no available rooms yet.
    /// </summary>
    public GameObject noRoomsYet;
    /// <summary>
    /// The button to connect to the Photon server.
    /// </summary>
    public GameObject connectButton;
    /// <summary>
    /// The button to create a new room.
    /// </summary>
    public GameObject createRoomButton;
    /// <summary>
    /// The button to exit a currently joined room.
    /// </summary>
    public GameObject exitRoomButton;
    /// <summary>
    /// Text object for the main title in the UI.
    /// </summary>
    public GameObject titleText;

    [Header("UI Texts")]
    /// <summary>
    /// UI text displaying the current Photon connection status.
    /// </summary>
    public TextMeshProUGUI status;
    /// <summary>
    /// UI text displaying the total number of online players.
    /// </summary>
    public TextMeshProUGUI totalOnlinePlayers;
    /// <summary>
    /// UI text displaying the total number of rooms.
    /// </summary>
    public TextMeshProUGUI totalRooms;
    /// <summary>
    /// UI text displaying the current Photon region.
    /// </summary>
    public TextMeshProUGUI region;

    [Header("UI Prefabs")]
    /// <summary>
    /// Prefab used for listing rooms in the UI.
    /// </summary>
    public RCC_PhotonUIRoom roomPrefab;
    /// <summary>
    /// Prefab used for chat messages in the UI.
    /// </summary>
    public RCC_PhotonUIChatLine chatLinePrefab;

    //  Dictionaries for cached rooms and players.
    /// <summary>
    /// Dictionary storing all cached RoomInfo objects keyed by room name.
    /// </summary>
    private Dictionary<string, RoomInfo> cachedRoomList;
    /// <summary>
    /// Dictionary storing UI entries for each room, keyed by room name.
    /// </summary>
    private Dictionary<string, GameObject> roomListEntries;

    /// <summary>
    /// Initializes singleton, ensures only one instance, and prevents destruction on scene load.
    /// </summary>
    private void Awake() {

        if (Instance != null) {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

    }

    /// <summary>
    /// Called once at the start. Initializes UI states, dictionary structures, and sets a random nickname if not set.
    /// </summary>
    private void Start() {

        browseRoomsPanel.SetActive(false);
        createRoomButton.SetActive(false);
        connectButton.SetActive(true);
        exitRoomButton.SetActive(false);
        nickPanel.gameObject.SetActive(true);
        chatLinesPanel.SetActive(false);
        titleText.SetActive(true);

        //  Initializing dictionaries.
        cachedRoomList = new Dictionary<string, RoomInfo>();
        roomListEntries = new Dictionary<string, GameObject>();

        Debug.Log("Ready to connect");
        status.text = "Ready to connect";

        nickPanel.SetTextWithoutNotify("RCC Player " + Random.Range(0, 99999).ToString());

    }

    /// <summary>
    /// Registers the manager as a Photon callback target when enabled.
    /// </summary>
    public override void OnEnable() {
        PhotonNetwork.AddCallbackTarget(this);
    }

    /// <summary>
    /// Removes the manager from Photon callback targets when disabled.
    /// </summary>
    public override void OnDisable() {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    /// <summary>
    /// Unity's Update method, updates UI text for online players/rooms/region if connected.
    /// </summary>
    private void Update() {

        if (PhotonNetwork.IsConnected && PhotonNetwork.InLobby) {
            totalOnlinePlayers.text = "Total Online Players: " + PhotonNetwork.CountOfPlayers.ToString();
            totalRooms.text = "Total Online Rooms: " + PhotonNetwork.CountOfRooms.ToString();
            region.text = "Region: " + PhotonNetwork.CloudRegion.ToString();
        } else {
            totalOnlinePlayers.text = "";
            totalRooms.text = "";
            region.text = "";
        }

    }

    /// <summary>
    /// Called by the UI. Initiates connection process if not connected, or hides the nickname panel if already connected.
    /// </summary>
    public void Connect() {

        if (!PhotonNetwork.IsConnected)
            ConnectToServer();
        else
            nickPanel.gameObject.SetActive(false);

    }

    /// <summary>
    /// Establishes the connection to the Photon server using settings, sets sync rates, and updates UI.
    /// </summary>
    private void ConnectToServer() {

        Debug.Log("Connecting to server");
        status.text = "Connecting to server";

        if (string.IsNullOrWhiteSpace(nickPanel.text))
            PhotonNetwork.NickName = "Player_" + Random.Range(0, 999);
        else
            PhotonNetwork.NickName = nickPanel.text;

        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.SendRate = 40;
        PhotonNetwork.SerializationRate = 40;

        PhotonNetwork.ConnectUsingSettings();

        browseRoomsPanel.SetActive(false);
        createRoomButton.SetActive(false);
        connectButton.SetActive(false);
        exitRoomButton.SetActive(false);
        nickPanel.gameObject.SetActive(false);
        chatLinesPanel.SetActive(false);
        titleText.SetActive(true);

        if (RCC_InfoLabel.Instance)
            RCC_InfoLabel.Instance.ShowInfo("Connecting to server");

    }

    /// <summary>
    /// Callback from Photon when successfully connected to Master Server.
    /// Joins the lobby automatically.
    /// </summary>
    public override void OnConnectedToMaster() {

        Debug.Log("Connected to server, entering to lobby");
        status.text = "Connected to server, entering to lobby";

        PhotonNetwork.JoinLobby();

        browseRoomsPanel.SetActive(false);
        createRoomButton.SetActive(false);
        connectButton.SetActive(false);
        exitRoomButton.SetActive(false);
        nickPanel.gameObject.SetActive(false);
        chatLinesPanel.SetActive(false);
        titleText.SetActive(true);

        if (RCC_InfoLabel.Instance)
            RCC_InfoLabel.Instance.ShowInfo("Connected to server, entering to lobby");

    }

    /// <summary>
    /// Callback from Photon when joined a lobby.
    /// </summary>
    public override void OnJoinedLobby() {

        Debug.Log("Entered to lobby");
        status.text = "Entered to lobby";

        browseRoomsPanel.SetActive(true);
        createRoomButton.SetActive(true);
        connectButton.SetActive(false);
        exitRoomButton.SetActive(false);
        nickPanel.gameObject.SetActive(false);
        chatLinesPanel.SetActive(false);
        titleText.SetActive(true);

        if (RCC_InfoLabel.Instance)
            RCC_InfoLabel.Instance.ShowInfo("Entered to lobby");

    }

    /// <summary>
    /// Callback from Photon when successfully joined a room.
    /// Updates the UI to show chat panel and exit button.
    /// </summary>
    public override void OnJoinedRoom() {

        Debug.Log("Joined room");
        status.text = "Joined room";

        nickPanel.gameObject.SetActive(false);
        browseRoomsPanel.SetActive(false);
        createRoomButton.SetActive(false);
        connectButton.SetActive(false);
        exitRoomButton.SetActive(true);
        chatLinesPanel.SetActive(true);
        titleText.SetActive(false);

        if (RCC_InfoLabel.Instance)
            RCC_InfoLabel.Instance.ShowInfo("Joined room\nYou can spawn your vehicle from the 'Options' menu");

    }

    /// <summary>
    /// Callback from Photon when successfully created a room.
    /// Loads the gameplay scene.
    /// </summary>
    public override void OnCreatedRoom() {

        Debug.Log("Created room");
        status.text = "Created room";

        nickPanel.gameObject.SetActive(false);
        browseRoomsPanel.SetActive(false);
        createRoomButton.SetActive(false);
        connectButton.SetActive(false);
        exitRoomButton.SetActive(true);
        chatLinesPanel.SetActive(true);
        titleText.SetActive(false);

        if (RCC_InfoLabel.Instance)
            RCC_InfoLabel.Instance.ShowInfo("Created room\nYou can spawn your vehicle from the 'Options' menu");

        LoadLevel(gameplaySceneName);

    }

    /// <summary>
    /// Called by Photon when the room list is updated.
    /// Updates the cachedRoomList, then refreshes the UI.
    /// </summary>
    /// <param name="roomList">List of current rooms.</param>
    public override void OnRoomListUpdate(List<RoomInfo> roomList) {

        ClearRoomListView();
        UpdateCachedRoomList(roomList);
        UpdateRoomListView(roomList);

        if (roomListEntries != null && roomListEntries.Count > 0)
            noRoomsYet.SetActive(false);
        else
            noRoomsYet.SetActive(true);

    }

    /// <summary>
    /// Updates the dictionary of cached rooms based on the latest room list from the lobby.
    /// Removes rooms that are closed, invisible, or removed.
    /// </summary>
    /// <param name="roomList">Current list of rooms retrieved from Photon.</param>
    private void UpdateCachedRoomList(List<RoomInfo> roomList) {

        foreach (RoomInfo info in roomList) {

            // Remove room from cached room list if it got closed, became invisible or was marked as removed
            if (!info.IsOpen || !info.IsVisible || info.RemovedFromList) {

                if (cachedRoomList.ContainsKey(info.Name))
                    cachedRoomList.Remove(info.Name);

                continue;
            }

            // Update cached room info
            if (cachedRoomList.ContainsKey(info.Name))
                cachedRoomList[info.Name] = info;
            else
                cachedRoomList.Add(info.Name, info);

        }

    }

    /// <summary>
    /// Instantiates UI room entries under roomsContent for each room in the cachedRoomList.
    /// </summary>
    /// <param name="roomList">Current list of rooms from Photon (not used here directly).</param>
    private void UpdateRoomListView(List<RoomInfo> roomList) {

        foreach (RoomInfo info in cachedRoomList.Values) {

            GameObject entry = Instantiate(roomPrefab.gameObject);
            entry.transform.SetParent(roomsContent.transform);
            entry.transform.localScale = Vector3.one;
            entry.GetComponent<RCC_PhotonUIRoom>().Check(info.Name, info.PlayerCount.ToString() + " / " + info.MaxPlayers.ToString());
            roomListEntries.Add(info.Name, entry);

        }

    }

    /// <summary>
    /// Destroys all existing room entries in the UI and clears the roomListEntries dictionary.
    /// </summary>
    private void ClearRoomListView() {

        foreach (GameObject entry in roomListEntries.Values)
            Destroy(entry.gameObject);

        roomListEntries.Clear();

    }

    /// <summary>
    /// Called by the UI to create a new room or join if it already exists, with a limit of 8 players.
    /// </summary>
    public void CreateRoom() {

        Debug.Log("Creating room");
        status.text = "Creating room";

        nickPanel.gameObject.SetActive(false);
        browseRoomsPanel.SetActive(false);
        createRoomButton.SetActive(false);
        connectButton.SetActive(false);
        exitRoomButton.SetActive(false);
        chatLinesPanel.SetActive(false);
        titleText.SetActive(false);

        RoomOptions roomOptions = new RoomOptions();
        roomOptions.IsOpen = true;
        roomOptions.IsVisible = true;
        roomOptions.MaxPlayers = 8;

        PhotonNetwork.JoinOrCreateRoom("New RCC Room " + Random.Range(0, 999), roomOptions, TypedLobby.Default);

    }

    /// <summary>
    /// Called by the UI to join a room selected from the UI list.
    /// </summary>
    /// <param name="room">Reference to the RCC_PhotonUIRoom UI element.</param>
    public void JoinSelectedRoom(RCC_PhotonUIRoom room) {

        Debug.Log("Joining room");
        status.text = "Joining room";

        PhotonNetwork.JoinRoom(room.roomName.text);

    }

    /// <summary>
    /// Sends a chat message across all clients in the room using an RPC.
    /// </summary>
    /// <param name="inputField">The input field containing the chat message.</param>
    public void Chat(TMP_InputField inputField) {

        photonView.RPC("RPCChat", RpcTarget.AllBuffered, PhotonNetwork.NickName, inputField.text);

    }

    /// <summary>
    /// RPC method for receiving chat messages from all clients. Instantiates a new chat line in the UI.
    /// </summary>
    /// <param name="nickName">Nickname of the sender.</param>
    /// <param name="text">Chat message text.</param>
    [PunRPC]
    public void RPCChat(string nickName, string text) {

        RCC_PhotonUIChatLine newChatLine = Instantiate(chatLinePrefab.gameObject, chatLinesContent.transform).GetComponent<RCC_PhotonUIChatLine>();
        newChatLine.Line(nickName + " : " + text);

        RCC_PhotonUIChatLine[] chatLines = chatLinesContent.GetComponentsInChildren<RCC_PhotonUIChatLine>();

        if (chatLines.Length > 7)
            Destroy(chatLines[0].gameObject);

    }

    /// <summary>
    /// Called by the UI to leave the current room if the player is in one.
    /// </summary>
    public void ExitRoom() {

        if (PhotonNetwork.InRoom) {

            Debug.Log("Leaving the room");
            status.text = "Leaving the room";

            PhotonNetwork.LeaveRoom();

            if (RCC_InfoLabel.Instance)
                RCC_InfoLabel.Instance.ShowInfo("Leaving the room");

        }

    }

    /// <summary>
    /// Callback from Photon when the local player has left the room.
    /// Updates the UI to show the room list again.
    /// </summary>
    public override void OnLeftRoom() {

        Debug.Log("Exited room");
        status.text = "Exited room";

        nickPanel.gameObject.SetActive(false);
        browseRoomsPanel.SetActive(true);
        createRoomButton.SetActive(true);
        connectButton.SetActive(false);
        exitRoomButton.SetActive(false);
        titleText.SetActive(false);

        if (RCC_InfoLabel.Instance)
            RCC_InfoLabel.Instance.ShowInfo("Exited room");

    }

    /// <summary>
    /// Called by the UI to leave the lobby if the player is in one.
    /// </summary>
    public void ExitLobby() {

        if (PhotonNetwork.InLobby) {

            Debug.Log("Leaving the lobby");
            status.text = "Leaving the lobby";

            PhotonNetwork.LeaveLobby();

            if (RCC_InfoLabel.Instance)
                RCC_InfoLabel.Instance.ShowInfo("Leaving the lobby");

        }

    }

    /// <summary>
    /// Callback from Photon when the local player has left the lobby.
    /// Updates the UI to allow reconnection.
    /// </summary>
    public override void OnLeftLobby() {

        Debug.Log("Exited to lobby");
        status.text = "Exited to lobby";

        nickPanel.gameObject.SetActive(true);
        browseRoomsPanel.SetActive(false);
        createRoomButton.SetActive(false);
        connectButton.SetActive(false);
        exitRoomButton.SetActive(false);
        chatLinesPanel.SetActive(false);
        titleText.SetActive(false);

        if (RCC_InfoLabel.Instance)
            RCC_InfoLabel.Instance.ShowInfo("Exited lobby");

    }

    /// <summary>
    /// Callback from Photon when disconnected from the server.
    /// Resets the UI to the initial state.
    /// </summary>
    /// <param name="cause">The reason for the disconnection.</param>
    public override void OnDisconnected(DisconnectCause cause) {

        Debug.Log("Disconnected");
        status.text = "Disconnected";

        nickPanel.gameObject.SetActive(true);
        browseRoomsPanel.SetActive(false);
        createRoomButton.SetActive(false);
        connectButton.SetActive(false);
        exitRoomButton.SetActive(false);
        chatLinesPanel.SetActive(false);
        titleText.SetActive(false);

        if (RCC_InfoLabel.Instance)
            RCC_InfoLabel.Instance.ShowInfo("Disconnected");

    }

    /// <summary>
    /// Loads a specific scene using Photon, to ensure all players in the room also load it.
    /// </summary>
    /// <param name="level">Name of the scene to load.</param>
    public void LoadLevel(string level) {

        Debug.Log("Loading the level");
        status.text = "Loading the level";

        PhotonNetwork.LoadLevel(level);

    }

    /// <summary>
    /// Callback from Photon if creating a room fails.
    /// Returns the UI to the lobby state.
    /// </summary>
    /// <param name="returnCode">Short code representing the failure reason.</param>
    /// <param name="message">Descriptive message of the failure.</param>
    public override void OnCreateRoomFailed(short returnCode, string message) {

        Debug.Log("Create room failed, returning to lobby");
        status.text = "Create room failed, returning to lobby";

        nickPanel.gameObject.SetActive(false);
        browseRoomsPanel.SetActive(true);
        createRoomButton.SetActive(true);
        connectButton.SetActive(false);
        exitRoomButton.SetActive(false);
        chatLinesPanel.SetActive(false);
        titleText.SetActive(false);

        if (RCC_InfoLabel.Instance)
            RCC_InfoLabel.Instance.ShowInfo("Create room failed, returning to lobby");

    }

    /// <summary>
    /// Callback from Photon if joining a room fails.
    /// Returns the UI to the lobby state.
    /// </summary>
    /// <param name="returnCode">Short code representing the failure reason.</param>
    /// <param name="message">Descriptive message of the failure.</param>
    public override void OnJoinRoomFailed(short returnCode, string message) {

        Debug.Log("Join room failed, returning to lobby");
        status.text = "Join room failed, returning to lobby";

        nickPanel.gameObject.SetActive(false);
        browseRoomsPanel.SetActive(true);
        createRoomButton.SetActive(true);
        connectButton.SetActive(false);
        exitRoomButton.SetActive(false);
        chatLinesPanel.SetActive(false);
        titleText.SetActive(false);

        if (RCC_InfoLabel.Instance)
            RCC_InfoLabel.Instance.ShowInfo("Join room failed, returning to lobby");

    }

    /// <summary>
    /// Callback from Photon if joining a random room fails.
    /// Returns the UI to the lobby state.
    /// </summary>
    /// <param name="returnCode">Short code representing the failure reason.</param>
    /// <param name="message">Descriptive message of the failure.</param>
    public override void OnJoinRandomFailed(short returnCode, string message) {

        Debug.Log("Join random room failed, returning to lobby");
        status.text = "Join random room failed, returning to lobby";

        nickPanel.gameObject.SetActive(false);
        browseRoomsPanel.SetActive(true);
        createRoomButton.SetActive(true);
        connectButton.SetActive(false);
        exitRoomButton.SetActive(false);
        chatLinesPanel.SetActive(false);
        titleText.SetActive(false);

        if (RCC_InfoLabel.Instance)
            RCC_InfoLabel.Instance.ShowInfo("Join random room failed, returning to lobby");
    }

}

#endif
