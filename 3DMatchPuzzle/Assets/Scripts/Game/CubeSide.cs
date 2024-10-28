using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeSide : MonoBehaviour
{
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private Collider meshCollider;

    public void Hit()
    {
        Debug.Log(gameObject.name);
    }
}
