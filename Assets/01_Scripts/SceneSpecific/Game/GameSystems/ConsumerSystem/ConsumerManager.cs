using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ConsumerManager : MonoBehaviour
{
    [SerializeField] private WorkFlowController workFlowController;
    private ObjectPool<ConsumerFSM> Pool;
}
