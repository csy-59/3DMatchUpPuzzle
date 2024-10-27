using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameUIManager : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;

    private void Awake()
    {
        gameManager.OnGameStateChanged?.RemoveListener(OnGameStateChanged);
        gameManager.OnGameStateChanged?.AddListener(OnGameStateChanged);
    }

    private void OnGameStateChanged(GameManager.GameState _state)
    {
        switch (_state)
        {
            case GameManager.GameState.Run:     OnGameRunState(); break;
            case GameManager.GameState.Lost:    OnGameLostState(); break;
            case GameManager.GameState.Pause:   OnGamePauseState(); break;
            case GameManager.GameState.Win:     OnGameWinState(); break;
            case GameManager.GameState.Lose:    OnGameLoseState(); break;
            default:
                break;
        }
    }

    protected virtual void OnGameRunState() { }
    protected virtual void OnGameLostState() { }
    protected virtual void OnGamePauseState() { }
    protected virtual void OnGameWinState() { }
    protected virtual void OnGameLoseState() { }

}
