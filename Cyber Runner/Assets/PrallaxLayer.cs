using System;
using System.Collections;
using System.Collections.Generic;
using Services;
using UnityEngine;

public class PrallaxLayer : MonoBehaviour
{
    
    public float Speed;
    public SpriteRenderer Renderer;
    private Sprite _sprite;
    private LazyService<PlayerController> Player;

    void Start()
    {
        _sprite = Renderer.sprite;
        
    }

    void Update()
    {
        transform.localPosition += Speed * Time.deltaTime * Vector3.left;

        if (Math.Abs(transform.localPosition.x) >= _sprite.bounds.size.x)
        {
            transform.localPosition = Vector3.zero;
        }
    }

    private void ResetSprite()
    {
        
    }
}
