//----------------------------------------------
//            Realistic Car Controller
//
// Copyright © 2014 - 2024 BoneCracker Games
// https://www.bonecrackergames.com
// Ekrem Bugra Ozdoganlar
//
//----------------------------------------------

#if RCC_PHOTON && PHOTON_UNITY_NETWORKING
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RCC_PhotonUIRoom : RCC_Core {

    internal string roomNameString;
    public TextMeshProUGUI roomName;
    public TextMeshProUGUI maxPlayers;

    public void Check(string _roomName, string _maxPlayers) {

        roomNameString = _roomName;
        roomName.text = _roomName;
        maxPlayers.text = _maxPlayers;

    }

    public void JoinRoom() {

        RCC_PhotonManager.Instance.JoinSelectedRoom(this);

    }

}
#endif