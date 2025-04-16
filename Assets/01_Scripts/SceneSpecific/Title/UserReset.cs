using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class UserReset : MonoBehaviour
{
    [SerializeField] private Button button;

    private void Start()
    {
        button.onClick.AddListener(OnDeleteButtonTouched);
    }
    
    public void OnDeleteButtonTouched()
    {
        DeleteUserTask().Forget();
    }

    private async UniTask DeleteUserTask()
    {
        TokenManager.ReadToken();
        if (string.IsNullOrEmpty(TokenManager.LoginToken))
            return;
        button.interactable = false;
        bool isSucceed = await RestApiService.PostAsyncWithToken<bool>(Endpoints.DeleteUserUrl);
        if (isSucceed)
        {
            TokenManager.DeleteToken();
            GuestLoginManager.DeleteUUID();
        }
        button.interactable = true;
    }
}
