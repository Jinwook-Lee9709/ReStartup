using System;
using System.Collections;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Button = UnityEngine.UI.Button;

public enum SpumCharacter
{
    Apologize,
    Construct,
    ConstructComplete,
    FoodResearch,
    FoodResearchComplete,
    HireEmployee,
    HireEmployeeComplete,
}

public class AlertPopup : MonoBehaviour
{
    private static AlertPopup instance;
    
    [FormerlySerializedAs("cavnas")] [SerializeField] private Canvas canvas;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private Button background;
    [SerializeField] private Image panel;
    [SerializeField] private SerializedDictionary<SpumCharacter, GameObject> spumCharacters;
    private Image backgroundImage;

    private bool isTouchable
    {
        get => background.interactable;
        set => background.interactable = value;
    }
    private bool isError;
    private SpumCharacter currentCharacter;
    
    private void Start()
    {  
        if(instance != null)
            Destroy(gameObject);
        
        instance = this;
        ServiceLocator.Instance.RegisterGlobalService(this);
        DontDestroyOnLoad(gameObject);
        backgroundImage = background.GetComponent<Image>();
        background.onClick.AddListener(OnCloseButtonTouched);
        canvas.gameObject.SetActive(false);
    }
    
    public void PopUp(string title, string description, SpumCharacter spumCharacter = SpumCharacter.Apologize, bool isTouchable = true, bool isError = false)
    {
        titleText.text = title;
        descriptionText.text = description;
        this.isTouchable = isTouchable;
        this.isError = isError;
        canvas.gameObject.SetActive(true);

        ActivateSpumCharacterObject(spumCharacter);
        
        DoPopupAnimation();
    }
    private void ActivateSpumCharacterObject(SpumCharacter spumCharacter)
    {
        currentCharacter = spumCharacter;
        foreach (var pair in spumCharacters)
        {
            if (pair.Key == spumCharacter)
            {
                pair.Value.SetActive(true);
            }
            else
            {
                pair.Value.SetActive(false);
            }
        }
    }

    public void ChangeCharacter(SpumCharacter spumCharacter)
    {
        var original = spumCharacters[currentCharacter];
        var target = spumCharacters[spumCharacter];
        var originalScale = original.transform.localScale;
        var targetScale = target.transform.localScale;
        original.transform.PopdownAnimation(duration:0.2f, onComplete: () =>
        {
            original.transform.localScale = originalScale;
            original.SetActive(false);
            target.SetActive(true);
            target.transform.localScale = Vector3.zero;
            target.transform.PopupAnimation(duration:0.2f, scale: targetScale.x);
        });
        currentCharacter = spumCharacter;
        
    }

    public void ChangeText(string title, string description)
    {
        titleText.text = title;
        descriptionText.text = description;
    }
    
    private void DoPopupAnimation()
    {
        if (background != null)
        {
            backgroundImage.FadeInAnimation();
        }

        if (panel != null)
        {
            panel.transform.PopupAnimation();
        }
    }


    private void OnCloseButtonTouched()
    {
        if (!isError)
            ClosePopup();
        else
            ClosePopupAndGotoTitle();
    }
    
    public void ClosePopup(Action action = null)
    {
        var backgroundImage = background.GetComponent<Image>();
        backgroundImage.FadeOutAnimation();
        panel.transform.PopdownAnimation(onComplete: () =>
        {
            canvas.gameObject.SetActive(false);
            action?.Invoke();
        });
    }

    private void ClosePopupAndGotoTitle()
    {
        Action gotoTitle = () =>
        {
            if (SceneManager.GetActiveScene().buildIndex == 0)
                return;
            var sceneManager = ServiceLocator.Instance.GetGlobalService<SceneController>();
            sceneManager.LoadSceneWithLoading(SceneIds.Title);
        };
        ClosePopup(gotoTitle);
    }

    public void EnableTouch()
    {
        isTouchable = true;
    }
    
    public void DisableTouch()
    {
        isTouchable = false;
    }


}
