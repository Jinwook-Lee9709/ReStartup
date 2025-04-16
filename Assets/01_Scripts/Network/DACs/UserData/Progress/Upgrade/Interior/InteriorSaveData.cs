using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class InteriorSaveData
{
    public int id;
    [JsonConverter(typeof(ThemeIdConverter))]
    public ThemeIds theme;
    public int level;

}
