using Excellcube.EasyTutorial.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class TutorialLocalizer : TextLocalizer
{
    public override string GetLocalizedText(string table, string key)
    {
        return LocalizationSettings.StringDatabase.GetLocalizedString(table, key);
    }
}
