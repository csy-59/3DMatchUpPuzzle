using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum GameState
{
    Ready,
    Search,
    Hunt,
    Result
}

public class GameManager : MonoBehaviour
{
    [Header("GameTime")]
    [SerializeField] private int searchTime;
    private int second = -1;
    private float elapsedTime = 0f;
    public UnityEvent<int> OnGameTimeSpend { get; private set; } = new UnityEvent<int>();

    [Header("GameState")]
    [SerializeField] private GameState currentState;
    public GameState CurrentState => currentState;

    public UnityEvent OnGameReady { get; private set; } = new UnityEvent();
    public UnityEvent OnGameSearch { get; private set; } = new UnityEvent();
    public UnityEvent OnGameHunt { get; private set; } = new UnityEvent();
    public UnityEvent OnGameEnd { get; private set; } = new UnityEvent();

    [Header("Animals")]
    [SerializeField] private GameObject[] animals;
    public GameObject[] Animals => animals;
    private AnimalData[] animalDatas;
    public AnimalData[] AnimalDatas => animalDatas;

    [Header("Manager")]
    [SerializeField] private SearchManager searchManager;
    [SerializeField] private HuntingManager huntingManager;

    private void Start()
    {
        animalDatas = new AnimalData[animals.Length];
        int index = 0;
        foreach(var ani in animals)
        {
            animalDatas[index] = ani.GetComponent<AnimalData>();
            ++index;
        }

        searchManager.OnTargetFound?.RemoveListener(OnTargetFound);
        searchManager.OnTargetFound?.AddListener(OnTargetFound);

        currentState = GameState.Ready;
        OnGameReady?.Invoke();
    }

    public void OnTargetTracked()
    {
        if(currentState == GameState.Ready)
        {
            currentState = GameState.Search;
            OnGameSearch?.Invoke();
        }
    }

    private void Update()
    {
        if(currentState == GameState.Search)
        {
            elapsedTime += Time.deltaTime;
            if (second != (int)elapsedTime)
            {
                second = (int)elapsedTime;
                OnGameTimeSpend?.Invoke(second);
            }

            if (second >= searchTime)
            {
                currentState = GameState.Result;
                OnGameEnd?.Invoke();
                ShowResult();
            }
        }

    }

    private void OnTargetFound(AnimalData _data)
    {
        currentState = GameState.Hunt;
        OnGameHunt?.Invoke();
        huntingManager.SetHuntingScene(_data);
        huntingManager.StartHunting();
    }

    private void ShowResult()
    {

    }
}
