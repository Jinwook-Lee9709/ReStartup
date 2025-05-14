using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectHandler : MonoBehaviour
{
    public void OnEffectEnd()
    {
        this.gameObject.SetActive(false);
    }
}
