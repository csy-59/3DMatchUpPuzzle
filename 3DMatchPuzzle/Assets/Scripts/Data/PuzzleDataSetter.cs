using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class PuzzleDataSetter : MonoBehaviour
{
    private PuzzleData data;
    public PuzzleData Data => data;

    [SerializeField] private Transform[] pieces = new Transform[6];
    [SerializeField] private Transform core;
    [SerializeField] private float playTime;


    [ContextMenu("Save")]
    public void Save()
    {
        if (data == null)
        {
            data = new PuzzleData();
            data.Name = gameObject.name;
            data.MaxPlayTime = playTime;
        }

        for (int i = 0; i < pieces.Length; ++i)
        {
            data.PuzzlePieceTransforms[i].Position = pieces[i].localPosition;
            data.PuzzlePieceTransforms[i].Rotation = pieces[i].localRotation.eulerAngles;
        }

        LocalDataManager.Instance.PuzzleDatas.Add(data);
    }

    public void Load(PuzzleData _data)
    {
        data = _data;
    }

    public void SetPuzzle(ref Transform[] _pieces)
    {
        for (int i = 0; i < pieces.Length; ++i)
        {
            _pieces[i].localPosition = data.PuzzlePieceTransforms[i].Position;
            _pieces[i].localRotation = Quaternion.Euler(data.PuzzlePieceTransforms[i].Rotation);
        }
    }
}
