using UnityEngine;

/// <summary>
///     ��ü ���� �÷ο쿡 �ʿ��� �̱��� ��ü���� ���� ���ø����̼� ���� �� �ڵ� ���� ���ִ� Ŭ����
/// </summary>
public class GameInitializer : MonoBehaviour
{
    [SerializeField] TitleSceneManager titleSceneManager;
    private void Awake()
    {
        LocalSaveLoadManager.GameSettingInit();
        switch (LocalSaveLoadManager.Data.LanguageType)
        {
            case LanguageType.Korean:
                titleSceneManager.SwitchToKorean();
                break;
            case LanguageType.English:
                titleSceneManager.SwitchToEnglish();
                break;
        }
        Application.targetFrameRate = 120;
        QualitySettings.vSyncCount = 0;
    }
    private void Start()
    {


    }
}