using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class PuzzleDataSetter : MonoBehaviour
{
    private PuzzleData data;

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

        string prefabName = string.Empty;
        for(int i = 0; i < data.PuzzlePieceTransforms.Length; ++i)
        {
            prefabName = $"Prefabs/{data.Name}/{data.Name}_Piece{i + 1}";
            var piece = Resources.Load(prefabName) as GameObject;
            pieces[i] = Instantiate(piece).transform;
            pieces[i].SetParent(transform);
            pieces[i].gameObject.SetActive(false);
        }

        prefabName = $"Prefabs/{data.Name}/{data.Name}_Core";
        var coreResource = Resources.Load(prefabName) as GameObject;
        core = Instantiate(coreResource).transform;
        core.SetParent(transform);
        core.gameObject.SetActive(false);
    }

    private void SetPuzzle(ref Transform[] _transform)
    {
        for (int i = 0; i < _transform.Length; ++i)
        {
            pieces[i].SetParent(_transform[i]);
            pieces[i].localPosition = data.PuzzlePieceTransforms[i].Position;
            pieces[i].localRotation = Quaternion.Euler(data.PuzzlePieceTransforms[i].Rotation);
        }
    }
}
