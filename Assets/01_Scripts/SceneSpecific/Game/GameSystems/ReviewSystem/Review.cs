using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Review : MonoBehaviour
{
    private ReviewData data;

    public TextMeshProUGUI starText;
    public TextMeshProUGUI reviewScriptText;
    public TextMeshProUGUI dateText;
    public TextMeshProUGUI rankPointText;

    public void Init(ReviewData data)
    {
        this.data = data;
        starText.text = $"별점 : {data.stars}점";
        reviewScriptText.text = data.reviewMessage;
        dateText.text = data.date;
        rankPointText.text = $"{data.addPoint:+0;-0;0} 점";
    }

}
