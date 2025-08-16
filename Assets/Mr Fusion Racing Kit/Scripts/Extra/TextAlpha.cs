using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class TextAlpha : MonoBehaviour
{
    private Text uiText;
    private Color color;
    public float fadeSpeed = 1.5f;

    void Start()
    {
        uiText = GetComponent<Text>();
        color = uiText.color;
    }


    void Update ()
    {
        color.a = Mathf.PingPong(Time.unscaledTime * fadeSpeed, 1.0f);
        uiText.color = color;
	}
}
