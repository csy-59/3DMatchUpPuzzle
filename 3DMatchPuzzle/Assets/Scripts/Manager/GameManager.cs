using System.Collections;
using System.Collections.Generic;
using System.Security.Policy;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// ������ �Ѱ��ϴ� �Ŵ���
/// </summary>
public partial class GameManager : MonoBehaviour
{
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


    private void OnPuzzleDetacted(bool _isSameAsCurrentPuzzle)
    {
        // ���� ����, Ȥ�� �Ͻ����� �߿� �Ҿ��ٰ� �ٽ� ���� �� ����
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

