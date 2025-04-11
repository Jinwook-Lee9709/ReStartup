using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FurnitureBase : MonoBehaviour, IInterior
{
    [SerializeField] private SpriteRenderer spriteRenderer;

    public void ChangeSpirte(Sprite sprite)
    {
        spriteRenderer.sprite = sprite;
    }
}
