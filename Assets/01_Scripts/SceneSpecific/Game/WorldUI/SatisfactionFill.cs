using UnityEngine;

public class SpriteMaskController : MonoBehaviour
{
    public Material maskMaterial; // 적용할 Material
    [Range(0, 1)] public float fillAmount = 1.0f; // 초기 Fill 값

    private void Start()
    {
        maskMaterial = GetComponent<Material>();
    }

    private void Update()
    {
        // _FillAmount 값을 동적으로 설정하여 자르기 효과 적용
        if (maskMaterial != null)
        {
            maskMaterial.SetFloat("_FillAmount", fillAmount);
        }

        // 테스트용: 자동 감소 (왼쪽 마우스 버튼 누를 때)
        if (Input.GetMouseButton(0))
        {
            fillAmount = Mathf.Max(0, fillAmount - Time.deltaTime * 0.5f); // 0.5 속도로 감소
        }

        // 오른쪽 마우스 버튼으로 fillAmount 다시 증가
        if (Input.GetMouseButton(1))
        {
            fillAmount = Mathf.Min(1, fillAmount + Time.deltaTime * 0.5f); // 0.5 속도로 증가
        }
    }
}