using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SmallManagerBase : MonoBehaviour
{
    private void Awake()
    {
        GameManager.Instance.OnGameStateChanged?.RemoveListener(OnGameStateChanged);
        GameManager.Instance.OnGameStateChanged?.AddListener(OnGameStateChanged);
    }

    private void OnGameStateChanged(GameManager.GameState _state)
    {
        switch(_state)
        {
            case GameManager.GameState.Title:           OnTitleState(); break;

            case GameManager.GameState.InGame_run:      OnGameRunState(); break;
            case GameManager.GameState.InGame_lost:     OnGameLostState(); break;
            case GameManager.GameState.InGame_pause:    OnGamePauseState(); break;
            case GameManager.GameState.InGame_win:      OnGameWinState(); break;
            case GameManager.GameState.InGame_lose:     OnGameLoseState(); break;
            
            case GameManager.GameState.Trophy:          OnTrophyState(); break;

            default:
                break;
        }
    }

    protected abstract void OnTitleState();

    protected abstract void OnGameRunState();
    protected virtual void OnGameLostState() { }
    protected virtual void OnGamePauseState() { }
    protected virtual void OnGameWinState() { }
    protected virtual void OnGameLoseState() { }

    protected abstract void OnTrophyState();

}
