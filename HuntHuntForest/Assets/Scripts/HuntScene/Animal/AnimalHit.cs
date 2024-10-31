using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AnimalHit : MonoBehaviour
{
    [SerializeField] private AnimalData data;
    [SerializeField] private Collider hitBox;

    private int currentHealth;
    public int CurrentHealth { get => currentHealth; set { currentHealth = value; OnHit?.Invoke(); } }
    public UnityEvent OnHit { get; private set; } = new UnityEvent();

    private LayerMask playerFruitLayer = 1 << 7;

    private HuntingManager manager;

    private void Start()
    {
        manager = GameObject.FindObjectOfType<HuntingManager>();
        manager.OnHuntStateChanged?.RemoveListener(OnStateChanged);
        manager.OnHuntStateChanged?.AddListener(OnStateChanged);
    }

    private void OnStateChanged()
    {
        switch(manager.CurrentState)
        {
            case HuntingManager.HuntState.Ready: Ready(); break;
            case HuntingManager.HuntState.Defence: Defence(); break;
            case HuntingManager.HuntState.Attack: Attack(); break;
        }
    }

    private void Ready()
    {
        currentHealth = data.Health;
    }

    private void Defence()
    {
        hitBox.enabled = false;
    }

    private void Attack()
    {
        hitBox.enabled = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer != playerFruitLayer.value)
            return;

        // 주인공이 던지 과일에 맞음
        --CurrentHealth;
    }
}
