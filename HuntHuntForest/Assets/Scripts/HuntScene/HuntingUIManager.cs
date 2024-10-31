using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HuntingUIManager : MonoBehaviour
{
    [SerializeField] private HuntingManager manager;
    private AnimalHit animalHit;
    private CharacterHit playerHit;

    [Header("Ready")]
    [SerializeField] private Transform readyPanel;
    [SerializeField] private TextMeshProUGUI readyTime;

    [Header("Defence")]
    [SerializeField] private Transform defencePanel;

    [Header("Attack")]
    [SerializeField] private Transform attackPanel;

    [Header("Success")]
    [SerializeField] private Transform successPanel;

    [Header("Fail")]
    [SerializeField] private Transform failPanel;

    [Header("AnimalHealth")]
    [SerializeField] private Transform animalHeartPanel;
    [SerializeField] private GameObject[] animalHeart;

    [Header("PlayerHealth")]
    [SerializeField] private Transform playerHeartPanel;
    [SerializeField] private GameObject[] playerHeart;

    private void Start()
    {
        manager.OnHuntStateChanged?.RemoveListener(OnStateChanged);
        manager.OnHuntStateChanged?.AddListener(OnStateChanged);

        manager.OnAttackStart?.RemoveListener(OnAttackStart);
        manager.OnAttackStart?.AddListener(OnAttackStart);

        manager.OnDefenceStart?.RemoveListener(OnDefenceStart);
        manager.OnDefenceStart?.AddListener(OnDefenceStart);

        manager.OnReayTimePassed?.RemoveListener(OnReadyTime);
        manager.OnReayTimePassed?.AddListener(OnReadyTime);

        SetPanelsActive();
    }

    private void OnStateChanged()
    {
        switch(manager.CurrentState)
        {
            case HuntingManager.HuntState.Ready:
                {
                    animalHit = manager.AnimalHit;
                    playerHit = manager.PlayerHit;

                    animalHit.OnHit?.RemoveListener(OnAnimalHealth);
                    animalHit.OnHit?.AddListener(OnAnimalHealth);
                    OnAnimalHealth();

                    playerHit.OnHitFruit?.RemoveListener(OnPlayerHealth);
                    playerHit.OnHitFruit?.AddListener(OnPlayerHealth);

                    SetPanelsActive();
                    break;
                }
            case HuntingManager.HuntState.Defence:
            case HuntingManager.HuntState.Attack:
                {
                    SetPanelsActive(_player: true, _animal: true);
                    break;
                }
            case HuntingManager.HuntState.Success:
                {
                    SetPanelsActive(_success: true);
                    break;
                }
            case HuntingManager.HuntState.Fail:
                {
                    SetPanelsActive(_fail: true);
                    break;
                }
            case HuntingManager.HuntState.Return:
                {
                    gameObject.SetActive(false);
                    break;
                }
        }
    }

    private void OnReadyTime(int time)
    {
        if (time < 0)
        {
            SetPanelsActive();
            readyTime.text = string.Empty;
        }
        else
        {
            SetPanelsActive(_ready: true);
            readyTime.text = $"{time}";
        }
    }

    private void OnDefenceStart()
    {
        OnAnimalHealth();
        SetPanelsActive(_defence: true, _animal: true, _player: true);
    }

    private void OnAttackStart()
    {
        SetPanelsActive(_attack: true, _animal: true, _player: true);

    }

    private void OnAnimalHealth()
    {
        for (int i = 0; i < animalHeart.Length; ++i)
        {
            animalHeart[i].gameObject.SetActive(animalHit.CurrentHealth > i);
        }
    }

    private void OnPlayerHealth()
    {
        for (int i = 0; i < playerHeart.Length; ++i)
        {
            playerHeart[i].gameObject.SetActive(playerHit.CurrentHealth > i);
        }
    }

    private void SetPanelsActive(
        bool _ready = false, bool _defence = false,
        bool _attack = false, bool _success = false, 
        bool _fail = false, bool _animal = false, 
        bool _player = false)
    {
        readyPanel.gameObject.SetActive(_ready);
        defencePanel.gameObject.SetActive(_defence);
        attackPanel.gameObject.SetActive(_attack);
        successPanel.gameObject.SetActive(_success);
        failPanel.gameObject.SetActive(_fail);
        animalHeartPanel.gameObject.SetActive(_animal);
        playerHeartPanel.gameObject.SetActive(_player);
    }
}
