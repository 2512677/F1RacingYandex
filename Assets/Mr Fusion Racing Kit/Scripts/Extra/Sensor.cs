using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Sensor : MonoBehaviour
{
    public List<Collider> collidersInRange = new List<Collider>();
    public List<int> layers = new List<int>();

    List<Collider> _childColliders = new List<Collider>();

    public void AddLayer(int layer)
    {
        layers.Add(layer);
    }

    void GetChildColliders()
    {
        if (_childColliders.Count > 0)
            return;

        transform.parent.GetComponentsInChildren(_childColliders);
    }

    void OnTriggerEnter(Collider col)
    {
        GetChildColliders();

        if (_childColliders.Contains(col))
            return;

        if (!collidersInRange.Contains(col))
        {
            for (int i = 0; i < layers.Count; i++)
            {
                if (layers[i] == col.gameObject.layer)
                {
                    collidersInRange.Add(col);
                }
            }
        }
    }


    void OnTriggerStay(Collider col)
    {
        GetChildColliders();

        if (_childColliders.Contains(col))
            return;

        if (!collidersInRange.Contains(col))
        {
            for (int i = 0; i < layers.Count; i++)
            {
                if (layers[i] == col.gameObject.layer)
                {
                    collidersInRange.Add(col);
                }
            }
        }
    }


    void OnTriggerExit(Collider col)
    {
        if (collidersInRange.Contains(col))
        {
            collidersInRange.Remove(col);
        }
    }
}
