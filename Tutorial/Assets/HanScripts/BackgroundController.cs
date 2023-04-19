using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackgroundController : MonoBehaviour
{
    public Image backgroundImage;
    //public bool isSwitched = false;
    
    public void SwitchBGImage(Sprite sprite)
    {
      
       backgroundImage.sprite = sprite;

    }
}

