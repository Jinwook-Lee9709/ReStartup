using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 전체 게임 플로우에 필요한 싱글톤 객체들을 최초 어플리케이션 실행 시 자동 생성 해주는 클래스
/// </summary>
public class GameInitializer : MonoBehaviour
{
    private void Awake()
    {
        LocalSaveLoadManager.Instance.GameSettingInit();
        UserDataManager.Instance.InitCurrentUserData();
    }
}
