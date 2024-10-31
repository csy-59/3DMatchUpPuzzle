using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SearchManager : MonoBehaviour
{
    [Header("Field")]
    [SerializeField] private GameObject[] grass;
    [SerializeField] private Transform minPosition;
    [SerializeField] private Transform maxPosition;
    [SerializeField][Range(50, 5000)] private int maxGrassCount;

    private int targetLayer;
    public UnityEvent<AnimalData> OnTargetFound { get; private set; } = new UnityEvent<AnimalData>();

    [SerializeField] private GameManager gameManager;
    private GameObject[] animals;
    private AnimalData[] animalDatas;
    private float totalRate;

    private void Start()
    {
        for(int i = 0; i< maxGrassCount; ++i)
        {
            int randomIndex = UnityEngine.Random.Range(0, grass.Length);

            Vector3 position = new Vector3(UnityEngine.Random.Range(minPosition.position.x, maxPosition.position.x), 0f,
                UnityEngine.Random.Range(minPosition.position.z, maxPosition.position.z));

            Vector3 rotation = new Vector3(0f, UnityEngine.Random.Range(0f, 180f), 0f);
            GameObject obj = Instantiate(grass[randomIndex], position, Quaternion.Euler(rotation));

        }

        targetLayer = LayerMask.NameToLayer("TargetAnimal");

        animals = gameManager.Animals;
        animalDatas = gameManager.AnimalDatas;
        foreach (var anim in animalDatas)
        {
            totalRate += anim.Ratio;
        }
    }

    private void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Debug.DrawRay(ray.origin, ray.direction, Color.green);

#if UNITY_EDITOR
        if (Input.GetMouseButton(0))
#elif PLATFORM_ANDROID
        if(Input.GetTouch(0).phase == TouchPhase.Moved)
#endif
        {
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 10000, targetLayer))
            {
                var data = hit.transform.GetComponent<AnimalData>();
                OnTargetFound?.Invoke(data);
            }
        }
    }

    public void SpawnAnimal()
    {
        float rate = UnityEngine.Random.Range(0f, totalRate);

        float elapsed = 0f;
        foreach(var anim in animalDatas)
        {
            elapsed += anim.Ratio;
            if(elapsed > rate)
            {
                Vector3 position = new Vector3(UnityEngine.Random.Range(minPosition.position.x, maxPosition.position.x), 0f,
                UnityEngine.Random.Range(minPosition.position.z, maxPosition.position.z));

                Vector3 rotation = new Vector3(0f, UnityEngine.Random.Range(0f, 180f), 0f);
                animals[(int)anim.Type].transform.position = position;
                animals[(int)anim.Type].transform.rotation = Quaternion.Euler(rotation);
                animals[(int)anim.Type].SetActive(true);
            }
            animals[(int)anim.Type].SetActive(false);
        }
    }
}
