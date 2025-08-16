using UnityEngine;
using System.Collections;

public class ToggleGameObject : MonoBehaviour
{
    public void ToggleActive(GameObject target)
    {
        target.SetActive(!target.activeSelf);
    }
}
