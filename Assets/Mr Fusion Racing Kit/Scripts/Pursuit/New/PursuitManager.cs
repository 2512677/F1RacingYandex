using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PursuitManager : MonoBehaviour
{
    [Header("Variables")]
    public bool PoliceIsSearchingForCar;    // Полиция ищет игрока
    public bool IsInPursuit;               // Идёт активная погоня
    public float ChaseLevel;               // «Уровень розыска»
    public float ArrestEscapeStatus;       // (не используется прямо)

    private bool IsArrested;
    private bool IsEscaped;
    private float EscapeTimer;             // (не используется прямо)

    public int BrokenObjectsTotal;             // Сколько объектов игрок разрушил
    public int BrokenObjectsSincePursuitStart;  // (не используется прямо)
    public int CurrentCopsInChase;              // (не используется прямо)
    public bool CopsCanSpawn;                   // (не используется прямо)
    public int DestroyedCops;                   // Сколько полицейских машин уничтожено игроком

    private int XP;                     // Начисляемый опыт
    public float ArrestTimer;           // (не используется прямо)
    public bool HelicopterInChase;      // (не используется прямо)

    private bool PursuitResultEvenntTriggert;
    private GameObject[] Cops;

    public GameObject Helicopter;       // Префаб вертолёта (если нужен)
    public List<Transform> AvaiblePursuitTargets = new List<Transform>();

    [Header("Reference Stuff")]
    public Image EscapeBar;              // UI Image (Filled) для полоски побега
    public CanvasGroup EscapeBarGO;      // CanvasGroup контейнер для EscapeBar
    public CanvasGroup EscapeVignette;   // (необязательно) эффект виньетки при побеге
    public CanvasGroup EscapedResults;   // Панель «Вы сбежали»
    public CanvasGroup ArrestedBar;      // Панель «Вы арестованы»
    public Image ArrestBarImage;         // UI Image (Filled) для полоски ареста
    public Animation ArrestedTextAnim;   // Анимация текста «Вы арестованы»
    public Animation EscapedTextAnim;    // Анимация текста «Вы сбежали»
    public CanvasGroup MainUI;           // Основной HUD, который скрывается при результатах

    public AudioSource MPS;              // AudioSource для музыки погони
    public AudioClip ChaseMusic;         // Клип «Музыка погони»

  
   
  

    private float EscapeTimeReducer;
    private bool MusicPlays;
    private bool EscapeIntroEffectHasTriggert;
    private int RandomInt;

    public CanvasGroup EndPursuitScreen; // (опционально) экран конца погони

  

    private bool PursuitStartEffectsHasStarted;

    private void Start()
    {
        

        Randomize();
    }

    private void OnTriggerEnter(Collider target)
    {
        // Меняем проверку тега на "Player"
        if (target.CompareTag("Player") && PoliceIsSearchingForCar)
        {
            IsInPursuit = true;
           
        }
    }

    private void FixedUpdate()
    {
        // Корректируем скорость заполнения бара побега в зависимости от ChaseLevel
        if (ChaseLevel <= 100f) EscapeTimeReducer = 0.0003f;
        else if (ChaseLevel <= 200f) EscapeTimeReducer = 0.00025f;
        else if (ChaseLevel <= 300f) EscapeTimeReducer = 0.0002f;

       

        if (!IsInPursuit)
        {
          

            if (BrokenObjectsTotal <= 20)
                PoliceIsSearchingForCar = false;
            else
                PoliceIsSearchingForCar = true;

            return;
        }

        if (!PursuitStartEffectsHasStarted)
        {
          
            PursuitStartEffectsHasStarted = true;
          
            // Логика спавна копов через TrafficController удалена
        }


        if (!IsArrested && !IsEscaped)
        {
          
            {
                ArrestedBar.alpha += 0.02f;
                ArrestBarImage.fillAmount += 0.003f;
            }
            
            {
                ArrestedBar.alpha -= 0.02f;
                ArrestBarImage.fillAmount -= 0.002f;
            }

            ChaseLevel += 0.02f;

            if (!MusicPlays)
            {
                PoliceIsSearchingForCar = false;
               
                MPS.clip = ChaseMusic;
                MPS.Play();
                
                MusicPlays = true;
            }

        
            {
                if (!EscapeIntroEffectHasTriggert)
                    EscapeIntroEffectHasTriggert = true;

                EscapeBarGO.alpha += 0.02f;
                EscapeBar.fillAmount += EscapeTimeReducer;
            }
            
            {
                EscapeBar.fillAmount = 0f;
                EscapeBarGO.alpha -= 0.02f;
                EscapeIntroEffectHasTriggert = false;
            }
        }

        if (EscapeBar.fillAmount >= 1f)
        {
            IsEscaped = true;
            IsArrested = false;

            if (!PursuitResultEvenntTriggert)
            {
                PursuitResultEvenntTriggert = true;
                StartCoroutine("EscapedEvent");
            }
        }

        if (ArrestBarImage.fillAmount >= 1f)
        {
            IsEscaped = false;
            IsArrested = true;

            if (!PursuitResultEvenntTriggert)
            {
                PursuitResultEvenntTriggert = true;
                StartCoroutine("ArrestedEvent");
            }
        }
    }

    private void Update()
    {
        // Здесь можно добавить принудительную остановку игрока или дополнительные эффекты,
        // но в упрощённой версии мы этого не делаем.
    }

    public IEnumerator EscapedEvent()
    {
        IsInPursuit = false;
        PoliceIsSearchingForCar = false;

        yield return new WaitForSeconds(0.01f);
        EscapedTextAnim.Play();

        yield return new WaitForSeconds(1.5f);
        EscapedResults.alpha = 1f;
        EscapedResults.interactable = true;
        EscapedResults.blocksRaycasts = true;

        MainUI.alpha = 0f;
        MainUI.blocksRaycasts = false;
        MainUI.interactable = false;

        EscapeBarGO.alpha = 0f;
        PursuitStartEffectsHasStarted = false;

        calcxptoadd();
      
    }

    private void calcxptoadd()
    {
        XP = DestroyedCops * 30 + BrokenObjectsTotal * 5;
    }

    public IEnumerator ArrestedEvent()
    {
        IsInPursuit = false;
        PoliceIsSearchingForCar = false;

        yield return new WaitForSeconds(0.5f);
        ArrestedTextAnim.Play();

        PlayerPrefs.SetInt("Arrested", 1);

        yield return new WaitForSeconds(3f);
      
    }

    public void OkAfterEscape()
    {
        EscapedResults.alpha = 0f;
        EscapedResults.interactable = false;
        EscapedResults.blocksRaycasts = false;

        MainUI.alpha = 1f;
        MainUI.blocksRaycasts = true;
        MainUI.interactable = true;

        Cops = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject obj in Cops)
            Destroy(obj);

        Cops = GameObject.FindGameObjectsWithTag("CopDummy");
        foreach (GameObject obj in Cops)
            Destroy(obj);

        Cops = GameObject.FindGameObjectsWithTag("Nailbar");
        foreach (GameObject obj in Cops)
            Destroy(obj);
    }

    private IEnumerator CountTime()
    {
        if (RandomInt > 2 && !HelicopterInChase && IsInPursuit && ChaseLevel >= 290f)
        {
         
            HelicopterInChase = true;
            Instantiate(Helicopter, transform.position, transform.rotation);
        }
        yield return new WaitForSeconds(90f);
        Randomize();
    }

    private void Randomize()
    {
        RandomInt = Random.Range(0, 4);
        StartCoroutine(CountTime());
    }
}
