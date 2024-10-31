using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

public class HuntingManager : MonoBehaviour
{
    //[Header("Animals")]
    private GameObject[] animalPrefabs;
    private AnimalData[] animalDatas;
    private Dictionary<AnimalType, GameObject> animalDictionary = new Dictionary<AnimalType, GameObject>();
    private int[] animalHuntedCounts;
    public int[] AniamlHuntedCounts => animalHuntedCounts;
    private AnimalData animalData;
    private GameObject animalObj;
    private AnimalAttack attack;
    public AnimalHit AnimalHit { get; private set; }
    private bool isReady = false;
    [Header("Manager")]
    [SerializeField] private GameManager gameManager;

    [Header("Player")]
    [SerializeField] private CharacterHit playerHit;
    [SerializeField] private CharacterAttack playerAttack;
    public CharacterHit PlayerHit { get; private set; }

    public enum HuntState
    {
        Ready,
        Defence,
        Attack,
        Success,
        Fail,
        Return,
    }

    [Header("State")]
    [SerializeField] private HuntState currentState; 
    public UnityEvent OnHuntStateChanged { get; private set; } = new UnityEvent();
    public HuntState CurrentState
    {
        get { return currentState; }
        private set
        {
            currentState = value;
            OnHuntStateChanged?.Invoke();
        }
    }

    [Header("Ready")]
    [SerializeField] private int readyTime;
    private WaitForSeconds waitForOneSecond;
    public UnityEvent<int> OnReayTimePassed { get; private set; } = new UnityEvent<int>();

    [Header("Defence")]
    [SerializeField] private float minAttackCoolTime;
    [SerializeField] private float maxAttackCoolTime;
    private int animalAttackCount;
    public UnityEvent OnDefenceStart { get; private set; } = new UnityEvent();

    [Header("Attack")]
    [SerializeField] private int playerMaxAttackCount = 3;
    [SerializeField] private float attackStartOffsetTime = 1f;
    private WaitForSeconds waitForAttackStart;
    private int playerAttackCount;
    public UnityEvent OnAttackStart { get; private set; } = new UnityEvent();

    [Header("HuntEnd")]
    [SerializeField] private float returnToSearchTime;
    private WaitForSeconds waitForReturnToSearch;

    private void Start()
    {
        Initialize();
    }

    public void Test()
    {
        SetHuntingScene(animalDictionary[AnimalType.Pudu].GetComponent<AnimalData>());
        StartHunting();
    }

    private void Initialize()
    {
        animalPrefabs = gameManager.Animals;
        animalDatas = gameManager.AnimalDatas;
        animalHuntedCounts = new int[animalPrefabs.Length];
        for(int i = 0; i< animalPrefabs.Length; ++i)
        {
            animalDictionary[animalDatas[i].Type] = animalPrefabs[i];
            animalHuntedCounts[i] = 0;
        }

        waitForOneSecond = new WaitForSeconds(1f);
        waitForReturnToSearch = new WaitForSeconds(returnToSearchTime);
        waitForAttackStart = new WaitForSeconds(attackStartOffsetTime);

        playerHit.OnHitFruit?.RemoveListener(OnAnimalAttackEnd);
        playerHit.OnHitFruit?.AddListener(OnAnimalAttackEnd);
        playerAttack.OnCharacterAttackEnd?.RemoveListener(OnPlayerAttackEnd);
        playerAttack.OnCharacterAttackEnd?.AddListener(OnPlayerAttackEnd);

        PlayerHit = playerHit;

        ResetData();
    }

    public void SetHuntingScene(AnimalData _data)
    {
        animalData = _data;
        foreach(var anim in animalPrefabs)
        {
            anim.SetActive(false);
        }
        animalObj = animalDictionary[_data.Type];
        animalObj.gameObject.SetActive(true);
        attack = animalObj.GetComponent<AnimalAttack>();
        AnimalHit = animalObj.GetComponent<AnimalHit>();

        AnimalHit.OnHit?.RemoveListener(OnAnimalHit);
        AnimalHit.OnHit?.AddListener(OnAnimalHit);

        CurrentState = HuntState.Ready;

        isReady = true;
    }

    public void StartHunting()
    {
        if (isReady == false)
        {
            Debug.LogError("초기화 되지 않음");
            return;
        }

        StartCoroutine(CoHuntReady());
    }

    private IEnumerator CoHuntReady()
    {
        for(int i = 0; i < readyTime; ++i)
        {
            yield return waitForOneSecond;
            OnReayTimePassed?.Invoke(readyTime - i);
            Debug.Log(readyTime - i);
        }

        yield return waitForOneSecond;
        OnReayTimePassed?.Invoke(0);

        StartCoroutine(CoDefenceTurn());
    }

    private IEnumerator CoDefenceTurn()
    {
        Debug.Log("방어 턴 시작");
        OnDefenceStart?.Invoke();
        yield return waitForOneSecond;

        CurrentState = HuntState.Defence;
        for (int i = 0; i< animalData.AttackCount; ++i)
        {
            float waitTime = UnityEngine.Random.Range(minAttackCoolTime, maxAttackCoolTime);
            yield return new WaitForSeconds(waitTime);
            var fruit = attack.Throw();
            fruit.OnFruitDetroy.AddListener(OnAnimalAttackEnd);
        }
    }


    private void OnAnimalAttackEnd()
    {
        if (CurrentState != HuntState.Defence)
        {
            animalAttackCount = 0;
            return;
        }

        ++animalAttackCount;

        // 사냥 실패
        if(playerHit.CurrentHealth <= 0)
        {
            Debug.Log("사냥 실패");
            CurrentState = HuntState.Fail;
            animalAttackCount = 0;
            OnHuntingEnd();
            return;
        }

        // 공격 종료
        if(animalAttackCount >= animalData.AttackCount)
        {
            Debug.Log("방어 종료");
            animalAttackCount = 0;
            StartCoroutine(CoStartAttack());
        }
    }

    private IEnumerator CoStartAttack()
    {

        Debug.Log("공격 시작");
        OnAttackStart?.Invoke();
        yield return waitForAttackStart;

        CurrentState = HuntState.Attack;
    }

    private void OnPlayerAttackEnd()
    {
        ++playerAttackCount;

        // 공격 종료
        if (playerAttackCount >= playerMaxAttackCount
            && CurrentState != HuntState.Success && CurrentState != HuntState.Fail)
        {
            Debug.Log("공격 종료");
            CurrentState = HuntState.Defence;
            playerAttackCount = 0;
            StartCoroutine(CoDefenceTurn());
        }
    }
    
    private void OnAnimalHit()
    {
        if(AnimalHit.CurrentHealth <= 0)
        {
            Debug.Log("사냥 성공");
            ++animalHuntedCounts[(int)animalData.Type];
            CurrentState = HuntState.Success;
            playerAttackCount = 0;
            OnHuntingEnd();
        }
    }

    private void OnHuntingEnd()
    {
        StopAllCoroutines();
        StartCoroutine(CoHuntEnd());
    }

    private IEnumerator CoHuntEnd()
    {
        yield return waitForReturnToSearch;

        ResetData();

        CurrentState = HuntState.Return;
    }

    private void ResetData()
    {
        animalData = null;
        animalObj = null;
        attack = null;

        animalAttackCount = 0;
        playerAttackCount = 0;

        isReady = false;
    }
}
