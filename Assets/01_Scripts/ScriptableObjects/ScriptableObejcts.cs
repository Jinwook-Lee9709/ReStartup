using System;
using System.Collections;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using UnityEngine;

[ CreateAssetMenu(fileName = "WorkDurationRatioSO", menuName = "ScriptableObejcts/WorkDurationRatioSO", order = 1)]
public class WorkDurationRatioSO : ScriptableObject
{
    [SerializedDictionary, SerializeField] public SerializedDictionary<string, float> WorkDurationRatio;
}
