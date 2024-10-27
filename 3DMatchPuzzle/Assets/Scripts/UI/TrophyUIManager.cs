using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrophyUIManager : MonoBehaviour
{
    [SerializeField]
    public void OnClickTrophyBtn()
    {
        gameObject.SetActive(true);
    }
}
