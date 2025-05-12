using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BusinessModelUI : MonoBehaviour
{
    public GameObject packageContent;
    [SerializeField] List<RectTransform> packageCards = new();
    [SerializeField] List<ContentSizeFitter> contentSizeFitters = new();
    private float foldWidth = 900f;
    // Start is called before the first frame update
    void Start()
    {
        var uiManager = ServiceLocator.Instance.GetSceneService<GameManager>().uiManager;
        var cenversWidth = uiManager.canvas.GetComponent<RectTransform>().rect.width;
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

    // Update is called once per frame
    void Update()
    {
        
    }
}
