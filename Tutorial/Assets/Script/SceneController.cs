using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public int tryCount = 0;
    public int level = 0;

    void Start()
    {
      DontDestroyOnLoad(gameObject);
    }
}
