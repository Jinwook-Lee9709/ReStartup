using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class TMPDropShadow : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI tmp;

    private void Start()
    {
        Material originalMaterial = tmp.material;
        Material newMaterial = new Material(originalMaterial);
        tmp.material = newMaterial;
        newMaterial.SetFloat(ShaderUtilities.ID_FaceDilate, 5.3f); // 텍스트 외곽선
        newMaterial.SetFloat(ShaderUtilities.ID_OutlineSoftness, 0.5f); // 부드러운 외곽선
        newMaterial.SetColor(ShaderUtilities.ID_OutlineColor, Color.black); // 그림자 색상
    }
    
}
