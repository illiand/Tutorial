﻿using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class UISpritesAnimation : MonoBehaviour
{
    public float duration;

    [SerializeField] private Sprite[] sprites;

    private Image image;
    private int index = 0;
    private float timer = 0;

    void Start()
    {
        image = GetComponent<Image>();
    }
    private void Update()
    {
        //当每帧时间过了之后播放下一帧图片
        if ((timer += Time.deltaTime) >= (duration / sprites.Length))// 播放总时长/帧数 =每帧时间
        {
            timer = 0;
            image.sprite = sprites[index];
            index = (index + 1) % sprites.Length;//小于length的数余数取整永远是它自己，index到length-1就可以了
            if(index == sprites.Length - 1)
            {
                Destroy(gameObject, duration / sprites.Length);
            }
        }
        
        
    }
}
