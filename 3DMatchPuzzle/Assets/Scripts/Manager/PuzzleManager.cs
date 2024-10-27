using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PuzzleManager: SingletonBehaviour<PuzzleManager>
{
    [Header("Puzzles")]
    [SerializeField] private DefaultObserverEventHandler[] puzzleImageEvents;
    private List<PuzzleDataSetter> puzzleDatas;
    private DefaultObserverEventHandler currentPuzzle; // ���� �÷��� ���� ����
    private DefaultObserverEventHandler detactedPuzzle; // ���� ������ ����(currentPuzzle�� �ĺ�)

    public UnityEvent<bool> OnPuzzleDetactedEvent { get; private set; } = new UnityEvent<bool>();
    public UnityEvent OnPuzzleLostEvent { get; private set; } = new UnityEvent();

    [Header("Cube")]
    [SerializeField] private DefaultObserverEventHandler cubeImageEvent;

    protected override void Init()
    {
        base.Init();

        currentPuzzle = null;
        foreach (var e in puzzleImageEvents)
        {
            e.OnTargetFound.AddListener(() => OnPuzzleDetacted(e));
        }
    }

    public void LoadPuzzle()
    {
        int puzzleCount = LocalDataManager.Instance.PuzzleDatas.Count;
        puzzleDatas = new List<PuzzleDataSetter>(puzzleCount);

        foreach(var p in LocalDataManager.Instance.PuzzleDatas)
        {
            GameObject go = new GameObject(p.Name);
            go.AddComponent<PuzzleDataSetter>().Load(p);
        }
    }

    public void ResetPuzzleAll()
    {
        foreach(var e in puzzleImageEvents)
        {
            e.gameObject.SetActive(true);
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
}
