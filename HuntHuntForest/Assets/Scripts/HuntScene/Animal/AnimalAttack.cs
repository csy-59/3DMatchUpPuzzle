using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class AnimalAttack : MonoBehaviour
{
    [SerializeField] private Transform[] throwPositions;
    [SerializeField] private float throwPower = 2f;
    [SerializeField] private GameObject[] fruits;

    public SliceableFruit Throw()
    {
        int index = Random.Range(0, fruits.Length);
        int throwPosition = Random.Range(0, throwPositions.Length);
        var go = Instantiate(fruits[index], throwPositions[throwPosition].position, Quaternion.identity);

        var rigid = go.GetComponent<Rigidbody>();
        rigid.AddForce(
            (Camera.main.transform.position - throwPositions[throwPosition].position + Vector3.up) * throwPower, 
            ForceMode.Impulse);
        rigid.AddTorque(Vector3.right * -10f);

        Destroy(go, 4f);

        return go.GetComponent<SliceableFruit>();
    }
}
