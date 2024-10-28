using System.Collections;
using System.Collections.Generic;
using System.Security.Policy;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// ������ �Ѱ��ϴ� �Ŵ���
/// </summary>
public partial class GameManager : MonoBehaviour
{
    #region GameState
    public enum GameState
    {
        None = -1, // ���� ����

        Run, // �ΰ��� - �ν� �� ���� ����
        Lost, // �ΰ��� - ���� ���� �� Ÿ�� ����
        Pause, // �ΰ��� - ���� ����
        Win, // �ΰ��� - �¸�
        Lose, // �ΰ��� - �й�
    }

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
    public UnityEvent OnGameEnd { get; private set; } = new UnityEvent();
    #endregion


    #region GameTime
    private float maxPlayTime;
    private float elapsedTime;
    private int elapsedSeconds;
    [SerializeField] private float returnToHomeTime;
    private WaitForSeconds waitForReturnToHome;

    public UnityEvent<float, float> OnSecondTicked { get; private set; } = new UnityEvent<float, float>();
    #endregion


    private void Start()
    {
        PuzzleManager.Instance.OnPuzzleDetactedEvent?.RemoveListener(OnPuzzleDetacted);
        PuzzleManager.Instance.OnPuzzleDetactedEvent?.AddListener(OnPuzzleDetacted);

        PuzzleManager.Instance.OnPuzzleLostEvent?.RemoveListener(OnPuzzleLost);
        PuzzleManager.Instance.OnPuzzleLostEvent?.AddListener(OnPuzzleLost);

        waitForReturnToHome = new WaitForSeconds(returnToHomeTime);
    }

    public void ResetGame()
    {
        CurrentGameState = GameState.None;
        maxPlayTime = 0f;
        elapsedTime = 0f;
        elapsedSeconds = -1;
    }

    private void OnPuzzleDetacted(bool _isSameAsCurrentPuzzle)
    {
        if(GameState.None == CurrentGameState)
        {
            CurrentGameState = GameState.Run;
            StartGame();
        }
        // ���� ����, Ȥ�� �Ͻ����� �߿� �Ҿ��ٰ� �ٽ� ���� �� ����
        else if(CurrentGameState == GameState.Lost)
        {
            Time.timeScale = 1f;
            CurrentGameState = PrevGameState;
        }
    }

    private void OnPuzzleLost()
    {
        if(CurrentGameState == GameState.Run || 
            CurrentGameState == GameState.Pause ||
            CurrentGameState == GameState.Lost) // �߰��� ���������� ����
        {
            Time.timeScale = 0f;
            CurrentGameState = GameState.Lost;
        }
    }

    public void StartGame()
    {
        PuzzleManager.Instance.GameStart();
        var data = PuzzleManager.Instance.GetCurrentPuzzleData();
        if (data == null)
            Debug.LogError("PuzzleData �̻�");

        maxPlayTime = data.MaxPlayTime;
    }

    public void PauseGame()
    {
        Time.timeScale = 0f;
        CurrentGameState = GameState.Pause;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        CurrentGameState = GameState.Run;
    }

    public void GameEnd(bool _gameWin)
    {
        CurrentGameState = _gameWin ? GameState.Win : GameState.Lose;
        StartCoroutine(CoReturnToTitle());
    }

    private void Update()
    {
        if (CurrentGameState != GameState.Run)
            return;

        // ���� �ð� ǥ��
        if(maxPlayTime > 0f)
        {
            elapsedTime += Time.deltaTime;
            int newSecond = (int)elapsedTime;
            if(newSecond > elapsedSeconds)
            {
                OnSecondTicked?.Invoke((maxPlayTime - elapsedTime), maxPlayTime);
                elapsedSeconds = newSecond;
            }
            if(maxPlayTime - elapsedTime <=0 )
            {
                GameEnd(false);
            }
        }

        // ȭ�� ��ġ�� ���� �ǽ� ���̱�
        if (Input.GetMouseButton(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Debug.DrawRay(ray.origin, ray.direction * 1000, Color.blue);

            RaycastHit hit;
            if(Physics.Raycast(ray, out hit, 1000) == true)
            {
                CubeSide side = hit.transform.GetComponent<CubeSide>();
                if(side)
                {
                    side.Hit();
                }
            }
        }
    }

    private IEnumerator CoReturnToTitle()
    {
        yield return waitForReturnToHome;
        CurrentGameState = GameState.None;
        OnGameEnd?.Invoke();
    }
}

