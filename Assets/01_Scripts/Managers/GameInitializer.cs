using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��ü ���� �÷ο쿡 �ʿ��� �̱��� ��ü���� ���� ���ø����̼� ���� �� �ڵ� ���� ���ִ� Ŭ����
/// </summary>
public class GameInitializer : MonoBehaviour
{
    private void Awake()
    {
        LocalSaveLoadManager.GameSettingInit();
    }
}
