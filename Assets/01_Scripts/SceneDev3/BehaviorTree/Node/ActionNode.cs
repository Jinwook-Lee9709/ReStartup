using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ActionNode<T> : BehaviorNode<T> where T : MonoBehaviour
{
    protected ActionNode(T context) : base(context)
    {

    }
    protected abstract override NodeStatus OnUpdate();

}
