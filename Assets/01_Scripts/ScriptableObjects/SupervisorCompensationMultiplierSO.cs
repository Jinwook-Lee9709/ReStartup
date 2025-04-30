using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SupervisorCompensationSO", menuName = "ScriptableObejcts/SupervisorCompensationSO", order = 2)]
public class SupervisorCompensationSO : ScriptableObject
{
    public VInspector.SerializedDictionary<int, int> multipliers;
    public VInspector.SerializedDictionary<int, int> baseCompensation;
}
