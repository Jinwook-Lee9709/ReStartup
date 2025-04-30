using Cysharp.Threading.Tasks;
using Excellcube.EasyTutorial.Utils;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NameRegister : MonoBehaviour
{
    [SerializeField] private TMP_InputField nameInput;
    [SerializeField] private CreateNameWarningPopup warningTextPrefab;
    [SerializeField] private Button registButton;
    [SerializeField] private int nameLengthLimit;
    public GameObject rootParent;
    public CreateNamePopup parent;
    private void OnEnable()
    {
        nameInput.lineLimit = 10;
        registButton.onClick.RemoveAllListeners();
        registButton.onClick.AddListener(OnRegistButtonTouch);
        RegexFilter.Init();
    }

    private void OnRegistButtonTouch()
    {
        if(nameInput.text.Length >= nameLengthLimit)
        {
            WarningMessageActive("글자 수가 초과되었습니다.");
            return;
        }
        if(nameInput.text == "")
        {
            WarningMessageActive("이름을 입력해주세요.");
            return;
        }
        var check = nameInput.text;
        if (!RegexFilter.SpecialStringFilter(check))
        {
            WarningMessageActive("특수문자가 존재합니다.");
            return;
        }
        if (!RegexFilter.BadWordFilter(check))
        {
            WarningMessageActive("비속어가 포함 되어 있습니다.");
            return;
        }
        SaveName(check).Forget();
        TutorialEvent.Instance.Broadcast(Strings.tutorialCompeleteKey);
        parent.OnCancleAction?.Invoke();
        return;
    }

    private async UniTask SaveName(string name)
    {
        await UserDataDAC.SaveUserName(name);
        UserDataManager.Instance.CurrentUserData.Name = name;
    }

    private void WarningMessageActive(string message)
    {
        var warning = Instantiate(warningTextPrefab, parent.transform);
        warning.Init(message);
    }
}
