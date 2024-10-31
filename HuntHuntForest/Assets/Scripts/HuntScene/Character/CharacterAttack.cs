using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class CharacterAttack : MonoBehaviour
{
    [SerializeField] private Transform camaraTrs;
    [SerializeField] private Vector3 position;
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

#if UNITY_EDITOR
        if(Input.GetMouseButtonDown(0))
#elif PLATFORM_ANDROID
        if(Input.GetTouch(0).phase == TouchPhase.Began)
#endif
        {
#if UNITY_EDITOR
            mousePosition = Input.mousePosition;
#elif PLATFORM_ANDROID
            mousePosition = Input.GetTouch(0).position;
#endif
        }
        else if(Input.GetMouseButtonUp(0))
        {
#if UNITY_EDITOR
            var offset = mousePosition - Input.mousePosition;
#elif PLATFORM_ANDROID
            Vector3 offset = mousePosition - (Vector3)Input.GetTouch(0).position;
#endif
            var force = offset.magnitude;

            appleRigid.isKinematic = false;
            appleRigid.AddForce((camaraTrs.forward + camaraTrs.up).normalized * force * throwForceRatio);
            appleRigid.AddTorque(-offset.y, offset.x, 0f);

            isFruitThrowing = true;

            StartCoroutine(CoResetApple());
        }
    }

    private void LateUpdate()
    {
        if (isFruitThrowing)
            return;

        apple.transform.position = camaraTrs.position + 
            camaraTrs.forward * 0.4f - camaraTrs.up * .12f;
        apple.transform.rotation = camaraTrs.rotation;
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
