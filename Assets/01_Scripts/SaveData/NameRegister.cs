using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NameRegister : MonoBehaviour
{
    [SerializeField] private TMP_InputField nameInput;
    [SerializeField] private Button registButton, theme1Button;

    private void Start()
    {
        nameInput.characterLimit = 8;
        registButton.onClick.RemoveAllListeners();
        registButton.onClick.AddListener(OnRegistButtonTouch);
        RegexFilter.Init();

        if(UserDataManager.Instance.CurrentUserData.Name == null)
        {
            theme1Button.interactable = false;
        }
    }

    private void OnRegistButtonTouch()
    {
        var check = nameInput.text;
        if (!RegexFilter.SpecialStringFilter(check))
        {
            Debug.Log("특수문자 존재!");
            return;
        }
        if (!RegexFilter.BadWordFilter(check))
        {
            Debug.Log("욕설 금지!!");
            return;
        }
        Debug.Log("훌륭합니다!!");
        SaveName(check).Forget();
        theme1Button.interactable = true;
        transform.PopdownAnimation();
        return;
    }

    private async UniTask SaveName(string name)
    {
        await UserDataDAC.SaveUserName(name);
        UserDataManager.Instance.CurrentUserData.Name = name;
    }
}
