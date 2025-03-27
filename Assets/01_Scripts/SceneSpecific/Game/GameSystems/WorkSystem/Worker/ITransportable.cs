using UnityEngine;

public interface ITransportable
{
    Transform HandPivot { get; }

    void LiftPackage(GameObject package);
    void DropPackage(Transform dropPoint);
}