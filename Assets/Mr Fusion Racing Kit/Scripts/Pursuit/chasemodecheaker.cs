using System.Collections;
using UnityEngine;
using RGSK;               //  пространство имЄн RSKK (Racing Game Starter Kit)

public class chasemodecheaker : MonoBehaviour
{
    [Tooltip("ќбъект, который надо включить только в режиме Chase")]
    public GameObject chasemodeobject;

    void Start()
    {
        StartCoroutine(CheckRaceType());
    }

    private IEnumerator CheckRaceType()
    {
        // ждЄм, пока RaceManager по€витс€ в сцене
        while (RaceManager.instance == null)
            yield return null;

        // если реальный тип гонки = Chase Ч активируем объект
        bool isChase = RaceManager.instance.raceType == RaceType.Chase;

        if (chasemodeobject)
            chasemodeobject.SetActive(isChase);

        // сам себе больше не нужен
        enabled = false;
    }
}
