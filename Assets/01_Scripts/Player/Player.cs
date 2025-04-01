using UnityEngine;
using UnityEngine.AI;

public class Player : WorkerBase, IInteractor, ITransportable
{
    protected WorkType workType = WorkType.All;
    private Transform handPivot;
    private float interactionSpeed = Constants.PLAYER_INTERACTION_SPEED;
    public float InteractionSpeed => interactionSpeed;
    public Transform HandPivot => handPivot;
    
    public void OnMoveOrWork(bool work, Vector2 pos)
    {
        Debug.Log(work);
        if (work)
            Debug.Log("InteractableObjects Touch");
        else
            agent.SetDestination(pos);
    }
    public void LiftPackage(GameObject package)
    {
        package.transform.SetParent(handPivot);
        package.transform.localPosition = Vector3.zero;
    }

    public void DropPackage(Transform dropPoint)
    {
        if (handPivot.childCount > 0)
        {
            var package = handPivot.GetChild(0).gameObject;
            package.transform.SetParent(dropPoint);
            package.transform.localPosition = Vector3.zero;
        }
    }
}