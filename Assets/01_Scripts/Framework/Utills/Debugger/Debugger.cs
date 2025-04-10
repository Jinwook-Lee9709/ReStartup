using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public abstract class Debugger : MonoBehaviour
{
    [SerializeField] protected DebugFlags DebugType;
    protected bool IsAvailable;

    protected void Start()
    {
        CheckIsAvailable();
        Variables.OnDebugFlagsChanged -= OnFlagsChanged;
        Variables.OnDebugFlagsChanged += OnFlagsChanged;
    }

    private void CheckIsAvailable()
    {
        EvaluateAndProcess(Variables.DebugFlags);
    }

    private void OnFlagsChanged(DebugFlags flags)
    {
        EvaluateAndProcess(flags);
    }

    private void EvaluateAndProcess(DebugFlags flags)
    {
        if (flags.HasFlag(DebugType))
        {
            OnActiavted();
            IsAvailable = true;
        }
        else
        {
            OnDeactivated();
            IsAvailable = false;
        }
    }
    
    protected abstract void OnActiavted();
    protected abstract void OnDeactivated();

}
