using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IWorker
{
    bool IsBusy { get; }
    
    void AssignWork(WorkBase work);
    void CancelTask();
}
