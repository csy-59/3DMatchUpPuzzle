using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class TitleUIManager : MonoBehaviour
{
    [SerializeField] private Button goToTrophyBtn;
    public UnityEvent OnClickTrophyBtn { get; private set; } = new UnityEvent();

    private void Start()
    {
        goToTrophyBtn.onClick.RemoveAllListeners();
        goToTrophyBtn.onClick.AddListener(GoToTrophy);

        LocalDataManager.Instance.DatakInitialize();
        PlayerPrebsManager.Instance.Init();
        PuzzleManager.Instance.LoadPuzzle();
    }

    private void GoToTrophy()
    {
        OnClickTrophyBtn?.Invoke();
        gameObject.SetActive(false);
    }
}
