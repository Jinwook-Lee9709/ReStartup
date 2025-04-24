using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollRestrictor : MonoBehaviour
{
    private ScrollRect scrollRect; 
    public RectTransform viewport;
    public RectTransform content;

    private void Start()
    {
        scrollRect = GetComponent<ScrollRect>();
    }
    
    private void Update()
    {
        scrollRect.horizontal = viewport.rect.size.x < content.rect.size.x;
    }
}
