using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPrebsManager : Singleton<PlayerPrebsManager>
{
    readonly static string Initialized = "Initialized";

    private bool[] isCleared;

    public void Init()
    {
        int puzzleCount = LocalDataManager.Instance.PuzzleDatas.Count;
        isCleared = new bool[puzzleCount];

        foreach(var puzzle in LocalDataManager.Instance.PuzzleDatas)
        {
            isCleared[puzzle.ID] = GetBool(puzzle.Name);
        }
    }

    public bool GetBool(string _key)
    {
        if(PlayerPrefs.HasKey(_key) == false)
        {
            PlayerPrefs.SetInt(_key, 0);
            PlayerPrefs.Save();
        }

        return PlayerPrefs.GetInt(_key) != 0;
    }

    public void SetBool(string _key, bool _value)
    {
        PlayerPrefs.SetInt(_key, _value ? 1 : 0);
    }
}
