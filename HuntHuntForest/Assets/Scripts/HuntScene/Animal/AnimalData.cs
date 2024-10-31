using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AnimalType
{
    Colobus,
    Gecko,
    Herring,
    Muskrat,
    Pudu,
    Sparrow,
    Squid,
    Taipan,
}

public class AnimalData : MonoBehaviour
{
    [field:SerializeField] public int ID { get; private set; }
    [field: SerializeField] public AnimalType Type { get; private set; }
    [field: SerializeField] public string Name { get; private set; }

    [field: SerializeField] public int Health { get; private set; }
    [field: SerializeField] public int AttackCount { get; private set; }
}
