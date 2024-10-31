using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Builboard : MonoBehaviour
{
    [SerializeField] private Transform targetTrs;

    private void LateUpdate()
    {
        transform.LookAt(targetTrs.position);
    }
}
