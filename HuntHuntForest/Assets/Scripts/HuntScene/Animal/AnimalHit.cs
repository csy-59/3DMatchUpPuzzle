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

    private int playerFruitLayer;

    private HuntingManager manager;

    private void Start()
    {
        manager = GameObject.FindObjectOfType<HuntingManager>();
        manager.OnHuntStateChanged?.RemoveListener(OnStateChanged);
        manager.OnHuntStateChanged?.AddListener(OnStateChanged);

        playerFruitLayer = LayerMask.NameToLayer("PlayerFruit");
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer != playerFruitLayer)
            return;

        // 주인공이 던지 과일에 맞음
        --CurrentHealth;
    }
}
