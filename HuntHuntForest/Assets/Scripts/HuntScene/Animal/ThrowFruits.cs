using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ThrowFruits : MonoBehaviour
{
    [SerializeField] private Transform[] throwPositions;
    [SerializeField] private float power = 2f;
    [SerializeField] private GameObject[] fruits;

    private void Start()
    {
        InvokeRepeating(nameof(Throw), 1f, 3f);
    }

    public void Throw()
    {
        int index = Random.Range(0, fruits.Length);
        int throwPosition = Random.Range(0, throwPositions.Length);
        var go = Instantiate(fruits[index], throwPositions[throwPosition].position, Quaternion.identity);
        go.GetComponent<Rigidbody>().AddForce((Camera.main.transform.position - throwPositions[throwPosition].position + Vector3.up) * power, 
            ForceMode.Impulse);
        Destroy(go, 4f);
    }
}
