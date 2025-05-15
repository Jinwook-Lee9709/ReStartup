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
        int idx = 1;
        if(nameInput.text.Length >= nameLengthLimit)
        {
            WarningMessageActive(LZString.GetUIString(string.Format(Strings.nameWarningFormat,idx.ToString())));
            return;
        }
        idx++;
        if(nameInput.text == "")
        {
            WarningMessageActive(LZString.GetUIString(string.Format(Strings.nameWarningFormat, idx.ToString())));
            return;
        }
        idx++;
        var check = nameInput.text;
        if (!RegexFilter.SpecialStringFilter(check))
        {
            WarningMessageActive(LZString.GetUIString(string.Format(Strings.nameWarningFormat, idx.ToString())));
            return;
        }
        idx++;
        if (!RegexFilter.BadWordFilter(check))
        {
            WarningMessageActive(LZString.GetUIString(string.Format(Strings.nameWarningFormat, idx.ToString())));
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
        ServiceLocator.Instance.GetSceneService<GameManager>().rankSystemManager.InitPlayerName();
    }

    private void WarningMessageActive(string message)
    {
        var warning = Instantiate(warningTextPrefab, parent.transform);
        warning.Init(message);
    }
}
