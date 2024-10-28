using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class PuzzleManager: SingletonBehaviour<PuzzleManager>
{
    private const int PieceCount = 6;

    [Serializable]
    private class PieceCollection
    {
        public DefaultObserverEventHandler Event;
        public List<Transform> Pieces;
        public Transform Hint;
    }

    [Header("Puzzles")]
    [SerializeField] private List<PieceCollection> puzzles;
    private List<PuzzleDataSetter> puzzleDatas;
    private int currentPuzzleIndex; // 현재 플레이 중인 퍼즐
    private int detactedPuzzleIndex; // 새로 감지될 퍼즐(currentPuzzle의 후보)

    public UnityEvent<bool> OnPuzzleDetactedEvent { get; private set; } = new UnityEvent<bool>();
    public UnityEvent OnPuzzleLostEvent { get; private set; } = new UnityEvent();

    [Header("Cube")]
    [SerializeField] private DefaultObserverEventHandler cubeImageEvent;

    [Header("ECT")]
    [SerializeField] private float hintRotateSpeed = 180f;
    [SerializeField] private float snapPositionOffset = 5f;
    [SerializeField] private float snapRotationOffset = 10f;

    protected override void Init()
    {
        base.Init();

        currentPuzzleIndex = -1;

        int index = 0;
        foreach (var e in puzzles)
        {
            int i = index;
            e.Event.OnTargetFound.AddListener(() => OnPuzzleDetacted(i));
            e.Event.OnTargetLost.AddListener(() => OnPuzzleLost(i));
            ++index;
        }

        cubeImageEvent.gameObject.SetActive(false);
    }

    public void LoadPuzzle()
    {
        int puzzleCount = LocalDataManager.Instance.PuzzleDatas.Count;
        puzzleDatas = new List<PuzzleDataSetter>(puzzleCount);

        foreach(var p in LocalDataManager.Instance.PuzzleDatas)
        {
            GameObject go = new GameObject(p.Name);
            puzzleDatas.Add(go.AddComponent<PuzzleDataSetter>());
            int index = puzzleDatas.Count - 1;

            puzzleDatas[index].Load(p);
            for (int i = 0; i < PieceCount; ++i)
            {
                puzzles[index].Pieces[i].localPosition = puzzleDatas[index].Data.PuzzlePieceTransforms[i].Position;
                puzzles[index].Pieces[i].localRotation = Quaternion.Euler(puzzleDatas[index].Data.PuzzlePieceTransforms[i].Rotation);
            }
        }
    }

    public void ResetPuzzleAll()
    {
        currentPuzzleIndex = -1;
        foreach(var e in puzzles)
        {
            e.Event.gameObject.SetActive(true);
            foreach(var t in e.Pieces)
            {
                t.gameObject.SetActive(false);
            }
        }

        cubeImageEvent.gameObject.SetActive(false);
    }

    private void OnPuzzleDetacted(int _puzzleIndex)
    {
        detactedPuzzleIndex = _puzzleIndex;
        OnPuzzleDetactedEvent?.Invoke(currentPuzzleIndex == _puzzleIndex);
    }

    public void GameStart()
    {
        currentPuzzleIndex = detactedPuzzleIndex;

        int puzzleCount = puzzleDatas.Count;
        for (int i = 0; i < puzzleCount; ++i)
        {
            if(i != currentPuzzleIndex)
            {
                puzzles[i].Event.gameObject.SetActive(false);
                puzzles[i].Hint.gameObject.SetActive(false);

                for (int j = 0; j < PieceCount; ++j)
                {
                    puzzles[i].Pieces[j].gameObject.SetActive(false);
                }
            }
        }

        puzzles[currentPuzzleIndex].Event.gameObject.SetActive(true);
        puzzles[currentPuzzleIndex].Hint.gameObject.SetActive(false);
        for (int j = 0; j < PieceCount; ++j)
        {
            puzzles[currentPuzzleIndex].Pieces[j].gameObject.SetActive(false);
        }

        cubeImageEvent.gameObject.SetActive(true);
        ShowHint(true);
    }

    public PuzzleData GetCurrentPuzzleData()
    {
        if(currentPuzzleIndex == -1)
        {
            return null;
        }
        return puzzleDatas[currentPuzzleIndex].Data;
    }

    private void OnPuzzleLost(int _puzzleIndex)
    {
        if (currentPuzzleIndex != _puzzleIndex)
            return;

        OnPuzzleLostEvent?.Invoke();
    }

    private void ShowHint(bool _show)
    {
        puzzles[currentPuzzleIndex].Hint.gameObject.SetActive(_show);
        if(_show)
        {
            StartCoroutine(CoLotateHint());
        }
        else
        {
            StopAllCoroutines();
        }
    }

    private IEnumerator CoLotateHint()
    {
        Transform target = puzzles[currentPuzzleIndex].Hint;

        while(true)
        {
            target.Rotate(0f, hintRotateSpeed * Time.deltaTime, 0f);
            yield return null;
        }
    }
}
