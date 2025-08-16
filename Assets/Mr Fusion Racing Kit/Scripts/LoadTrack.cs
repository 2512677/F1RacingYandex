using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadTrack : MonoBehaviour

{
	public string TrackName;

	private void Awake()
	{
		Application.LoadLevelAdditive(TrackName);
	}
}
