using UnityEngine;

public class BreakBots : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            foreach (BotHealth b in FindObjectsOfType<BotHealth>(true))
                b.ForceDestroy();
            Debug.Log("BreakBots: все боты сломаны");
        }
    }
}
