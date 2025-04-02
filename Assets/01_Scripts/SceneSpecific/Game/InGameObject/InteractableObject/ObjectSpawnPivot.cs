using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawnPivot : MonoBehaviour
{
    public ObjectArea ObjectArea { get; private set; }
    public int ObjectAreaId { get; private set; }

    public void Init(ObjectArea objectArea, int objectAreaId)
    {
        ObjectArea = objectArea;
        ObjectAreaId = objectAreaId;
    }
    
    
}
