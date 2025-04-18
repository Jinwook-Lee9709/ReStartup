using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Button = UnityEngine.UI.Button;

public class AlertPopup : MonoBehaviour
{
    private static AlertPopup instance;
    
    [FormerlySerializedAs("cavnas")] [SerializeField] private Canvas canvas;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private Button background;
    [SerializeField] private Image panel;
    private Image backgroundImage;
    
    private void Start()
    {  
        if(instance != null)
            Destroy(gameObject);
        
        instance = this;
        ServiceLocator.Instance.RegisterGlobalService(this);
        DontDestroyOnLoad(gameObject);
        backgroundImage = background.GetComponent<Image>();
        background.onClick.AddListener(Close);
        canvas.gameObject.SetActive(false);
    }
    
    public void PopUp(string title, string description)
    {
        titleText.text = title;
        descriptionText.text = description;
        canvas.gameObject.SetActive(true);
        
        if (background != null)
        {
            backgroundImage.FadeInAnimation();
        }

        if (panel != null)
        {
            panel.transform.PopupAnimation();
        }
        
    }

    private void Close()
    {
        var backgroundImage = background.GetComponent<Image>();
        backgroundImage.FadeOutAnimation();
        panel.transform.PopdownAnimation(onComplete: () =>
        {
            canvas.gameObject.SetActive(false);
            if (SceneManager.GetActiveScene().buildIndex == 0)
                return;
            var sceneManager = ServiceLocator.Instance.GetGlobalService<SceneController>();
            sceneManager.LoadSceneWithLoading(SceneIds.Title);
        });
       
    }
    
    
}
