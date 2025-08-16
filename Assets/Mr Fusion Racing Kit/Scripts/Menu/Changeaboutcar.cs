using UnityEngine;

public class Changeaboutcar : MonoBehaviour
{
	public GameObject perfomance;

	public GameObject general;

	public bool isPermomance;

	private void Start()
	{
	}

	private void Update()
	{
	}

	public void BackgroundChange()
	{
		if (!isPermomance)
		{
			perfomance.SetActive(value: true);
			general.SetActive(value: false);
			isPermomance = true;
		}
		else
		{
			perfomance.SetActive(value: false);
			general.SetActive(value: true);
			isPermomance = false;
		}
	}
}
