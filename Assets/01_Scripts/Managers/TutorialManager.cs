using Excellcube.EasyTutorial;
using Excellcube.EasyTutorial.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum TutorialPhase
{
    None = -1,


    Phase1,
    Phase2,
    Phase3,
    Phase4,
    Phase5,
    Phase6,
    Phase7,
}

public class TutorialManager : MonoBehaviour
{
    [VInspector.Foldout("Public")]
    [SerializeField] private List<ECEasyTutorial> tutorials;
    [SerializeField] private TutorialSkipPopup tutorialSkipPopupPrefab;
    [SerializeField] private EatrandAlarmPopup eatrandAlarmPopupPrefab;
    [SerializeField] private GameObject touchShield;
    [SerializeField] private GameObject smartPhone;
    private TutorialPhase currentPhase = TutorialPhase.Phase1;
    [VInspector.EndFoldout]
    private void Start()
    {
        TutorialInit();
    }

    public void ActiveTouchShield()
    {
        if (touchShield != null)
        {
            touchShield.SetActive(true);
        }
    }
    public void UnActiveTouchShield()
    {
        if (touchShield != null)
        {
            touchShield.SetActive(false);
        }
    }

    public void OnTutorialSkipButton(UnityAction action)
    {
        var popup = Instantiate(tutorialSkipPopupPrefab, transform);
        popup.Init(action);
    }

    public void TutorialOrderComplete()
    {
        TutorialEvent.Instance.Broadcast(Strings.tutorialCompeleteKey);
    }

    public void TutorialPhaseDone()
    {
        if (currentPhase == TutorialPhase.Phase7)
        {
            Destroy(gameObject);
        }
        currentPhase++;
        tutorials[(int)currentPhase].gameObject.SetActive(true);
    }

    public void TutorialInit()
    {
        PlayerPrefs.SetInt("ECET_CLEAR_ALL", 0);
    }
    public void ShakeSmartPhone()
    {
        StartCoroutine(ShakePhoneCoroutine(smartPhone));
        Handheld.Vibrate();
    }
    #region Phase1
    [VInspector.Foldout("Phase1")]
    [SerializeField][Range(0f, 3f)] private float shakeDuration;
    [SerializeField][Range(0f, 3f)] private float shakeAmount;
    [SerializeField] private CreateNamePopup createNamePopupPrefab;
    [SerializeField] private TutorialPhase1RewardPopup tutorialPhase1RewardPopupPrefab;
    [VInspector.EndFoldout]
    public void CreateName()
    {
        var popup = Instantiate(createNamePopupPrefab, transform);
        popup.Init(gameObject);
    }


    public IEnumerator ShakePhoneCoroutine(GameObject phone)
    {
        float timer = 0f;
        var prevPos = phone.transform.position;
        while (timer < shakeDuration)
        {
            phone.transform.position = prevPos + Random.insideUnitSphere * shakeAmount;
            timer += Time.deltaTime;
            yield return null;
        }
        phone.transform.position = prevPos;
    }

    public void OnTouchPhonePhase1()
    {
        var popup = Instantiate(eatrandAlarmPopupPrefab, transform);
        popup.Init("식당 사장님들은 주목!\r\n식당 운영이 막막한가요?\r\n잇트랜드 맛집 랭킹에 가입해보세요!");
        popup.transform.SetSiblingIndex(0);
    }

    public void OnPhase1End()
    {
        var popup = Instantiate(tutorialPhase1RewardPopupPrefab, transform);
        popup.Init();
        popup.transform.SetSiblingIndex(0);
    }
    #endregion
    #region Phase2
    public void OnTouchPhonePhase2()
    {
        var popup = Instantiate(eatrandAlarmPopupPrefab, transform);
        popup.Init("손님을 많이 끌어들이는 법!\r\n맛집 랭킹을 올리는 방법은\r\n이 가이드에서 확인하실 수 있습니다!");
        popup.transform.SetSiblingIndex(0);
    }

    #endregion
}
