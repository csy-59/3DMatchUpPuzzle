using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class TitleUIManager : MonoBehaviour
{
    [SerializeField] private Button goToTrophyBtn;
    public UnityEvent OnClickTrophyBtn { get; private set; } = new UnityEvent();

    [SerializeField] private GameManager gameManager;

    private void Start()
    {
        goToTrophyBtn.onClick.RemoveAllListeners();
        goToTrophyBtn.onClick.AddListener(GoToTrophy);

        LocalDataManager.Instance.DatakInitialize();
        PlayerPrebsManager.Instance.Init();
        PuzzleManager.Instance.LoadPuzzle();

        PuzzleManager.Instance.OnPuzzleDetactedEvent?.RemoveListener(OnPuzzleDetected);
        PuzzleManager.Instance.OnPuzzleDetactedEvent?.AddListener(OnPuzzleDetected);

        gameManager.OnGameEnd.AddListener(() => gameObject.SetActive(true));
    }

    private void OnEnable()
    {
        PuzzleManager.Instance.ResetPuzzleAll();
        gameManager.ResetGame();
    }

    private void OnPuzzleDetected(bool _)
    {
        gameObject.SetActive(false);
    }

    private void GoToTrophy()
    {
        OnClickTrophyBtn?.Invoke();
        gameObject.SetActive(false);
    }
}
