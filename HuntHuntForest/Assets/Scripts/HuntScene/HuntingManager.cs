using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

public class HuntingManager : MonoBehaviour
{ 
    [Serializable] 
    private class AnimalPrefab
    {
        public AnimalType Type;
        public GameObject Prefab;
    }

    [Header("Animals")]
    [SerializeField] private AnimalPrefab[] animalPrefabs;
    private Dictionary<AnimalType, GameObject> animalDictionary = new Dictionary<AnimalType, GameObject>();
    private AnimalData animalData;
    private GameObject animalObj;
    private AnimalAttack attack;
    private AnimalHit animalHit;
    private bool isReady = false;

    [Header("Player")]
    [SerializeField] private CharacterHit playerHit;
    [SerializeField] private CharacterAttack playerAttack;

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

    [Header("Attack")]
    [SerializeField] private int playerMaxAttackCount = 3;
    [SerializeField] private float attackStartOffsetTime = 1f;
    private WaitForSeconds waitForAttackStart;
    private int playerAttackCount;

    [Header("HuntEnd")]
    [SerializeField] private float returnToSearchTime;
    private WaitForSeconds waitForReturnToSearch;

    private void Start()
    {
        Initialize();

        // 테스트
        SetHuntingScene(animalDictionary[AnimalType.Pudu].GetComponent<AnimalData>());
        StartHunting();
    }

    private void Initialize()
    {
        foreach (var anim in animalPrefabs)
        {
            animalDictionary[anim.Type] = anim.Prefab;
        }

        waitForOneSecond = new WaitForSeconds(1f);
        waitForReturnToSearch = new WaitForSeconds(returnToSearchTime);
        waitForAttackStart = new WaitForSeconds(attackStartOffsetTime);

        playerHit.OnHitFruit.AddListener((SliceableFruit _) => OnAnimalAttackEnd());
        playerAttack.OnCharacterAttackEnd?.RemoveListener(OnPlayerAttackEnd);
        playerAttack.OnCharacterAttackEnd?.AddListener(OnPlayerAttackEnd);

        ResetData();
    }

    public void SetHuntingScene(AnimalData _data)
    {
        animalData = _data;
        foreach(var anim in animalPrefabs)
        {
            anim.Prefab.gameObject.SetActive(false);
        }
        animalObj = animalDictionary[_data.Type];
        animalObj.gameObject.SetActive(true);
        attack = animalObj.GetComponent<AnimalAttack>();
        animalHit = animalObj.GetComponent<AnimalHit>();

        animalHit.OnHit?.RemoveListener(OnAnimalHit);
        animalHit.OnHit?.AddListener(OnAnimalHit);

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

        CurrentState = HuntState.Defence;
        StartCoroutine(CoDefenceTurn());
    }

    private IEnumerator CoDefenceTurn()
    {
        Debug.Log("방어 턴 시작");
        for(int i = 0; i< animalData.AttackCount; ++i)
        {
            float waitTime = UnityEngine.Random.Range(minAttackCoolTime, maxAttackCoolTime);
            yield return new WaitForSeconds(waitTime);
            var fruit = attack.Throw();
            fruit.OnFruitSliced.AddListener(OnAnimalAttackEnd);
        }
    }


    private void OnAnimalAttackEnd()
    {
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
        yield return waitForAttackStart;

        Debug.Log("공격 시작");
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
        if(animalHit.CurrentHealth <= 0)
        {
            currentState = HuntState.Success;
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
