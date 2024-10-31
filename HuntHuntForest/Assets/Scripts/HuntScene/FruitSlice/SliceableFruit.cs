using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SliceableFruit : MonoBehaviour
{
    [SerializeField] private int damage;
    public int Damage => damage;

    [SerializeField] private MeshRenderer render;
    [SerializeField] private Transform[] effectObjs;
    [SerializeField] private ParticleSystem[] effects;
    private WaitForSeconds waitForEffectEnd;

    public UnityEvent OnFruitSliced = new UnityEvent();
    private void Start()
    {
        Init();
    }

    private void Init()
    {
        float maxEffectTime = 0f;
        foreach(var effect in effects)
        {
            if(maxEffectTime < effect.time)
            {
                maxEffectTime = effect.time;
            }
        }

        waitForEffectEnd= new WaitForSeconds(maxEffectTime);

        foreach(var obj in effectObjs)
        {
            obj.SetParent(transform);
            obj.gameObject.SetActive(false);
        }
    }

    private void nEnable()
    {
        foreach (var obj in effectObjs)
        {
            obj.SetParent(transform);
            obj.gameObject.SetActive(false);
        }

        render.enabled = true;
    }

    public void Sliced()
    {
        OnFruitSliced?.Invoke();
        render.enabled = false;

        foreach(var obj in effectObjs)
        {
            obj.SetParent(null);
            obj.position = transform.position;
            obj.rotation = transform.rotation;
            obj.gameObject.SetActive(true);
        }

        foreach(var e in effects)
        {
            e.Play();

        }

        StartCoroutine(CoSliceEffect());
    }

    private IEnumerator CoSliceEffect()
    {
        yield return waitForEffectEnd;

        foreach (var obj in effectObjs)
        {
            obj.gameObject.SetActive(false);
            obj.SetParent(transform);
        }
    }

    private void OnDestroy()
    {
        foreach(var obj in effectObjs)
        {
            Destroy(obj);
        }
    }
}
