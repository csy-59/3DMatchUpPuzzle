using System.Collections;
using System.Collections.Generic;
using System.Security.Policy;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 게임을 총괄하는 매니저
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
        
        // 게임 도중, 혹은 일시정지 중에 잃었다가 다시 잡을 수 있음
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
            Debug.LogError($"{typeof(T).Name}에 대한 매니저가 등록되어 있지 않음!");
            return null;
        }

        return returnManager as T;
    }
}

/// <summary>
/// 게임에 필요한 선언들 모음
/// </summary>
public partial class GameManager : SingletonBehavior<GameManager>
{
    public enum GameState
    {
        None = -1, // 상태 없음

        Title, // 시작 화면

        InGame_run, // 인게임 - 인식 후 게임 동작
        InGame_lost, // 인게임 - 게임 동작 중 타겟 잃음
        InGame_pause, // 인게임 - 게임 멈춤
        InGame_win, // 인게임 - 승리
        InGame_lose, // 인게임 - 패배

        Trophy, // 트로피 창
    }
}

