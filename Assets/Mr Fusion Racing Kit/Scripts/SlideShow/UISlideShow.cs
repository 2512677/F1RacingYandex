using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
[HideInInspector]
public class UISlideShow : MonoBehaviour
{
	[SerializeField]
	public GameObject slideSource;

	[Header("The Slide Show Properties")]
	[Space(10f)]
	[SerializeField]
	public SlideShowEffects slideShowMethod;

	public bool autoSlideOn;

	public bool LoadingImagesAtRuntime;

	public float slideShowTime = 4f;

	[Range(0.1f, 1f)]
	public float slideAnimationSpeed;

	public static UISlideShow SP;

	private float waitForNextClick;

	private Transform currentObj;

	private bool onHoldNextClick;

	private bool onHoldPreviousClick;

	private void Awake()
	{
		SP = this;
	}

	private void Start()
	{
		if (!slideSource)
		{
			UnityEngine.Debug.Log("Please assign the parentObj component value!");
		}
		else if (!LoadingImagesAtRuntime)
		{
			InitialStartSlide();
		}
	}

	public void InitialStartSlide()
	{
		try
		{
			slideSource.transform.GetChild(slideSource.transform.childCount - 1).GetComponent<Animator>().SetTrigger("open");
			StartAutoSlide();
		}
		catch (Exception message)
		{
			UnityEngine.Debug.Log(message);
		}
	}

	private void Update()
	{
		if (UnityEngine.Input.GetKeyDown(KeyCode.Escape))
		{
			Application.Quit();
		}
	}

	public void OnNextImage()
	{
		if (currentObj == null)
		{
			currentObj = slideSource.transform.GetChild(slideSource.transform.childCount - 1);
		}
		if (!onHoldNextClick && (bool)currentObj.GetComponent<Animator>())
		{
			if (SlideShowEffects.Random == slideShowMethod)
			{
				int num = UnityEngine.Random.Range(0, 5);
				currentObj.GetComponent<Image>().fillMethod = (Image.FillMethod)num;
				slideSource.transform.GetChild(0).GetComponent<Image>().fillMethod = (Image.FillMethod)num;
				SetImageFillOrigin(currentObj, slideSource.transform.GetChild(0), (SlideShowEffects)num, 0, 1, nextSlide: true);
			}
			else
			{
				currentObj.GetComponent<Image>().fillMethod = (Image.FillMethod)slideShowMethod;
				slideSource.transform.GetChild(0).GetComponent<Image>().fillMethod = (Image.FillMethod)slideShowMethod;
				SetImageFillOrigin(currentObj, slideSource.transform.GetChild(0), slideShowMethod, 0, 1, nextSlide: true);
			}
			slideSource.transform.GetChild(0).SetAsLastSibling();
			onHoldNextClick = true;
			StartCoroutine(HoldNextImageShow());
			StartAutoSlide();
		}
	}

	private void SetImageFillOrigin(Transform currentSlide, Transform childSlide, SlideShowEffects currentMethod, int originValue1, int originValue2, bool nextSlide)
	{
		currentObj.GetComponent<Animator>().speed = slideAnimationSpeed;
		childSlide.GetComponent<Animator>().speed = slideAnimationSpeed;
		waitForNextClick = currentObj.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length;
		if (currentMethod == SlideShowEffects.Horizontal || SlideShowEffects.Vertical == currentMethod)
		{
			currentSlide.GetComponent<Image>().fillOrigin = originValue1;
			childSlide.GetComponent<Image>().fillOrigin = originValue2;
		}
		else if (nextSlide)
		{
			currentSlide.GetComponent<Image>().fillOrigin = originValue1;
			currentSlide.GetComponent<Image>().fillClockwise = false;
			childSlide.GetComponent<Image>().fillOrigin = originValue1;
			childSlide.GetComponent<Image>().fillClockwise = true;
		}
		else
		{
			currentSlide.GetComponent<Image>().fillOrigin = originValue2;
			currentSlide.GetComponent<Image>().fillClockwise = true;
			childSlide.GetComponent<Image>().fillOrigin = originValue2;
			childSlide.GetComponent<Image>().fillClockwise = false;
		}
		currentObj.GetComponent<Animator>().SetTrigger("hide");
		childSlide.GetComponent<Animator>().SetTrigger("show");
		currentObj = childSlide;
	}

	private void StartAutoSlide()
	{
		if (autoSlideOn)
		{
			CancelInvoke();
			float time = slideShowTime + waitForNextClick;
			Invoke("OnNextImage", time);
		}
	}

	public void OnPreviousImage()
	{
		if (currentObj == null)
		{
			currentObj = slideSource.transform.GetChild(slideSource.transform.childCount - 1);
		}
		if (!onHoldPreviousClick)
		{
			currentObj.transform.SetAsFirstSibling();
			int childCount = slideSource.transform.childCount;
			if (SlideShowEffects.Random == slideShowMethod)
			{
				int num = UnityEngine.Random.Range(0, 5);
				currentObj.GetComponent<Image>().fillMethod = (Image.FillMethod)num;
				slideSource.transform.GetChild(childCount - 1).GetComponent<Image>().fillMethod = (Image.FillMethod)num;
				SetImageFillOrigin(currentObj, slideSource.transform.GetChild(childCount - 1), (SlideShowEffects)num, 1, 0, nextSlide: false);
			}
			else
			{
				currentObj.GetComponent<Image>().fillMethod = (Image.FillMethod)slideShowMethod;
				slideSource.transform.GetChild(childCount - 1).GetComponent<Image>().fillMethod = (Image.FillMethod)slideShowMethod;
				SetImageFillOrigin(currentObj, slideSource.transform.GetChild(childCount - 1), slideShowMethod, 1, 0, nextSlide: false);
			}
			onHoldPreviousClick = true;
			StartCoroutine(HoldPreviousImageShow());
			StartAutoSlide();
		}
	}

	private IEnumerator HoldNextImageShow()
	{
		yield return new WaitForSeconds(waitForNextClick);
		onHoldNextClick = false;
	}

	private IEnumerator HoldPreviousImageShow()
	{
		yield return new WaitForSeconds(waitForNextClick);
		onHoldPreviousClick = false;
	}

	public void OnClickImage(GameObject obj)
	{
		if (!onHoldPreviousClick && !onHoldNextClick)
		{
			UnityEngine.Debug.Log("You are clicked on the " + obj.GetComponent<Image>().sprite.name);
		}
	}
}
