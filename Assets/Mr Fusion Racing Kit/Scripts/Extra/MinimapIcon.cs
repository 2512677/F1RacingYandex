using UnityEngine;
using System.Collections;

public class MinimapIcon : MonoBehaviour
{
    public Transform target;
    public float height = 10;


    void Start ()
    {
        if (target != null)
        {
            gameObject.name += ":" + target.name;
        }
    }
	

	void Update ()
    {
        if (!target)
            return;

        //Follow the target
        transform.position = new Vector3(target.position.x, target.position.y + height, target.position.z);

        //Rotate in the direction of the target
        transform.eulerAngles = new Vector3(90, target.eulerAngles.y, 0);
    }
}
