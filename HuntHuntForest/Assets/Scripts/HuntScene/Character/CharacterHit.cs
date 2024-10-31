using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.Events;

public class CharacterHit : MonoBehaviour
{
    [SerializeField] private Transform camaraTrs;
    [SerializeField] private int maxHealth;
    private int currentHealth;
    public int CurrentHealth => currentHealth;

    [SerializeField] private GameObject hitBox;

    public UnityEvent OnHitFruit = new UnityEvent();
    private int fruitLayer;

    private HuntingManager manager;

    private void Start()
    {
        manager = GameObject.FindObjectOfType<HuntingManager>();
        manager.OnHuntStateChanged?.RemoveListener(OnStateChanged);
        manager.OnHuntStateChanged?.AddListener(OnStateChanged);

        fruitLayer = LayerMask.NameToLayer("SliceableObj");
    }

    private void LateUpdate()
    {
        if(hitBox.activeSelf)
        {
            hitBox.transform.position = camaraTrs.position +
                camaraTrs.forward * -0.4f;
            hitBox.transform.rotation = camaraTrs.rotation;
        }
    }

    private void OnStateChanged()
    {
        switch (manager.CurrentState)
        {
            case HuntingManager.HuntState.Ready: Ready(); break;
            case HuntingManager.HuntState.Defence: Defence(); break;
            case HuntingManager.HuntState.Attack: Attack(); break;
        }
    }

    private void Ready()
    {
        currentHealth = maxHealth;
    }

    private void Defence()
    {
        hitBox.SetActive(true);
    }

    private void Attack()
    {
        hitBox.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer != fruitLayer)
            return;     

        var fruit = other.GetComponent<SliceableFruit>();

        // 과일에 맞음
        currentHealth -= fruit.Damage;
        OnHitFruit?.Invoke();
    }
}
