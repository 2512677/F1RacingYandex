namespace UnluckSoftware
{
	//	Simple Particle Scaler
	//	Copyright Unluck Software	
	//	www.chemicalbliss.com																			

	using UnityEngine;
	using UnityEditor;
	[System.Serializable]

//#pragma warning disable 0618
	public class ParticleScaler :EditorWindow
	{
		public float scaleMultiplier = 1.0f;
		public float originalScale = 1.0f;
		public bool autoRename;
		//public string titleTex = "Particle Scaler";
		public Texture2D tex;
		public string cachePath = "Assets/";

		GUIStyle styleBigButton = null;
		GUIStyle styleScaleButton = null;
		GUIStyle styleToggle = null;

		[MenuItem("MR Fusuin Engine/VFX/Particle Scaler")]
		public static void ShowWindow()
		{
			EditorWindow win = EditorWindow.GetWindow(typeof(ParticleScaler));
			win.titleContent = new GUIContent("Particle Scaler");
			win.minSize = new Vector2(200.0f, 130.0f);
			win.maxSize = new Vector2(200.0f, 130.0f);
		}

		public void OnEnable()
		{
			if (tex != null) return;
			//byte[] b64_bytes = System.Convert.FromBase64String(titleTex);
			tex = new Texture2D(1, 1);
			//tex.LoadImage(b64_bytes);
		}

		public void ScaleParticles()
		{
			foreach (GameObject gameObj in Selection.gameObjects)
			{
				if (autoRename)
				{
					string[] s = gameObj.name.Split('¤');
					if (s.Length == 1)
					{
						gameObj.name += " ¤" + scaleMultiplier;
					} else
					{
						float i = float.Parse(s[s.Length - 1]);
						gameObj.name = s[0] + "¤" + scaleMultiplier * i;
					}
				}
				ParticleSystem[] pss = null;
				pss = gameObj.GetComponentsInChildren<ParticleSystem>();
				foreach (ParticleSystem ps in pss)
				{
					ps.Stop();
					ScaleParticles(gameObj, ps);
					ps.Play();
				}
			}
		}

		void StyleSetup()
		{
			if (styleBigButton != null) return;
			styleBigButton = new GUIStyle(GUI.skin.button);
			styleBigButton.fixedWidth = 90.0f;
			styleBigButton.fixedHeight = 20.0f;
			styleBigButton.fontSize = 9;
			styleScaleButton = new GUIStyle(GUI.skin.button);
			styleScaleButton.fixedWidth = 46.0f;
			styleScaleButton.fixedHeight = 15.0f;
			styleScaleButton.fontSize = 9;
			styleToggle = new GUIStyle(GUI.skin.toggle);
			styleToggle.fontSize = 9;
		}

		public void OnGUI()
		{
			StyleSetup();
			if (tex != null)
				GUI.DrawTexture(new Rect(position.width * .5f - 100, 0, 200, 22), tex, ScaleMode.ScaleToFit, true, 0);
			GUILayout.Space(20);
			EditorGUILayout.Space();
			scaleMultiplier = EditorGUILayout.Slider(scaleMultiplier, 0.01f, 4.0f);
			EditorGUILayout.Space();
			EditorGUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			if (GUILayout.Button("0.25", styleScaleButton))
			{
				scaleMultiplier = 0.25f;
				ScaleParticles();
			}
			if (GUILayout.Button("0.5", styleScaleButton))
			{
				scaleMultiplier = 0.5f;
				ScaleParticles();
			}
			if (GUILayout.Button("1.5", styleScaleButton))
			{
				scaleMultiplier = 1.5f;
				ScaleParticles();
			}
			if (GUILayout.Button("2", styleScaleButton))
			{
				scaleMultiplier = 2f;
				ScaleParticles();
			}
			GUILayout.FlexibleSpace();
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			if (GUILayout.Button("Крутить", styleBigButton))
			{
				ScaleParticles();
			}
			GUILayout.FlexibleSpace();
			if (GUILayout.Button("Сохранит Prefab", styleBigButton))
			{
				CreatePrefabs();
			}
			GUILayout.FlexibleSpace();
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.BeginHorizontal();
			GUILayout.Label("", GUILayout.Width(10.0f));
			autoRename = GUILayout.Toggle(autoRename, "Автопереименование", styleToggle);
			GUILayout.FlexibleSpace();
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.Space();
			//	GUI.color = colorBlueLight;
			//EditorGUILayout.BeginHorizontal();
			//if (GUILayout.Button("Play", EditorStyles.miniButtonLeft))
			//{
			//	ParticleCalls("Play");
			//}
			//if (GUILayout.Button("Pause", EditorStyles.miniButtonMid))
			//{
			//	ParticleCalls("Pause");
			//}
			//if (GUILayout.Button("Stop", EditorStyles.miniButtonRight))
			//{
			//	ParticleCalls("Stop");
			//}
			//EditorGUILayout.EndHorizontal();
		}

		public void CreatePrefabs()
		{
			if (Selection.gameObjects.Length > 0)
			{
				string path = EditorUtility.SaveFolderPanel("Select Folder ", cachePath, "");
				if (path.Length > 0)
				{
					if (path.Contains("" + Application.dataPath))
					{
						string s = "" + path + "/";
						string d = "" + Application.dataPath + "/";
						string p = "Assets/" + s.Remove(0, d.Length);
						cachePath = p;
						GameObject[] objs = Selection.gameObjects;
						bool cancel = false;
						foreach (GameObject go in objs)
						{
							if (!cancel)
							{
								if (AssetDatabase.LoadAssetAtPath(p + go.gameObject.name + ".prefab", typeof(GameObject)) != null)
								{
									int option = EditorUtility.DisplayDialogComplex("Вы уверены?", "" + go.gameObject.name + ".prefab" + "уже существует. Хотите перезаписать его?", "Да", "Нет", "Отмена");
									switch (option)
									{
										case 0:
										CreateNew(go.gameObject, p + go.gameObject.name + ".prefab");
										goto case 1;
										case 1:
										break;
										case 2:
										cancel = true;
										break;
										default:
										Debug.LogError("Unrecognized option.");
										break;
									}
								} else CreateNew(go.gameObject, p + go.gameObject.name + ".prefab");
							}
						}
					} else
					{
						Debug.LogError("Prefab Save Failed: Can't save outside project: " + path);
					}
				}
			} else
			{
				Debug.LogWarning("No GameObjects Selected");
			}
		}

		public static void CreateNew(GameObject obj, string localPath)
		{
#if UNITY_2018_3 || UNITY_2018_4 || UNITY_2018_5 || UNITY_2018_6 || UNITY_2018_7 || UNITY_2018_8 || UNITY_2018_9 || UNITY_2019 || UNITY_2020 || UNITY_2021 || UNITY_2022 || UNITY_2023 || UNITY_2024 || UNITY_2025
			PrefabUtility.SaveAsPrefabAssetAndConnect(obj, localPath, InteractionMode.UserAction);
			//PrefabUtility.SaveAsPrefabAsset(obj, localPath);
#else
			Object prefab = PrefabUtility.CreateEmptyPrefab(localPath);
			PrefabUtility.ReplacePrefab(obj, prefab, ReplacePrefabOptions.ConnectToPrefab);
#endif
		}

		public void UpdateParticles()
		{
			foreach (GameObject gameObj in Selection.gameObjects)
			{
				ParticleSystem[] pss = null;
				pss = gameObj.GetComponentsInChildren<ParticleSystem>();
				foreach (ParticleSystem ps in pss)
				{
					ps.Stop();
					ps.Play();
				}
			}
		}

		public void ParticleCalls(string call)
		{
			foreach (GameObject gameObj in Selection.gameObjects)
			{
				ParticleSystem[] pss = null;
				pss = gameObj.GetComponentsInChildren<ParticleSystem>();
				foreach (ParticleSystem ps in pss)
				{
					if (call == "Pause") ps.Pause();
					else if (call == "Play") ps.Play();
					else if (call == "Stop")
					{
						ps.Stop();
						ps.Clear();
					}
				}
			}
		}


		public void ScaleParticles(GameObject __parent_cs1, ParticleSystem __particles_cs1)
		{

			if (__parent_cs1 != __particles_cs1.gameObject)
			{
				__particles_cs1.transform.localPosition *= scaleMultiplier;
			}
			SerializedObject serializedParticles = new SerializedObject(__particles_cs1);

#if UNITY_5_0 || UNITY_5_1 || UNITY_5_2 || UNITY_5_3 || UNITY_5_4
			serializedParticles.FindProperty("InitialModule.gravityModifier").floatValue *= scaleMultiplier;
#else
			serializedParticles.FindProperty("InitialModule.gravityModifier.scalar").floatValue *= scaleMultiplier;
			serializedParticles.FindProperty("InitialModule.gravityModifier.minScalar").floatValue *= scaleMultiplier;

#endif

#if UNITY_5_0 || UNITY_5_1 || UNITY_5_2 || UNITY_5_3 || UNITY_5_4
#else
			serializedParticles.FindProperty("NoiseModule.strength.scalar").floatValue *= scaleMultiplier;
			serializedParticles.FindProperty("NoiseModule.strength.minScalar").floatValue *= scaleMultiplier;
			serializedParticles.FindProperty("NoiseModule.strengthY.scalar").floatValue *= scaleMultiplier;
			serializedParticles.FindProperty("NoiseModule.strengthY.minScalar").floatValue *= scaleMultiplier;
			serializedParticles.FindProperty("NoiseModule.strengthZ.scalar").floatValue *= scaleMultiplier;
			serializedParticles.FindProperty("NoiseModule.strengthZ.minScalar").floatValue *= scaleMultiplier;
			serializedParticles.FindProperty("NoiseModule.frequency").floatValue /= scaleMultiplier;

			//sizeAmount
			serializedParticles.FindProperty("NoiseModule.sizeAmount.scalar").floatValue *= scaleMultiplier;
			serializedParticles.FindProperty("NoiseModule.sizeAmount.minScalar").floatValue *= scaleMultiplier;
			ScaleCurve(serializedParticles.FindProperty("NoiseModule.sizeAmount.minCurve").animationCurveValue);
			ScaleCurve(serializedParticles.FindProperty("NoiseModule.sizeAmount.maxCurve").animationCurveValue);
			//rotationAmount
			serializedParticles.FindProperty("NoiseModule.rotationAmount.scalar").floatValue *= scaleMultiplier;
			serializedParticles.FindProperty("NoiseModule.rotationAmount.minScalar").floatValue *= scaleMultiplier;
			ScaleCurve(serializedParticles.FindProperty("NoiseModule.rotationAmount.minCurve").animationCurveValue);
			ScaleCurve(serializedParticles.FindProperty("NoiseModule.rotationAmount.maxCurve").animationCurveValue);
			//positionAmount
			serializedParticles.FindProperty("NoiseModule.positionAmount.scalar").floatValue *= scaleMultiplier;
			serializedParticles.FindProperty("NoiseModule.positionAmount.minScalar").floatValue *= scaleMultiplier;
			ScaleCurve(serializedParticles.FindProperty("NoiseModule.positionAmount.minCurve").animationCurveValue);
			ScaleCurve(serializedParticles.FindProperty("NoiseModule.positionAmount.maxCurve").animationCurveValue);

			serializedParticles.FindProperty("LightsModule.rangeCurve.scalar").floatValue *= scaleMultiplier;
			serializedParticles.FindProperty("LightsModule.rangeCurve.minScalar").floatValue *= scaleMultiplier;
#endif

#if UNITY_5_0 || UNITY_5_1 || UNITY_5_2 || UNITY_5_3 || UNITY_5_5 || UNITY_5_6 || UNITY_5_7 || UNITY_5_8 || UNITY_5_9
			serializedParticles.FindProperty("InitialModule.startSize.scalar").floatValue *= scaleMultiplier;
			serializedParticles.FindProperty("InitialModule.startSpeed.scalar").floatValue *= scaleMultiplier;
#else
			serializedParticles.FindProperty("InitialModule.startSize.scalar").floatValue *= scaleMultiplier;
			serializedParticles.FindProperty("InitialModule.startSize.minScalar").floatValue *= scaleMultiplier;
			serializedParticles.FindProperty("InitialModule.startSizeY.scalar").floatValue *= scaleMultiplier;
			serializedParticles.FindProperty("InitialModule.startSizeY.minScalar").floatValue *= scaleMultiplier;
			serializedParticles.FindProperty("InitialModule.startSizeZ.scalar").floatValue *= scaleMultiplier;
			serializedParticles.FindProperty("InitialModule.startSizeZ.minScalar").floatValue *= scaleMultiplier;
			ScaleCurve(serializedParticles.FindProperty("InitialModule.startSize.minCurve").animationCurveValue);
			ScaleCurve(serializedParticles.FindProperty("InitialModule.startSize.maxCurve").animationCurveValue);
			ScaleCurve(serializedParticles.FindProperty("InitialModule.startSizeY.minCurve").animationCurveValue);
			ScaleCurve(serializedParticles.FindProperty("InitialModule.startSizeY.maxCurve").animationCurveValue);
			ScaleCurve(serializedParticles.FindProperty("InitialModule.startSizeZ.minCurve").animationCurveValue);
			ScaleCurve(serializedParticles.FindProperty("InitialModule.startSizeZ.maxCurve").animationCurveValue);
			serializedParticles.FindProperty("InitialModule.startSpeed.scalar").floatValue *= scaleMultiplier;
			serializedParticles.FindProperty("InitialModule.startSpeed.minScalar").floatValue *= scaleMultiplier;

			serializedParticles.FindProperty("VelocityModule.x.minScalar").floatValue *= scaleMultiplier;
			serializedParticles.FindProperty("VelocityModule.y.minScalar").floatValue *= scaleMultiplier;
			serializedParticles.FindProperty("VelocityModule.z.minScalar").floatValue *= scaleMultiplier;


			// VELOCIY RADIAL
			serializedParticles.FindProperty("VelocityModule.radial.minScalar").floatValue *= scaleMultiplier;
			serializedParticles.FindProperty("VelocityModule.radial.scalar").floatValue *= scaleMultiplier;
			ScaleCurve(serializedParticles.FindProperty("VelocityModule.radial.minCurve").animationCurveValue);
			ScaleCurve(serializedParticles.FindProperty("VelocityModule.radial.maxCurve").animationCurveValue);

			// TRAIL
			serializedParticles.FindProperty("TrailModule.widthOverTrail.minScalar").floatValue *= scaleMultiplier;
			serializedParticles.FindProperty("TrailModule.widthOverTrail.scalar").floatValue *= scaleMultiplier;
			ScaleCurve(serializedParticles.FindProperty("TrailModule.widthOverTrail.minCurve").animationCurveValue);
			ScaleCurve(serializedParticles.FindProperty("TrailModule.widthOverTrail.maxCurve").animationCurveValue);



			serializedParticles.FindProperty("ClampVelocityModule.x.minScalar").floatValue *= scaleMultiplier;
			serializedParticles.FindProperty("ClampVelocityModule.y.minScalar").floatValue *= scaleMultiplier;
			serializedParticles.FindProperty("ClampVelocityModule.z.minScalar").floatValue *= scaleMultiplier;



			serializedParticles.FindProperty("ForceModule.x.minScalar").floatValue *= scaleMultiplier;
			serializedParticles.FindProperty("ForceModule.y.minScalar").floatValue *= scaleMultiplier;
			serializedParticles.FindProperty("ForceModule.z.minScalar").floatValue *= scaleMultiplier;


			serializedParticles.FindProperty("ClampVelocityModule.magnitude.minScalar").floatValue *= scaleMultiplier;

#endif


#if UNITY_5
			serializedParticles.FindProperty("ShapeModule.boxX").floatValue *= scaleMultiplier;
			serializedParticles.FindProperty("ShapeModule.boxY").floatValue *= scaleMultiplier;
			serializedParticles.FindProperty("ShapeModule.boxZ").floatValue *= scaleMultiplier;
#else
			serializedParticles.FindProperty("ShapeModule.m_Scale").vector3Value *= scaleMultiplier;
#endif

#if UNITY_5_0 || UNITY_5_1 || UNITY_5_2 || UNITY_5_3 || UNITY_5_4 || UNITY_5_5
			serializedParticles.FindProperty("ShapeModule.radius").floatValue *= scaleMultiplier;
#else
			serializedParticles.FindProperty("ShapeModule.radius.value").floatValue *= scaleMultiplier;
#endif
			serializedParticles.FindProperty("VelocityModule.x.scalar").floatValue *= scaleMultiplier;
			serializedParticles.FindProperty("VelocityModule.y.scalar").floatValue *= scaleMultiplier;
			serializedParticles.FindProperty("VelocityModule.z.scalar").floatValue *= scaleMultiplier;

			ScaleCurve(serializedParticles.FindProperty("VelocityModule.x.minCurve").animationCurveValue);
			ScaleCurve(serializedParticles.FindProperty("VelocityModule.x.maxCurve").animationCurveValue);
			ScaleCurve(serializedParticles.FindProperty("VelocityModule.y.minCurve").animationCurveValue);
			ScaleCurve(serializedParticles.FindProperty("VelocityModule.y.maxCurve").animationCurveValue);
			ScaleCurve(serializedParticles.FindProperty("VelocityModule.z.minCurve").animationCurveValue);
			ScaleCurve(serializedParticles.FindProperty("VelocityModule.z.maxCurve").animationCurveValue);

			serializedParticles.FindProperty("ClampVelocityModule.x.scalar").floatValue *= scaleMultiplier;
			serializedParticles.FindProperty("ClampVelocityModule.y.scalar").floatValue *= scaleMultiplier;
			serializedParticles.FindProperty("ClampVelocityModule.z.scalar").floatValue *= scaleMultiplier;
			serializedParticles.FindProperty("ClampVelocityModule.magnitude.scalar").floatValue *= scaleMultiplier;

			ScaleCurve(serializedParticles.FindProperty("ClampVelocityModule.x.minCurve").animationCurveValue);
			ScaleCurve(serializedParticles.FindProperty("ClampVelocityModule.x.maxCurve").animationCurveValue);
			ScaleCurve(serializedParticles.FindProperty("ClampVelocityModule.y.minCurve").animationCurveValue);
			ScaleCurve(serializedParticles.FindProperty("ClampVelocityModule.y.maxCurve").animationCurveValue);
			ScaleCurve(serializedParticles.FindProperty("ClampVelocityModule.z.minCurve").animationCurveValue);
			ScaleCurve(serializedParticles.FindProperty("ClampVelocityModule.z.maxCurve").animationCurveValue);
			ScaleCurve(serializedParticles.FindProperty("ClampVelocityModule.magnitude.minCurve").animationCurveValue);
			ScaleCurve(serializedParticles.FindProperty("ClampVelocityModule.magnitude.maxCurve").animationCurveValue);

			serializedParticles.FindProperty("ForceModule.x.scalar").floatValue *= scaleMultiplier;
			serializedParticles.FindProperty("ForceModule.y.scalar").floatValue *= scaleMultiplier;
			serializedParticles.FindProperty("ForceModule.z.scalar").floatValue *= scaleMultiplier;

			ScaleCurve(serializedParticles.FindProperty("ForceModule.x.minCurve").animationCurveValue);
			ScaleCurve(serializedParticles.FindProperty("ForceModule.x.maxCurve").animationCurveValue);
			ScaleCurve(serializedParticles.FindProperty("ForceModule.y.minCurve").animationCurveValue);
			ScaleCurve(serializedParticles.FindProperty("ForceModule.y.maxCurve").animationCurveValue);
			ScaleCurve(serializedParticles.FindProperty("ForceModule.z.minCurve").animationCurveValue);
			ScaleCurve(serializedParticles.FindProperty("ForceModule.z.maxCurve").animationCurveValue);
			serializedParticles.ApplyModifiedProperties();
		}

		public void ScaleCurve(AnimationCurve curve)
		{
			for (int i = 0; i < curve.keys.Length; i++)
			{
				var tmp_cs1 = curve.keys[i];
				tmp_cs1.value *= scaleMultiplier;
				curve.keys[i] = tmp_cs1;
			}
		}
	}
}