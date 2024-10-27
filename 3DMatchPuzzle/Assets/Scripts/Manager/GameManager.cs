using System.Collections;
using System.Collections.Generic;
using System.Security.Policy;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 게임을 총괄하는 매니저
/// </summary>
public partial class GameManager : MonoBehaviour
{
    public enum GameState
    {
        None = -1, // 상태 없음

        Run, // 인게임 - 인식 후 게임 동작
        Lost, // 인게임 - 게임 동작 중 타겟 잃음
        Pause, // 인게임 - 게임 멈춤
        Win, // 인게임 - 승리
        Lose, // 인게임 - 패배
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


    private void OnPuzzleDetacted(bool _isSameAsCurrentPuzzle)
    {
        // 게임 도중, 혹은 일시정지 중에 잃었다가 다시 잡을 수 있음
        if(CurrentGameState == GameState.Lost)
        {
            CurrentGameState = PrevGameState;
        }
    }

    private void OnPuzzleLost()
    {
        if(CurrentGameState == GameState.Run || 
            CurrentGameState == GameState.Pause)
        {
            CurrentGameState = GameState.Lost;
        }
    }
}

