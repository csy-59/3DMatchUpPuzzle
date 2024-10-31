using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CharacterAttack : MonoBehaviour
{
    [SerializeField] private Transform fruitPosition;
    [SerializeField] private GameObject apple;
    private Rigidbody appleRigid;

    private bool isFruitThrowing = false;
    private HuntingManager manager;

    private Vector3 mousePosition;

    [SerializeField] private float throwForceRatio;
    [SerializeField] private float returnTime;
    private WaitForSeconds waitForReturnTime;

    public UnityEvent OnCharacterAttackEnd { get; private set; } = new UnityEvent();

    private void Start()
    {
        appleRigid = apple.GetComponent<Rigidbody>();
        manager = FindObjectOfType<HuntingManager>();
        manager.OnHuntStateChanged?.RemoveListener(OnStateChanged);
        manager.OnHuntStateChanged?.AddListener(OnStateChanged);

        waitForReturnTime = new WaitForSeconds(returnTime);
    }

    private void OnStateChanged()
    {
        switch(manager.CurrentState)
        {
            case HuntingManager.HuntState.Attack: Attack(); break;
            default: Ready(); break;
        }
    }

    private void Attack()
    {
        apple.gameObject.SetActive(true);
        this.enabled = true;
        isFruitThrowing = false;
        mousePosition = Vector3.zero;
    }

    private void Ready()
    {
        apple.gameObject.SetActive(false);
        this.enabled = false;
    }

    private void Update()
    {
        if (isFruitThrowing)
            return;

        if(Input.GetMouseButtonDown(0))
        {
            mousePosition = Input.mousePosition;
        }
        else if(Input.GetMouseButtonUp(0))
        {
            var offset = mousePosition - Input.mousePosition;
            var force = offset.magnitude;

            appleRigid.isKinematic = false;
            appleRigid.AddForce((transform.forward + transform.up).normalized * force * throwForceRatio);
            appleRigid.AddTorque(Vector3.right * -10f);

            isFruitThrowing = true;

            StartCoroutine(CoResetApple());
        }
    }

    private void LateUpdate()
    {
        if (isFruitThrowing)
            return;

        apple.transform.position = fruitPosition.position;
        apple.transform.rotation = fruitPosition.rotation;
    }

    private IEnumerator CoResetApple()
    {
        yield return waitForReturnTime;
        appleRigid.isKinematic = true;
        appleRigid.velocity = Vector3.zero;
        appleRigid.angularVelocity = Vector3.zero;
        isFruitThrowing = false;

        OnCharacterAttackEnd?.Invoke();
    }
}
