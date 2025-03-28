using UnityEngine;

/// <summary>
///     ��ü ���� �÷ο쿡 �ʿ��� �̱��� ��ü���� ���� ���ø����̼� ���� �� �ڵ� ���� ���ִ� Ŭ����
/// </summary>
public class GameInitializer : MonoBehaviour
{
    private void Awake()
    {
        LocalSaveLoadManager.GameSettingInit();
        Application.targetFrameRate = 120;
        QualitySettings.vSyncCount = 0;
    }
}