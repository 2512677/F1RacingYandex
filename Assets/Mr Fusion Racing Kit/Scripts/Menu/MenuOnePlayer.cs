using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

namespace RGSK
{
	public class MenuOnePlayer : MonoBehaviour
	{
		public MenuSingleRacePanel singleRacePanel;

		public MenuTournamentPanel championshipPanel;

		public MenuKnockoutPanel knockoutPanel;

		public MenuOptionsPanel optionsPanel;

		public Button singleRaceButton;

		public Button championshipButton;

		public Button knockoutButton;

		public Button optionsButton;

		private void Start()
		{
			if (singleRacePanel != null)
			{
				singleRaceButton.onClick.AddListener(delegate
				{
					_003CStart_003Eg__ShowPanel_007C8_4("SingleRace");
				});
			}
			if (championshipButton != null)
			{
				championshipButton.onClick.AddListener(delegate
				{
					_003CStart_003Eg__ShowPanel_007C8_4("Tournament");
				});
			}
			if (knockoutButton != null)
			{
				knockoutButton.onClick.AddListener(delegate
				{
					_003CStart_003Eg__ShowPanel_007C8_4("Knockout");
				});
			}
			if (optionsButton != null)
			{
				optionsButton.onClick.AddListener(delegate
				{
					_003CStart_003Eg__ShowPanel_007C8_4("Options");
				});
			}
		}

		public void telegramm()
		{
			Application.OpenURL("https://t.me/+1mspOxHIR2I3YjVi");
		}

		public void facebook()
		{
			Application.OpenURL("https://www.facebook.com/groups/needforunity");
		}

		public void vk()
		{
			Application.OpenURL("https://vk.com/needforunity");
		}

		public void youtube()
		{
			Application.OpenURL("");
		}

		public void tiktok()
		{
			Application.OpenURL("");
		}

		public void instagramm()
		{
			Application.OpenURL("");
		}

		public void twitter()
		{
			Application.OpenURL("");
		}

		[CompilerGenerated]
		private void _003CStart_003Eg__ShowPanel_007C8_4(string panel)
		{
			base.gameObject.SetActive(value: false);
			switch (panel)
			{
			default:
				if (panel == "Options" && optionsPanel != null)
				{
					optionsPanel.gameObject.SetActive(value: true);
				}
				break;
			case "SingleRace":
				if (singleRacePanel != null)
				{
					singleRacePanel.gameObject.SetActive(value: true);
				}
				break;
			case "Tournament":
				if (championshipPanel != null)
				{
					championshipPanel.gameObject.SetActive(value: true);
				}
				break;
			case "Knockout":
				if (knockoutPanel != null)
				{
					knockoutPanel.gameObject.SetActive(value: true);
				}
				break;
			}
		}
	}
}
