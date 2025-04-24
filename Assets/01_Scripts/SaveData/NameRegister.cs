using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NameRegister : MonoBehaviour
{
    [SerializeField] private TMP_InputField nameInput;
    [SerializeField] private Button registButton;

    private void Start()
    {
        nameInput.characterLimit = 8;
        registButton.onClick.RemoveAllListeners();
        registButton.onClick.AddListener(OnRegistButtonTouch);
        RegexFilter.Init();
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
        return;
    }

    private async UniTask SaveName(string name)
    {
        await UserDataDAC.SaveUserName(name);
    }
}
