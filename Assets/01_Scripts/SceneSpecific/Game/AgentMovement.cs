using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavmeshTest : MonoBehaviour, IInteractor
{
    public bool IsBusy => currentWork != null;
    
    private WorkBase currentWork;
    
    private NavMeshAgent agent;

    private void Awake()
    {
        this.agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    private void Update()
    {
        
    }

    private void SetTargetPosition()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            agent.SetDestination(target);
        }
    }

    private float interactionSpeed = 10f;
    public float InteractionSpeed { get; }
    public void InteractWithObject(IInteractable interactable)
    {
        interactable.OnInteract(this);
    }

    public void OnInteractComplete(IInteractable interactable)
    {
        throw new System.NotImplementedException();
    }
    
    
    public void AssignWork(WorkBase work)
    {
        currentWork = work;
    }

    //나한테 Work가 있나
    //Work가 있다면 Dowork running success fail
    //행동트리 -> 
    
    
    
    public void CancelTask()
    {
        throw new System.NotImplementedException();
    }
}
