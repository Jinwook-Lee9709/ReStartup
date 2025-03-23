using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITransformable
{
    Transform handPivot { get; set; }

    void LiftPackage(Sprite packageSprite);
    void DropPackage();
}
