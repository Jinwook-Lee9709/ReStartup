using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NameRegister : MonoBehaviour
{
    [SerializeField] private TMP_InputField nameInput;
    [SerializeField] private TextMeshProUGUI warningText;
    [SerializeField] private Button registButton, theme1Button;

    private void OnEnable()
    {
        warningText.gameObject.SetActive(false);
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
            warningText.gameObject.SetActive(true);
            warningText.text = "특수문자 존재!";
            return;
        }
        if (!RegexFilter.BadWordFilter(check))
        {
            warningText.gameObject.SetActive(true);
            warningText.text = "욕설 금지!!";
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
