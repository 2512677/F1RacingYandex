using UnityEngine;

public class SelectImage : MonoBehaviour
{
	public void OnSelectImage()
	{
		UISlideShow.SP.OnClickImage(base.gameObject);
	}
}
