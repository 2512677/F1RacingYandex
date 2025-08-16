using UnityEngine;
using UnityEngine.UI;

namespace RGSK
{
	public class MenuOptionsPanel : MonoBehaviour
	{
		public AudioSettings audioSettings;

		public VideoSettings videoSettings;

		public GameplaySettings gameplaySettings;

		public MobileSettings mobileSettings;

		public PlayerSettings playerSettings;

		public Button audioSettingsButton;

		public Button videoSettingsButton;

		public Button gameplaySettingsButton;

		public Button mobileSettingsButton;

		public Button playerSettingsButton;

		[Header("Back")]
		public Button backButton;

		public GameObject previousPanel;

		private void Start()
		{
			if (audioSettingsButton != null)
			{
				audioSettingsButton.onClick.AddListener(delegate
				{
					ShowPanel("Audio");
				});
			}
			if (videoSettingsButton != null)
			{
				videoSettingsButton.onClick.AddListener(delegate
				{
					ShowPanel("Video");
				});
			}
			if (gameplaySettingsButton != null)
			{
				gameplaySettingsButton.onClick.AddListener(delegate
				{
					ShowPanel("Gameplay");
				});
			}
			if (mobileSettingsButton != null)
			{
				mobileSettingsButton.onClick.AddListener(delegate
				{
					ShowPanel("Mobile");
				});
			}
			if (playerSettingsButton != null)
			{
				playerSettingsButton.onClick.AddListener(delegate
				{
					ShowPanel("Player");
				});
			}
			if (backButton != null)
			{
				backButton.onClick.AddListener(delegate
				{
					Back();
				});
			}
		}

		private void ShowPanel(string panel)
		{
			HideAllPanels();
			switch (panel)
			{
			default:
				if (panel == "Player" && playerSettings != null)
				{
					playerSettings.gameObject.SetActive(value: true);
				}
				break;
			case "Audio":
				if (audioSettings != null)
				{
					audioSettings.gameObject.SetActive(value: true);
				}
				break;
			case "Video":
				if (videoSettings != null)
				{
					videoSettings.gameObject.SetActive(value: true);
				}
				break;
			case "Gameplay":
				if (gameplaySettings != null)
				{
					gameplaySettings.gameObject.SetActive(value: true);
				}
				break;
			case "Mobile":
				if (mobileSettings != null)
				{
					mobileSettings.gameObject.SetActive(value: true);
				}
				break;
			}
		}

		private void HideAllPanels()
		{
			if (audioSettings != null)
			{
				audioSettings.gameObject.SetActive(value: false);
			}
			if (videoSettings != null)
			{
				videoSettings.gameObject.SetActive(value: false);
			}
			if (gameplaySettings != null)
			{
				gameplaySettings.gameObject.SetActive(value: false);
			}
			if (mobileSettings != null)
			{
				mobileSettings.gameObject.SetActive(value: false);
			}
			if (playerSettings != null)
			{
				playerSettings.gameObject.SetActive(value: false);
			}
		}

		public void Back()
		{
			if (previousPanel != null)
			{
				base.gameObject.SetActive(value: false);
				previousPanel.SetActive(value: true);
			}
		}
	}
}
