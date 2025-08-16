using UnityEngine;

public class Lights : MonoBehaviour
{
	public GameObject Car;

	public GameObject Headlights;

	public GameObject Taillights;

	public GameObject ReverseLight;

	private void Start()
	{
	}

	private void OnTriggerEnter(Collider col)
	{
		if (Car.GetComponent<RCC_CarControllerV4>().engineRunning = true)
		{
			Headlights.SetActive(true);
		}
		else
		{
			Headlights.SetActive(false);
		}
	}

	private void OnTriggerExit(Collider col)
	{
		Headlights.SetActive(false);
	}
}
