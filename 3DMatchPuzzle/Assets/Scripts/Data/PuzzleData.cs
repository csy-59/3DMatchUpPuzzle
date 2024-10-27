using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PuzzlePieceTransform
{
    public Vector3 Position;

    public Vector3 Rotation;

    public PuzzlePieceTransform()
    {
        Position = new Vector3();
        Rotation = new Vector3();
    }
}

[Serializable]
public class PuzzleData
{
    public int ID;
    public string Name;

    public PuzzlePieceTransform[] PuzzlePieceTransforms;

    public float MaxPlayTime;

    public PuzzleData()
    {
        PuzzlePieceTransforms = new PuzzlePieceTransform[6];
        for(int i = 0; i < 6; ++i)
        {
            PuzzlePieceTransforms[i] = new PuzzlePieceTransform();
        }
    }
}
