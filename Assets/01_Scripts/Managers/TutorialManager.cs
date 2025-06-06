using Cysharp.Threading.Tasks;
using Excellcube.EasyTutorial;
using Excellcube.EasyTutorial.Utils;
using System;
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
}

public class TutorialManager : MonoBehaviour
{
    [VInspector.Foldout("Public")]
    [SerializeField] private List<ECEasyTutorial> tutorials;
    [SerializeField] private TutorialSkipPopup tutorialSkipPopupPrefab;
    [SerializeField] private EatrandAlarmPopup eatrandAlarmPopupPrefab;
    [SerializeField] private GameObject touchShield;
    [SerializeField] private GameObject smartPhone;
    [SerializeField] private ConsumerManager consumerManager;
    private TutorialPhase currentPhase = TutorialPhase.Phase1;
    [VInspector.EndFoldout]
    static public bool IsCompleteTutorial
    {
        get
        {
            return (PlayerPrefs.GetInt("ECET_CLEAR_ALL", 0) == 1);
        }
    }
    private void Start()
    {
        TutorialInit().Forget();
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
        popup.Init(() =>
        {
            action?.Invoke();
            consumerManager.StartSpawnRoutine();
            PlayerPrefs.SetInt("ECET_CLEAR_ALL", 1);
            Destroy(gameObject);
            TutorialEndTask().Forget();
        });
    }

    public async UniTask TutorialEndTask()
    {
        await UniTask.NextFrame();
        ServiceLocator.Instance.GetSceneService<GameManager>().uiManager.BroadcastMessage("ScrollRectLock");
    }

    public void TutorialOrderComplete()
    {
        TutorialEvent.Instance.Broadcast(Strings.tutorialCompeleteKey);
    }
    private async UniTask EndPhaseCoroutine()
    {
        await UniTask.NextFrame();

        if (currentPhase == TutorialPhase.Phase6)
        {
            consumerManager.StartSpawnRoutine();
            Destroy(gameObject);
            return;
        }
        PlayerPrefs.SetInt("ECET_CLEAR_ALL", 0);
        currentPhase++;
        tutorials[(int)currentPhase].gameObject.SetActive(true);
        tutorials[(int)currentPhase].StartTutorial();
    }

    public void TutorialPhaseDone()
    {
        EndPhaseCoroutine().Forget();
    }

    public async UniTask TutorialInit()
    {
        await UniTask.NextFrame();
        if(IsCompleteTutorial)
        {
            consumerManager.StartSpawnRoutine();
            Destroy(gameObject);
        }
    }

    public void ShakeSmartPhone()
    {
        StartCoroutine(ShakePhoneCoroutine(smartPhone));
        Handheld.Vibrate();
    }

    #region Phase1
    [VInspector.Foldout("Phase1")]
    [SerializeField][Range(0f, 3f)] private float shakeDuration;
    [SerializeField][Range(0f, 10f)] private float shakeAmount;
    [SerializeField] private CreateNamePopup createNamePopupPrefab;
    [SerializeField] private TutorialPhase1RewardPopup tutorialPhase1RewardPopupPrefab;
    [VInspector.EndFoldout]
    public void CreateName()
    {
        var popup = Instantiate(createNamePopupPrefab, transform);
        popup.Init(gameObject).Forget();
    }


    public IEnumerator ShakePhoneCoroutine(GameObject phone)
    {
        float timer = 0f;
        var prevPos = phone.transform.position;
        while (timer < shakeDuration)
        {
            phone.transform.position = prevPos +UnityEngine.Random.insideUnitSphere * shakeAmount;
            timer += Time.deltaTime;
            yield return null;
        }
        phone.transform.position = prevPos;
    }

    public void OnTouchPhonePhase1()
    {
        var popup = Instantiate(eatrandAlarmPopupPrefab, transform);
        popup.Init(LZString.GetUIString(string.Format(Strings.tutorialPopupFormat, 3.ToString())), LZString.GetUIString(string.Format(Strings.tutorialPopupFormat,1.ToString())));
        popup.transform.SetSiblingIndex(0);
    }

    public void OnPhase1End()
    {
        var popup = Instantiate(tutorialPhase1RewardPopupPrefab, transform);
        popup.Init();
    }
    #endregion


    #region Phase2
    [VInspector.Foldout("Phase2")]
    [SerializeField] private TutorialGuidePopup tutorialGuidePopupPrefab;
    [VInspector.EndFoldout]
    public void OnTouchPhonePhase2()
    {
        var popup = Instantiate(eatrandAlarmPopupPrefab, transform);
        popup.Init(LZString.GetUIString(string.Format(Strings.tutorialPopupFormat, 4.ToString())), LZString.GetUIString(string.Format(Strings.tutorialPopupFormat, 2.ToString())));
        popup.transform.SetSiblingIndex(0);
    }

    public void TutorialGuideImagePopup()
    {
        var popup = Instantiate(tutorialGuidePopupPrefab, transform);
        popup.Init();
    }

    #endregion


    #region Phase6
    [VInspector.Foldout("Phase6")]

    [SerializeField] private TutorialCompletePopup completePopup;

    [VInspector.EndFoldout]

    public void TutorialCompletePopup()
    {
        var popup = Instantiate(completePopup, transform);
        popup.Init();
    }
    #endregion
}
