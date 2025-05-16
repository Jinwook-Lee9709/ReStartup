using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class UserReset : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private Button startButton;
    [SerializeField] private Button loginButton;
    [SerializeField] private Button retryButton;

    private void Start()
    {
        button.onClick.AddListener(OnDeleteButtonTouched);
        button.interactable = GuestLoginManager.ReadUUID();

        if (PlayerPrefs.GetInt("ECET_CLEAR_ALL") != 1 && button.interactable)
        {
            DeleteUserTask().Forget();
        }
        
    }
    
    public void OnDeleteButtonTouched()
    {
        DeleteUserTask().Forget();
    }

    private async UniTask DeleteUserTask()
    {
        button.interactable = false;
        PlayerPrefs.SetInt("ECET_CLEAR_ALL", 0);
        TokenManager.ReadToken();
        bool isUUIDExist = GuestLoginManager.ReadUUID();
        
        if (string.IsNullOrEmpty(TokenManager.LoginToken))
            return;
        button.interactable = false;
        var response = await RestApiService.PostAsyncWithToken<bool>(Endpoints.DeleteUserUrl);
        if (response.ResponseCode != ResponseType.Success)
            return;
        TokenManager.DeleteToken();
        GuestLoginManager.DeleteUUID();
        
        startButton.gameObject.SetActive(false);
        startButton.interactable = true;
        loginButton.gameObject.SetActive(true);
        retryButton.gameObject.SetActive(false);
        
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
