using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapRendererLocator : MonoBehaviour
{
    [SerializeField] public List<SpriteRenderer> hallFloor;
    [SerializeField] public List<SpriteRenderer> hallWall;
    [SerializeField] public List<SpriteRenderer> kitchenFloor;
    [SerializeField] public List<SpriteRenderer> kitchenWall;
    [SerializeField] public Transform decor;

}
