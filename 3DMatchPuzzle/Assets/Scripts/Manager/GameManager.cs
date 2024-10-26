using System.Collections;
using System.Collections.Generic;
using System.Security.Policy;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// ������ �Ѱ��ϴ� �Ŵ���
/// </summary>
public partial class GameManager : SingletonBehavior<GameManager>
{
    #region GameState
    [SerializeField] private GameState currentGameState;
    public GameState CurrentGameState
    {
        get { return currentGameState; }
        private set
        {
            PrevGameState = currentGameState;
            currentGameState = value;
            OnGameStateChanged?.Invoke(value);
        }
    }
    [field: SerializeField] public GameState PrevGameState { get; private set; }

    public UnityEvent<GameState> OnGameStateChanged { get; private set; } = new UnityEvent<GameState>();
    #endregion

    #region Managers
    [Header("Managers")]
    [SerializeField] private SmallManagerBase[] managerList;
    private Dictionary<string, SmallManagerBase> managerDict = new Dictionary<string, SmallManagerBase>();
    #endregion

    protected override void Init()
    {
        base.Init();

        foreach (var manager in managerList)
        {
            managerDict.Add(manager.GetType().Name, manager);
        }

        GetManager<PuzzleManager>().OnPuzzleDetactedEvent?.RemoveListener(OnPuzzleDetacted);
        GetManager<PuzzleManager>().OnPuzzleDetactedEvent?.AddListener(OnPuzzleDetacted);
    }

    private void Start()
    {
        ResetGame();
    }

    private void ResetGame()
    {
        CurrentGameState = GameState.Title;
    }


    private void OnPuzzleDetacted(bool _isSameAsCurrentPuzzle)
    {
        if(CurrentGameState == GameState.Title)
        {
            CurrentGameState = GameState.InGame_run;
        }
        
        // ���� ����, Ȥ�� �Ͻ����� �߿� �Ҿ��ٰ� �ٽ� ���� �� ����
        else if(CurrentGameState == GameState.InGame_lost)
        {
            CurrentGameState = PrevGameState;
        }
    }

    private void OnPuzzleLost()
    {

    }










    private T GetManager<T>() where T : SmallManagerBase
    {
        SmallManagerBase returnManager;
        if (managerDict.TryGetValue(typeof(T).Name, out returnManager) == false)
        {
            Debug.LogError($"{typeof(T).Name}�� ���� �Ŵ����� ��ϵǾ� ���� ����!");
            return null;
        }

        return returnManager as T;
    }
}

/// <summary>
/// ���ӿ� �ʿ��� ����� ����
/// </summary>
public partial class GameManager : SingletonBehavior<GameManager>
{
    public enum GameState
    {
        None = -1, // ���� ����

        Title, // ���� ȭ��

        InGame_run, // �ΰ��� - �ν� �� ���� ����
        InGame_lost, // �ΰ��� - ���� ���� �� Ÿ�� ����
        InGame_pause, // �ΰ��� - ���� ����
        InGame_win, // �ΰ��� - �¸�
        InGame_lose, // �ΰ��� - �й�

        Trophy, // Ʈ���� â
    }
}

