using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FruitKnife : MonoBehaviour
{
    [SerializeField] private Material capMaterail;
    [SerializeField] private LayerMask targetLayer = 1 << 6;
    [SerializeField] private float sliceDistance = 10f;
    private Vector3 startNormalVector = Vector3.zero;

    private GameObject targetObj;

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
            case HuntingManager.HuntState.Defence:
                {
                    this.enabled = true;
                    break;
                }
            default:
                {
                    this.enabled = false;
                    break;
                }
        }
    }

    private void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Debug.DrawRay(ray.origin, ray.direction, Color.green);

        if (Input.GetMouseButton(0))
        {
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, sliceDistance, targetLayer))
            {
                // 자르기 시작
                startNormalVector = ray.direction;
                targetObj = hit.transform.gameObject;
            }
            else if (targetObj != null)
            {
                // 자르기 완료
                targetObj.GetComponent<SliceableFruit>().Sliced();

                Vector3 endNormalVector = ray.direction;
                Vector3 resultNoramlVector = Vector3.Cross(startNormalVector, endNormalVector);
                var objs = MeshSlicer.Instance.Slice(targetObj, targetObj.transform.position, resultNoramlVector, capMaterail);

                float dir = 1;
                foreach (var obj in objs)
                {
                    Rigidbody rb = obj.AddComponent<Rigidbody>();
                    rb.AddForce(Vector3.one * dir, ForceMode.Impulse);
                    dir *= -1f;

                    Destroy(obj, 3f);
                }

                targetObj = null;
            }
            else
            {
                targetObj = null;
            }
        }
        
    }
}
