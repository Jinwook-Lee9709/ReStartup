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
    [SerializeField] private List<ECEasyTutorial> tutorials;
    [SerializeField] private TutorialSkipPopup tutorialSkipPopupPrefab;
    [SerializeField] private GameObject touchShield;
    private TutorialPhase currentPhase = TutorialPhase.Phase1;

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
    #region Phase1
    [VInspector.Foldout("Phase1")]
    [SerializeField] private GameObject smartPhone;
    [SerializeField][Range(0f, 3f)] private float shakeDuration;
    [SerializeField][Range(0f, 3f)] private float shakeAmount;
    [SerializeField] private CreateNamePopup createNamePopupPrefab;
    [SerializeField] private EatrandSignInPopup eatrandSignInPopupPrefab;
    [SerializeField] private TutorialPhase1RewardPopup tutorialPhase1RewardPopupPrefab;
    [VInspector.EndFoldout]
    public void CreateName()
    {
        var popup = Instantiate(createNamePopupPrefab, transform);
        popup.Init(gameObject);
    }

    public void ShakeSmartPhone()
    {
        StartCoroutine(ShakePhoneCoroutine(smartPhone));
        Handheld.Vibrate();
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

    public void OnTouchPhone()
    {
        var popup = Instantiate(eatrandSignInPopupPrefab, transform);
        popup.Init();
        popup.transform.SetSiblingIndex(0);
    }

    public void OnPhase1End()
    {
        var popup = Instantiate(tutorialPhase1RewardPopupPrefab, transform);
        popup.Init();
        popup.transform.SetSiblingIndex(0);
    }
    #endregion
}
