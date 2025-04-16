using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Directory = System.IO.Directory;

public class NetworkManager : Singleton<NetworkManager>
{
    public string CurrentToken => TokenManager.LoginToken;
    public string CurrentRefreshToken => TokenManager.RefreshToken;
    public string uuid => GuestLoginManager.UUID;
    public bool IsConnected => Application.internetReachability != NetworkReachability.NotReachable;
    
    
}