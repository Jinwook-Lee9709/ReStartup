using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    [SerializeField] private SpriteRenderer foregroundBar;
    [SerializeField] private InteractableObjectBase target;
    private float originalWidth;
    
    private void Start()
    {
        originalWidth = foregroundBar.transform.localScale.x;
        target.OnInteractedEvent += OnValueChanged;
        target.OnClearWorkEvent += OnWorkCleared; 
        gameObject.SetActive(false);
    }

    private void OnValueChanged(float value)
    {
        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }
        foregroundBar.transform.localScale = new Vector3(value, 1, 1);
        foregroundBar.transform.localPosition = new Vector3((value - 1) * originalWidth / 2, 0, 0);
    }
    
    private void OnWorkCleared()
    {
        if (gameObject.activeSelf)
        {
            gameObject.SetActive(false);
        }
    }
    
    private void OnDestroy()
    {
        if (target != null)
        {
            target.OnInteractedEvent -= OnValueChanged;
            target.OnClearWorkEvent -= OnWorkCleared;
        }
    }
}