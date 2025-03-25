using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITransportable
{
    Transform HandPivot { get; }

    void LiftPackage(GameObject package);
    void DropPackage(Transform dropPoint);
}
