using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbySceneManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private List<Button> themeSelectButtons;
    [SerializeField] private SceneIds sceneId = SceneIds.Dev0;
    
    void Start()
    {
        var goldText = UserDataManager.Instance.CurrentUserData.Money.ToString();
        text.text = $"Money = {goldText}";
        for (int i = 0; i < themeSelectButtons.Count; i++)
        {
            var button = themeSelectButtons[i];
            int id = i+1;
            button.onClick.AddListener(() => OnThemeSelectButtonTouched(id));
        }
    }

    private void OnThemeSelectButtonTouched(int id)
    {
        PlayerPrefs.SetInt("Theme", id);
        var sceneManager = ServiceLocator.Instance.GetGlobalService<SceneController>();
        sceneManager.LoadSceneWithLoading(sceneId, GameSceneLoader.BeforeGameSceneLoad);
    }

    
    
}
