using UnityEngine;
using System.Collections;

namespace RGSK
{
    public class RaceTypeObjects : MonoBehaviour
    {
        public RTObjects[] raceTypeObjects;

        void Start()
        {
            if (RaceManager.instance == null)
                return;

            for (int i = 0; i < raceTypeObjects.Length; i++)
            {
                //Disable all gameObjects
                DisableAllObjects(raceTypeObjects[i].gameObjects);

                //Enable the gameObjects for the race type
                if(RaceManager.instance.raceType == raceTypeObjects[i].raceType)
                {
                    EnableAllObjects(raceTypeObjects[i].gameObjects);
                }
            }
        }


        void EnableAllObjects(GameObject[] gos)
        {
            //Enable all the gameObjects in the array
            foreach (GameObject go in gos)
            {
                go.SetActive(true);
            }
        }


        void DisableAllObjects(GameObject[] gos)
        {
            //Disable all the gameObjects in the array
            foreach (GameObject go in gos)
            {
                go.SetActive(false);
            }
        }


        [System.Serializable]
        public class RTObjects
        {
            public RaceType raceType;
            public GameObject[] gameObjects;
        }
    }
}