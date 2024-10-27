using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using Newtonsoft.Json;
using System;

public class LocalDataManager : SingletonBehaviour<LocalDataManager>
{
    public List<PuzzleData> PuzzleDatas {  get; private set; } = new List<PuzzleData>();

    private string filePath = Application.dataPath + "/Resources/Data/PuzzleData.json";
    private string fileResourcePath = "Data/PuzzleData";

    public void DatakInitialize()
    {
        TextAsset puzzleJsonFile = Resources.Load(fileResourcePath) as TextAsset;
        SerializedList<PuzzleData> serializedList = JsonUtility.FromJson<SerializedList<PuzzleData>>(puzzleJsonFile.text);
        PuzzleDatas = serializedList.Items;
    }

    public void Save()
    {
        int id = 0;
        foreach (var puzzleData in PuzzleDatas)
        {
            puzzleData.ID = id;
            ++id;
        }

        SerializedList<PuzzleData> serializedList = new SerializedList<PuzzleData>();
        serializedList.Items = PuzzleDatas;
        var jsonData = JsonUtility.ToJson(serializedList, true);
        File.WriteAllText(filePath, jsonData);
        Debug.Log(jsonData);
    }

    [Serializable]
    private class SerializedList<T>
    {
        public List<T> Items;
    }
}
