using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PuzzleManager : SmallManagerBase
{
    [Header("Puzzles")]
    [SerializeField] private DefaultObserverEventHandler[] puzzleImageEvents;
    private DefaultObserverEventHandler currentPuzzle; // 현재 플레이 중인 퍼즐
    private DefaultObserverEventHandler detactedPuzzle; // 새로 감지될 퍼즐(currentPuzzle의 후보)

    public UnityEvent<bool> OnPuzzleDetactedEvent { get; private set; } = new UnityEvent<bool>();
    public UnityEvent OnPuzzleLostEvent { get; private set; } = new UnityEvent();

    [Header("Cube")]
    [SerializeField] private DefaultObserverEventHandler cubeImageEvent;

    private void Start()
    {
        currentPuzzle = null;
        foreach(var e in puzzleImageEvents)
        {
            e.OnTargetFound.AddListener(() => OnPuzzleDetacted(e));
        }
    }

    private void OnPuzzleDetacted(DefaultObserverEventHandler _puzzle)
    {
        if (currentPuzzle != null)
            return;

        detactedPuzzle = _puzzle;
        OnPuzzleDetactedEvent?.Invoke(currentPuzzle == _puzzle);
    }

    private void OnPuzzleLost(DefaultObserverEventHandler _puzzle)
    {
        if (currentPuzzle == null)
            return;

        OnPuzzleLostEvent?.Invoke();
    }

    protected override void OnTitleState()
    {
        currentPuzzle = null;

        foreach(var p in puzzleImageEvents)
        {
            p.gameObject.SetActive(true);
        }
    }

    protected override void OnGameRunState()
    {
        currentPuzzle = detactedPuzzle;
        foreach (var p in puzzleImageEvents)
        {
            p.gameObject.SetActive(p == currentPuzzle);
        }
    }

    protected override void OnTrophyState()
    {
    }
}
