using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TalkController : MonoBehaviour
{
    public GameObject layout;
    private void Awake()
    {
        layout.SetActive(false);
    }
    
}
