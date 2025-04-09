// using System;
// using UnityEngine;
// using UnityEngine.AI;
//
// public class NavmeshTest : MonoBehaviour, IInteractor
// {
//     private NavMeshAgent agent;
//
//     private WorkBase currentWork;
//
//     private float interactionSpeed = 10f;
//     public bool IsBusy => currentWork != null;
//
//     private void Awake()
//     {
//         agent = GetComponent<NavMeshAgent>();
//         agent.updateRotation = false;
//         agent.updateUpAxis = false;
//     }
//
//     private void Update()
//     {
//     }
//
//     public float InteractionSpeed { get; }
//
//     private void SetTargetPosition()
//     {
//         if (Input.GetMouseButtonDown(0))
//         {
//             var target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
//             agent.SetDestination(target);
//         }
//     }
//
//     public void InteractWithObject(IInteractable interactable)
//     {
//         interactable.OnInteract(this);
//     }
//
//     public void OnInteractComplete(IInteractable interactable)
//     {
//         throw new NotImplementedException();
//     }
//
//
//     public void AssignWork(WorkBase work)
//     {
//         currentWork = work;
//     }
//
//     //나한테 Work가 있나
//     //Work가 있다면 Dowork running success fail
//     //행동트리 -> 
//
//
//     public void CancelTask()
//     {
//         throw new NotImplementedException();
//     }
// }