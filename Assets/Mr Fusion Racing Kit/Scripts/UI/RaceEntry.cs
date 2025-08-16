using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

namespace RGSK
{
    public class RaceEntry : MonoBehaviour
    {
        public GameObject[] Entries;
        protected List<EntryInfo> raceEntry = new List<EntryInfo>();


        public void Setup()
        {
            for (int i = 0; i < Entries.Length; i++)
            {
                raceEntry.Add(new EntryInfo());
                SetupEntry(Entries[i], i);
            }
        }


        public void SetupEntry(GameObject entry, int index)
        {
            RaceEntryElement[] raceEntryElements = entry.GetComponentsInChildren<RaceEntryElement>();

            if (raceEntryElements.Length == 0)
                return;

            foreach (RaceEntryElement element in raceEntryElements)
            {
                switch (element.entryElement)
                {
                    case UIRaceEntryElement.Position:
                        raceEntry[index].position = element.GetComponent<Text>();
                        element.GetComponent<Text>().text = string.Empty;
                        break;

                    case UIRaceEntryElement.Name:
                        raceEntry[index].name = element.GetComponent<Text>();
                        element.GetComponent<Text>().text = string.Empty;
                        break;

                    case UIRaceEntryElement.Vehicle:
                        raceEntry[index].vehicle = element.GetComponent<Text>();
                        element.GetComponent<Text>().text = string.Empty;
                        break;

                    case UIRaceEntryElement.BestLap:
                        raceEntry[index].bestLap = element.GetComponent<Text>();
                        element.GetComponent<Text>().text = string.Empty;
                        break;

                    case UIRaceEntryElement.TotalTime:
                        raceEntry[index].totalTime = element.GetComponent<Text>();
                        element.GetComponent<Text>().text = string.Empty;
                        break;

                    case UIRaceEntryElement.Gap:
                        raceEntry[index].gap = element.GetComponent<Text>();
                        element.GetComponent<Text>().text = string.Empty;
                        break;

                    case UIRaceEntryElement.Points:
                        raceEntry[index].totalPoints = element.GetComponent<Text>();
                        element.GetComponent<Text>().text = string.Empty;
                        break;

                    case UIRaceEntryElement.Nationality:
                        raceEntry[index].nationality = element.GetComponent<Image>();
                        raceEntry[index].nationality.enabled = false;
                        break;

                    case UIRaceEntryElement.TotalSpeed:
                        raceEntry[index].speedtrapSpeed = element.GetComponent<Text>();
                        element.GetComponent<Text>().text = string.Empty;
                        break;
                }
            }
        }
    }

    [System.Serializable]
    public class EntryInfo
    {
        public Text position;
        public Text name;
        public Text vehicle;
        public Text bestLap;
        public Text totalTime;
        public Text gap;
        public Text totalPoints;
        public Text totalDistance;
        public Text totalSpeed;
        public Image nationality;
        public Text speedtrapSpeed;
    }
}