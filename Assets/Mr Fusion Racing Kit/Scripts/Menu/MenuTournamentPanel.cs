using System;
using UnityEngine;
using UnityEngine.UI;

namespace RGSK
{
	public class MenuTournamentPanel : MonoBehaviour
	{
		[Serializable]
		public class RoundInformation
		{
			public Text roundNumber;

			public Text trackName;

			public Text raceType;
		}

		public ChampionshipData[] championships;

		private int championshipIndex;

		public RoundInformation[] roundInformation;

		public Text championshipName;

		public Image championshipImage;

		public Image championshipIcon;

		public Button nextChampionshipButton;

		public Button previousChampionshipButton;

		public Button startChampionship;

		[Header("Back")]
		public Button backButton;

		public GameObject previousPanel;

		private void Start()
		{
			if (nextChampionshipButton != null)
			{
				nextChampionshipButton.onClick.AddListener(delegate
				{
					AddChampionship(1);
				});
			}
			if (previousChampionshipButton != null)
			{
				previousChampionshipButton.onClick.AddListener(delegate
				{
					AddChampionship(-1);
				});
			}
			if (startChampionship != null)
			{
				startChampionship.onClick.AddListener(delegate
				{
					StartChampionship();
				});
			}
			if (backButton != null)
			{
				backButton.onClick.AddListener(delegate
				{
					Back();
				});
			}
			ClearRoundInformation();
			AddChampionship(0);
		}

		public void AddChampionship(int direction)
		{
			championshipIndex += direction;
			championshipIndex = Mathf.Clamp(championshipIndex, 0, championships.Length - 1);
			if (championshipName != null)
			{
				championshipName.text = championships[championshipIndex].championshipName;
			}
			if (championshipImage != null && championships[championshipIndex].championshipImage != null)
			{
				championshipImage.sprite = championships[championshipIndex].championshipImage;
			}
			if (championshipIcon != null && championships[championshipIndex].championshipIcon != null)
			{
				championshipIcon.sprite = championships[championshipIndex].championshipIcon;
			}
			ClearRoundInformation();
			for (int i = 0; i < championships[championshipIndex].championshipRounds.Count && i <= roundInformation.Length - 1; i++)
			{
				if (roundInformation[i].roundNumber != null)
				{
					roundInformation[i].roundNumber.text = "Round " + (i + 1).ToString();
				}
				if (roundInformation[i].trackName != null)
				{
					roundInformation[i].trackName.text = championships[championshipIndex].championshipRounds[i].trackData.trackName;
				}
				if (roundInformation[i].raceType != null)
				{
					roundInformation[i].raceType.text = championships[championshipIndex].championshipRounds[i].raceType.ToString();
				}
			}
			if (nextChampionshipButton != null)
			{
				nextChampionshipButton.gameObject.SetActive(championshipIndex < championships.Length - 1);
			}
			if (previousChampionshipButton != null)
			{
				previousChampionshipButton.gameObject.SetActive(championshipIndex > 0);
			}
		}

		public void StartChampionship()
		{
			ChampionshipManager championshipManager = new GameObject("Championship").AddComponent<ChampionshipManager>();
			championshipManager.championshipData = championships[championshipIndex];
			if (SceneController.instance != null)
			{
				SceneController.instance.LoadScene(championshipManager.CurrentRound().trackData.scene);
			}
			else
			{
				UnityEngine.Debug.LogWarning("Cannont load the scene beacuse a SceneLoader was not found. Please add a SceneController to this scene.");
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

		private void ClearRoundInformation()
		{
			for (int i = 0; i < roundInformation.Length; i++)
			{
				if (roundInformation[i].roundNumber != null)
				{
					roundInformation[i].roundNumber.text = string.Empty;
				}
				if (roundInformation[i].trackName != null)
				{
					roundInformation[i].trackName.text = string.Empty;
				}
				if (roundInformation[i].raceType != null)
				{
					roundInformation[i].raceType.text = string.Empty;
				}
			}
		}
	}
}
