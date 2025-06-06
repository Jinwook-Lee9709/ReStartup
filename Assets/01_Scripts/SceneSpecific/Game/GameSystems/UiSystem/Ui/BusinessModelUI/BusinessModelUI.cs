using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BusinessModelUI : MonoBehaviour
{
    public GameObject packageContent;
    [SerializeField] List<RectTransform> packageCards = new();
    [SerializeField] List<ContentSizeFitter> contentSizeFitters = new();
    public GameObject busunessModelUIBuyPopup;
    public GameObject busunessModelUIPackagePopup;
    private float foldWidth = 900f;
    public GameObject notEnoughCostPopup;
    public GameObject limitationPackagePopup;
    private Transform uicanvers;
    // Start is called before the first frame update
    void Start()
    {
        var uiManager = ServiceLocator.Instance.GetSceneService<GameManager>().uiManager;
        var cenversWidth = uiManager.canvas.GetComponent<RectTransform>().rect.width;
        uicanvers = uiManager.canvas.transform;
        var contentPadding = packageContent.GetComponent<HorizontalLayoutGroup>().padding;
        var newWidth = cenversWidth - (contentPadding.left + contentPadding.right);
        foreach( var card in packageCards )
        {
            var newRectTransform = new Vector2(newWidth, card.sizeDelta.y);
            card.sizeDelta = newRectTransform;
        }
        if (cenversWidth < foldWidth)
        {
            foreach( var fitter in contentSizeFitters )
            {
                fitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            }
        }
    }
    public void OnNotEnoughCostPopup()
    {
        Instantiate(notEnoughCostPopup, uicanvers);
    }
    public void OnLimitationPackagePopup()
    {
        Instantiate(limitationPackagePopup, uicanvers);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
