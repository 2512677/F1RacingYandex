using UnityEngine;

public class bl_Hud : MonoBehaviour
{
	public bl_HudInfo HudInfo;

	private void Start()
	{
		
		{
			Show();
		}
		if (bl_HudManager.instance != null)
		{
			if (HudInfo.m_Target == null)
			{
				HudInfo.m_Target = GetComponent<Transform>();
			}
			if (HudInfo.ShowDynamically)
			{
				HudInfo.Hide = true;
			}
			bl_HudManager.instance.CreateHud(HudInfo);
		}
		else
		{
			UnityEngine.Debug.LogError("Need have a Hud Manager in scene");
		}
	}

	public void Show()
	{
		if (bl_HudManager.instance != null)
		{
			bl_HudManager.instance.HideStateHud(HudInfo);
		}
	}

	public void Hide()
	{
		if (bl_HudManager.instance != null)
		{
			bl_HudManager.instance.HideStateHud(HudInfo, hide: true);
		}
	}
}
