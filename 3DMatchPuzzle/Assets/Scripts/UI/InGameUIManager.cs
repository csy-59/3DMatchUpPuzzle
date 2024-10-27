using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InGameUIManager : MonoBehaviour
{
    [Header("Lost Target")]
    [SerializeField] private Transform lostTargetTxt;

    [Header("Pause")]
    [SerializeField] private Transform pauseBtn;
    [SerializeField] private Transform pausePanel;

    [Header("Result")]
    [SerializeField] private Transform winPanel;
    [SerializeField] private Transform losePanel;

    [Header("Time")]
    [SerializeField] private TextMeshProUGUI timeTxt;
    [SerializeField] private Slider timeSlider;

    [Header("ECT")]
    [SerializeField] private GameManager gameManager;

    private void Awake()
    {
        gameManager.OnGameStateChanged?.RemoveListener(OnGameStateChanged);
        gameManager.OnGameStateChanged?.AddListener(OnGameStateChanged);

        gameManager.OnSecondTicked?.RemoveListener(ShowRemainTime);
        gameManager.OnSecondTicked?.AddListener(ShowRemainTime);
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

            case GameManager.GameState.None:    OnGameReset(); break;
            default:
                break;
        }
    }

    private void OnGameReset()
    {
        gameObject.SetActive(false);
    }

    private  void OnGameRunState()
    {
        gameObject.SetActive(true);

        pauseBtn.gameObject.SetActive(true);
        pausePanel.gameObject.SetActive(false);

        lostTargetTxt.gameObject.SetActive(false);

        winPanel.gameObject.SetActive(false);
        losePanel.gameObject.SetActive(false);
    }

    private void OnGameLostState()
    {
        pauseBtn.gameObject.SetActive(false);
        pausePanel.gameObject.SetActive(false);

        lostTargetTxt.gameObject.SetActive(true);

        winPanel.gameObject.SetActive(false);
        losePanel.gameObject.SetActive(false);
    }

    private void OnGamePauseState()
    {
        pauseBtn.gameObject.SetActive(false);
        pausePanel.gameObject.SetActive(true);

        lostTargetTxt.gameObject.SetActive(false);

        winPanel.gameObject.SetActive(false);
        losePanel.gameObject.SetActive(false);
    }
    private void OnGameWinState()
    {
        pauseBtn.gameObject.SetActive(false);
        pausePanel.gameObject.SetActive(false);

        lostTargetTxt.gameObject.SetActive(false);

        winPanel.gameObject.SetActive(true);
        losePanel.gameObject.SetActive(false);
    }
    private void OnGameLoseState()
    {
        pauseBtn.gameObject.SetActive(false);
        pausePanel.gameObject.SetActive(false);

        lostTargetTxt.gameObject.SetActive(false);

        winPanel.gameObject.SetActive(false);
        losePanel.gameObject.SetActive(true);
    }

    private void ShowRemainTime(float _remainTime, float _maxTime)
    {
        int second = (int)_remainTime % 60;
        int min = (int)_remainTime / 60;
        timeTxt.SetText($"{min}:{second}");
        timeSlider.value = _remainTime / _maxTime;
    }
}
