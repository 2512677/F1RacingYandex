using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace RGSK
{
    public class RaceUIManager : MonoBehaviour
    {
        public PreRacePanel preRacePanel;
        public StartingGridPanel startingGridPanel;
        public RacePanel racePanel;
        public InRaceStandingsPanel inRaceStandings;
        public DriftPanel driftPanel;
        public TargetScorePanel targetScorePanel;
        public PausePanel pausePanel;
        public PostRacePanel postRacePanel;
        public RaceResultsPanel raceResultsPanel;
        public ChampionshipResultsPanel championshipResultsPanel;
        public OtherResultsPanel otherResultsPanel;
        public ReplayPanel replayPanel;
        public GameObject activePanel;
        public static RaceUIManager instance; // Singleton

        public bool autoSelectButtons;

        void Awake()
        {
            //Find and assign all panels in the children
            preRacePanel = gameObject.GetComponentInChildren<PreRacePanel>(true);
            startingGridPanel = gameObject.GetComponentInChildren<StartingGridPanel>(true);
            racePanel = gameObject.GetComponentInChildren<RacePanel>(true);
            inRaceStandings = gameObject.GetComponentInChildren<InRaceStandingsPanel>(true);
            driftPanel = gameObject.GetComponentInChildren<DriftPanel>(true);
            targetScorePanel = gameObject.GetComponentInChildren<TargetScorePanel>(true);
            pausePanel = gameObject.GetComponentInChildren<PausePanel>(true); ;
            postRacePanel = gameObject.GetComponentInChildren<PostRacePanel>(true);
            raceResultsPanel = gameObject.GetComponentInChildren<RaceResultsPanel>(true);
            championshipResultsPanel = gameObject.GetComponentInChildren<ChampionshipResultsPanel>(true);
            otherResultsPanel = gameObject.GetComponentInChildren<OtherResultsPanel>(true);
            replayPanel = gameObject.GetComponentInChildren<ReplayPanel>(true);

            //Setup all the race entry panels
            RaceEntry[] raceEntryPanels = GetComponentsInChildren<RaceEntry>(true);
            foreach (RaceEntry entryInfo in raceEntryPanels)
            {
                entryInfo.Setup();
            }

            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }


        void Start()
        {
            if (racePanel == null)
            {
                Debug.LogWarning("A Race Panel has not been assigned!");
            }

            UpdatePanelsBasedOnRaceType();
        }


        public void UpdateUIBasedOnRaceState(RaceState state)
        {
            //Hide all UI panels
            HideAllPanels();

            //Show the UI panel
            ShowPanel(state.ToString());
        }


        public void UpdatePanelsBasedOnRaceType()
        {
            if (RaceManager.instance == null)
                return;

            if (driftPanel != null)
            {
                driftPanel.gameObject.SetActive(RaceManager.instance.raceType == RaceType.Drift);
            }

            if (targetScorePanel != null)
            {
                targetScorePanel.gameObject.SetActive(RaceManager.instance.raceType == RaceType.Drift
                    || RaceManager.instance.raceType == RaceType.TimeAttack);
            }
        }



        public void ShowPanel(string panelName)
        {
            switch (panelName) // Заменяем "panel" на "panelName"
            {
                case "PreRace":
                    if (preRacePanel != null)
                    {
                        preRacePanel.gameObject.SetActive(true);
                        activePanel = preRacePanel.gameObject;
                    }
                    break;

                case "Race":
                    if (racePanel != null)
                    {
                        racePanel.gameObject.SetActive(true);
                        activePanel = racePanel.gameObject;
                    }
                    break;

                case "Pause":
                    if (pausePanel != null)
                    {
                        pausePanel.gameObject.SetActive(true);
                        activePanel = pausePanel.gameObject;
                    }
                    break;

                case "PostRace":
                    if (postRacePanel != null)
                    {
                        postRacePanel.gameObject.SetActive(true);
                        activePanel = postRacePanel.gameObject;
                    }
                    break;

                case "Replay":
                    if (replayPanel != null)
                    {
                        replayPanel.gameObject.SetActive(true);
                        activePanel = replayPanel.gameObject;
                    }
                    break;

                default:
                    Debug.LogError($"RaceUIManager: Unknown panel name {panelName}");
                    break;
            }

            if (autoSelectButtons)
            {
                SelectButtonInPanel(activePanel);
            }
        }


        public void UpdateRaceStandingsUI()
        {
            if(startingGridPanel != null)
            {
                startingGridPanel.UpdateStartingGrid();
            }

            if (racePanel != null)
            {
                racePanel.UpdatePositionText();
            }

            if (inRaceStandings != null)
            {
                inRaceStandings.UpdateStandings();
            }

            if (raceResultsPanel != null)
            {
                raceResultsPanel.UpdateRaceResults();
            }

            if(championshipResultsPanel != null)
            {
                championshipResultsPanel.UpdateChampionshipResults();
            }

            if(otherResultsPanel != null)
            {
                otherResultsPanel.UpdateResults();
            }
        }


        void UpdateLapUI()
        {
            if(racePanel != null)
            {
                racePanel.UpdateLapText();
            }
        }


        public void HideAllPanels()
{
    if (preRacePanel != null)
        preRacePanel.gameObject.SetActive(false);
    if (racePanel != null)
        racePanel.gameObject.SetActive(false);
    if (pausePanel != null)
        pausePanel.gameObject.SetActive(false);
    if (postRacePanel != null)
        postRacePanel.gameObject.SetActive(false);
    if (replayPanel != null)
        replayPanel.gameObject.SetActive(false);
}


        public void SelectButtonInPanel(GameObject panel)
        {
			//Select the first button in the panel
            Selectable[] selectables = panel.GetComponentsInChildren<Selectable>();
            if (selectables.Length == 0)
                return;

            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(selectables[0].gameObject);
            selectables[0].OnSelect(new BaseEventData(EventSystem.current));
        }


        void OnEnable()
        {
            RaceManager.OnRacePositionsChange += UpdateRaceStandingsUI;
            RaceManager.OnPlayerFinish += UpdateRaceStandingsUI;
            RaceManager.OnVehicleSpawn += UpdateLapUI;
            RaceManager.OnRaceStateChange += UpdateUIBasedOnRaceState;
            RacerStatistics.OnEnterNewLap += UpdateLapUI;
        }


        void OnDisable()
        {
            RaceManager.OnRacePositionsChange -= UpdateRaceStandingsUI;
            RaceManager.OnPlayerFinish -= UpdateRaceStandingsUI;
            RaceManager.OnVehicleSpawn -= UpdateLapUI;
            RaceManager.OnRaceStateChange -= UpdateUIBasedOnRaceState;
            RacerStatistics.OnEnterNewLap -= UpdateLapUI;
        }
    }
}
