using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UIManager : MonoBehaviour
{
    [Header("Title")]
    [SerializeField] private Transform titlePanel;

    [Space]
    [Header("Game")]
    [SerializeField] private Transform gamePanel;

    [Space]
    [Header("Pause")]
    [SerializeField] private Transform pausePanel;

    [Space]
    [Header("Trophy")]
    [SerializeField] private Transform trophyPanel;

    public void StartGame()
    {
        SetGamePanelsActive(_game: true);
    }

    public void PauseGame()
    {
        SetGamePanelsActive(_pause: true);
    }

    public void GoToTrophyRoom()
    {
        SetGamePanelsActive(_trophy: true);
    }

    public void ReturnToTitle()
    {
        SetGamePanelsActive(_title: true);
    }

    private void SetGamePanelsActive(bool _title = false, bool _game = false, bool _pause = false, bool _trophy = false)
    {
        titlePanel.gameObject.SetActive(_title);
        gamePanel.gameObject.SetActive(_game);
        pausePanel.gameObject.SetActive(_pause);
        trophyPanel.gameObject.SetActive(_trophy);
    }
}
