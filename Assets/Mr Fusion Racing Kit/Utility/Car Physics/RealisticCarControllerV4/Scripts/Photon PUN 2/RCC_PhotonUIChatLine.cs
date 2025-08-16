//----------------------------------------------
//            Realistic Car Controller
//
// Copyright © 2014 - 2024 BoneCracker Games
// https://www.bonecrackergames.com
// Ekrem Bugra Ozdoganlar
//
//----------------------------------------------

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RCC_PhotonUIChatLine : RCC_Core {

    public TextMeshProUGUI text;

    private void Awake() {

        text = GetComponent<TextMeshProUGUI>();

    }

    public void Line(string chatText) {

        text.text = chatText;

    }

}
