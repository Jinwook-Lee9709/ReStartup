using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
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
        bool isUUIDExist = GuestLoginManager.ReadUUID();
        
        if (string.IsNullOrEmpty(TokenManager.LoginToken))
            return;
        button.interactable = false;
        var response = await RestApiService.PostAsyncWithToken<bool>(Endpoints.DeleteUserUrl);
        if (response.ResponseCode == ResponseType.Success)
        {
            TokenManager.DeleteToken();
            GuestLoginManager.DeleteUUID();
        }
        button.interactable = true;
    }

    private static async Task<bool> LoginAsGuest()
    {
        var token = await UserAuthController.LoginAsGuestTask();
        if (token == null)
            return true;
        TokenManager.SaveToken(token.token, token.refreshToken);
        return false;
    }
}
