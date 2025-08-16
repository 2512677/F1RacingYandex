using System.Collections.Generic;
using UnityEngine;

public class bl_HudManager : MonoBehaviour
{
	[Tooltip("Hud list manager, you can add a new hud directly here.")]
	public List<bl_HudInfo> Huds = new List<bl_HudInfo>();

	[Space(5f)]
	[Tooltip("You can use MainCamera or the root of your player")]
	public Transform LocalPlayer;

	[Space(5f)]
	public float clampBorder = 12f;

	public bool useGizmos = true;

	[Header("GUI Scaler")]
	[Tooltip("The resolution the UI layout is designed for. If the screen resolution is larger, the GUI will be scaled up, and if it's smaller, the GUI will be scaled down. This is done in accordance with the Screen Match Mode.")]
	[Space(5f)]
	public Vector2 m_ReferenceResolution = new Vector2(800f, 600f);

	[SerializeField]
	[Tooltip("Determines if the scaling is using the width or height as reference, or a mix in between.")]
	[Range(0f, 1f)]
	public float m_MatchWidthOrHeight;

	[Tooltip("Select Reference Resolution automatically in run time.")]
	public bool AutoScale = true;

	public GUIStyle TextStyle;

	private static bl_HudManager _instance;

	public static bl_HudManager instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = UnityEngine.Object.FindObjectOfType<bl_HudManager>();
			}
			return _instance;
		}
	}

	private void Start()
	{
		LocalPlayer = GameObject.FindWithTag("Player").transform;
	}

	private void OnDestroy()
	{
		_instance = null;
	}

	private void FixedUpdate()
	{
		if (AutoScale)
		{
			m_ReferenceResolution.x = Screen.width;
			m_ReferenceResolution.y = Screen.height;
		}
	}

	private void OnGUI()
	{
		if (bl_HudUtility.mCamera == null)
		{
			return;
		}
		if (LocalPlayer == null)
		{
			LocalPlayer = GameObject.FindWithTag("Player").transform;
		}
		for (int i = 0; i < Huds.Count; i++)
		{
			if (!Huds[i].Hide)
			{
				OnScreen(i);
				OffScreen(i);
			}
		}
	}

	private void OnScreen(int i)
	{
		if (Huds[i].m_Target == null)
		{
			Huds.Remove(Huds[i]);
			return;
		}
		Vector3 vector = Huds[i].m_Target.position + Huds[i].Offset;
		if (!(Vector3.Dot(LocalPlayer.forward, vector - LocalPlayer.position) > 0f))
		{
			return;
		}
		Vector3 vector2 = bl_HudUtility.mCamera.WorldToViewportPoint(vector);
		Vector2 vector3 = new Vector2(vector2.x * (float)Screen.width, (float)Screen.height * (1f - vector2.y));
		if (!Huds[i].Arrow.ShowArrow)
		{
			vector3.x = Mathf.Clamp(vector3.x, clampBorder, (float)Screen.width - clampBorder);
			vector3.y = Mathf.Clamp(vector3.y, clampBorder, (float)Screen.height - clampBorder);
		}
		float num = Vector3.Distance(LocalPlayer.position, vector);
		float num2 = num;
		if (num > 50f)
		{
			num = 50f;
		}
		float num3 = (Huds[i].m_TypeHud != TypeHud.Decreasing) ? ((50f - num) / 100f * 0.9f + 0.1f) : ((50f + num) / 100f * 0.9f + 0.1f);
		float num4 = (float)Huds[i].m_Icon.width * num3;
		if (num4 >= Huds[i].m_MaxSize)
		{
			num4 = Huds[i].m_MaxSize;
		}
		float num5 = (float)Huds[i].m_Icon.height * num3;
		if (num5 >= Huds[i].m_MaxSize)
		{
			num5 = Huds[i].m_MaxSize;
		}
		if (Huds[i].isPalpitin)
		{
			if (Huds[i].m_Color.a <= 0f)
			{
				Huds[i].tip = false;
			}
			else if (Huds[i].m_Color.a >= 1f)
			{
				Huds[i].tip = true;
			}
			if (!Huds[i].tip)
			{
				Huds[i].m_Color.a += Time.deltaTime * 0.5f;
			}
			else
			{
				Huds[i].m_Color.a -= Time.deltaTime * 0.5f;
			}
		}
		GUI.color = Huds[i].m_Color;
		GUI.DrawTexture(new Rect(vector3.x - num4 / 2f, vector3.y - num5 / 2f, num4, num5), Huds[i].m_Icon);
		if (!Huds[i].ShowDistance)
		{
			GUI.Label(new Rect(vector3.x - (float)Huds[i].m_Text.Length, vector3.y - num3 - 35f, 300f, 50f), Huds[i].m_Text, TextStyle);
		}
		else
		{
			GUI.Label(new Rect(vector3.x - (float)Huds[i].m_Text.Length, vector3.y - num3 - 35f, 300f, 60f), Huds[i].m_Text + "\n <color=whitte>[" + $"{num2:N0}m" + "]</color>", TextStyle);
		}
	}

	private void OffScreen(int i)
	{
		if (!(Huds[i].Arrow.ArrowIcon != null) || !Huds[i].Arrow.ShowArrow)
		{
			return;
		}
		Vector3 position = Huds[i].m_Target.position + Huds[i].Arrow.ArrowOffset;
		Vector3 a = bl_HudUtility.mCamera.WorldToScreenPoint(position);
		a.x /= bl_HudUtility.mCamera.pixelWidth;
		a.y /= bl_HudUtility.mCamera.pixelHeight;
		Vector3 direction = Huds[i].m_Target.position - bl_HudUtility.mCamera.transform.position;
		Vector3 vector = bl_HudUtility.mCamera.transform.InverseTransformDirection(direction).normalized / 5f;
		a.x = 0.5f + vector.x * 20f / bl_HudUtility.mCamera.aspect;
		a.y = 0.5f + vector.y * 20f;
		if (a.z < 0f)
		{
			a *= -1f;
			a *= -1f;
		}
		GUI.color = Huds[i].m_Color;
		float num = (float)bl_HudUtility.mCamera.pixelWidth * a.x;
		float num2 = (float)bl_HudUtility.mCamera.pixelHeight * (1f - a.y);
		if (!bl_HudUtility.isOnScreen(bl_HudUtility.ScreenPosition(Huds[i].m_Target), Huds[i].m_Target))
		{
			float rotation = bl_HudUtility.GetRotation(bl_HudUtility.mCamera.pixelWidth / 2, bl_HudUtility.mCamera.pixelHeight / 2, num, num2);
			Vector2 pivot = bl_HudUtility.GetPivot(num, num2, Huds[i].Arrow.ArrowSize);
			Matrix4x4 matrix = GUI.matrix;
			GUIUtility.RotateAroundPivot(rotation, pivot);
			GUI.DrawTexture(new Rect(pivot.x - bl_HudUtility.HalfSize(Huds[i].Arrow.ArrowSize), pivot.y - bl_HudUtility.HalfSize(Huds[i].Arrow.ArrowSize), Huds[i].Arrow.ArrowSize, Huds[i].Arrow.ArrowSize), Huds[i].Arrow.ArrowIcon);
			GUIUtility.RotateAroundPivot(0f - rotation, pivot);
			GUI.matrix = matrix;
			Vector2 vector2 = bl_HudUtility.Marge(pivot, 25f);
			if (!Huds[i].ShowDistance)
			{
				GUI.Label(bl_HudUtility.ScalerRect(new Rect(pivot.x + vector2.x, pivot.y + vector2.y, 300f, 75f)), Huds[i].m_Text, TextStyle);
			}
			else
			{
				float num3 = Vector3.Distance(LocalPlayer.position, Huds[i].m_Target.position);
				GUI.Label(bl_HudUtility.ScalerRect(new Rect(pivot.x + vector2.x, pivot.y + vector2.y, 300f, 75f)), Huds[i].m_Text + "\n <color=whitte>[" + $"{num3:N0}m" + "]</color>", TextStyle);
			}
			GUI.DrawTexture(bl_HudUtility.ScalerRect(new Rect(pivot.x + vector2.x, pivot.y + (float)(Huds[i].ShowDistance ? 20 : 10) + vector2.y, 25f, 25f)), Huds[i].m_Icon);
		}
		GUI.color = Color.white;
	}

	public void CreateHud(bl_HudInfo info)
	{
		Huds.Add(info);
	}

	public void RemoveHud(int i)
	{
		Huds.RemoveAt(i);
	}

	public void RemoveHud(bl_HudInfo hud)
	{
		if (Huds.Contains(hud))
		{
			Huds.Remove(hud);
		}
		else
		{
			UnityEngine.Debug.Log("Huds list dont contain this hud!");
		}
	}

	public void HideStateHud(int i, bool hide = false)
	{
		if (Huds[i] != null)
		{
			Huds[i].Hide = hide;
		}
	}

	public void HideStateHud(bl_HudInfo hud, bool hide = false)
	{
		if (!Huds.Contains(hud))
		{
			return;
		}
		for (int i = 0; i < Huds.Count; i++)
		{
			if (Huds[i] == hud)
			{
				Huds[i].Hide = hide;
			}
		}
	}

	private void OnDrawGizmosSelected()
	{
		if (!useGizmos)
		{
			return;
		}
		for (int i = 0; i < Huds.Count; i++)
		{
			if (Huds[i].m_Target != null)
			{
				Gizmos.color = new Color(0f, 0.35f, 0.9f, 0.9f);
				Gizmos.DrawWireSphere(Huds[i].m_Target.position, 3f);
				Gizmos.color = new Color(0f, 0.35f, 0.9f, 0.3f);
				Gizmos.DrawSphere(Huds[i].m_Target.position, 3f);
				if (i < Huds.Count - 1)
				{
					Gizmos.DrawLine(Huds[i].m_Target.position, Huds[i + 1].m_Target.position);
				}
				else
				{
					Gizmos.DrawLine(Huds[i].m_Target.position, Huds[0].m_Target.position);
				}
			}
		}
	}
}
