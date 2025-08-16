using System;
using UnityEngine;
using UnityEngine.UI;

namespace RGSK
{
	public class MenuSingleRacePanel : MonoBehaviour
	{
		public VehicleSelectionPanel vehicleSeletPanel;

		public QuickRacePanel quickRacePanel;

		public MenuOptionsPanel optionsPanel;

		public Button vehicleSelectButton;

		public Button quickRaceButton;

		public Button optionsButton;

		public Button quitButton;

		[Space(10f)]
		public Button startRace;

		[Header("Back")]
		public Button backButton;

		public GameObject previousPanel;

		internal object onClick;

		private string sceneToLoad;

		private void Start()
		{
			if (quickRaceButton != null)
			{
				quickRaceButton.onClick.AddListener(delegate
				{
					ShowPanel("Quick Race");
				});
			}
			if (vehicleSelectButton != null)
			{
				vehicleSelectButton.onClick.AddListener(delegate
				{
					ShowPanel("VehicleSelect");
				});
			}
			if (optionsButton != null)
			{
				optionsButton.onClick.AddListener(delegate
				{
					ShowPanel("Options");
				});
			}
			if (quitButton != null)
			{
				quitButton.onClick.AddListener(delegate
				{
					Quit();
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

		private void Quit()
		{
			throw new NotImplementedException();
		}

		private void ShowPanel(string panel)
		{
			base.gameObject.SetActive(value: false);
			if (!(panel == "QuickRace"))
			{
				if (!(panel == "VehicleSelect"))
				{
					if (panel == "Options" && optionsPanel != null)
					{
						optionsPanel.gameObject.SetActive(value: true);
					}
				}
				else if (vehicleSeletPanel != null)
				{
					vehicleSeletPanel.gameObject.SetActive(value: true);
				}
			}
			else if (quickRacePanel != null)
			{
				quickRacePanel.gameObject.SetActive(value: true);
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
