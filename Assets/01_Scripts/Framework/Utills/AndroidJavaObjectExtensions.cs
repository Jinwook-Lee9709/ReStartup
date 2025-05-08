using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AndroidJavaObjectExtensions
{
    public static AndroidJavaObject CreateChooser(List<AndroidJavaObject> intents, AndroidJavaObject activity)
    {
        if (intents.Count == 0)
            return null;

        // 맨 앞 인텐트를 기본 chooser 인텐트로 설정
        AndroidJavaObject chooser = new AndroidJavaClass("android.content.Intent")
            .CallStatic<AndroidJavaObject>("createChooser", intents[0], "메일 앱 선택");

        if (intents.Count > 1)
        {
            // 나머지 인텐트를 EXTRA_INITIAL_INTENTS로 추가
            AndroidJavaObject[] extraIntents = intents.GetRange(1, intents.Count - 1).ToArray();
            chooser.Call<AndroidJavaObject>("putExtra", "android.intent.extra.INITIAL_INTENTS", extraIntents);
        }

        return chooser;
    }
}