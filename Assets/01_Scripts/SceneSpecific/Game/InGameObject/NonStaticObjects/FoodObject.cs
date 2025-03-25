using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodObject : MonoBehaviour
{
    [SerializeField] private SpriteRenderer renderer;
    private FoodData data;

    public void SetSprite(Sprite sprite)
    {
        renderer.sprite = sprite;
    }
}
