using UnityEngine;
using UnityEngine.UI;

public class TitleSceneManager : MonoBehaviour
{
    [SerializeField] private Button button;

    private void Start()
    {
        button.onClick.AddListener(OnButtonClick);
    }

    private void OnButtonClick()
    {
        var sceneManager = ServiceLocator.Instance.GetGlobalService<SceneController>();
        sceneManager.LoadSceneWithLoading(SceneIds.Dev3);
    }
}