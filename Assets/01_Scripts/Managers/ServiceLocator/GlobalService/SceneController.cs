using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class SceneController
{
    public void LoadSceneWithLoading(SceneIds sceneId, Func<UniTask> postLoadAction = null)
    {
        LoadLoadingScene(sceneId, postLoadAction).Forget();
    }

    public async UniTask LoadLoadingScene(SceneIds sceneId, Func<UniTask> postLoadAction = null)
    {
        var loadingSceneHandle = Addressables.LoadSceneAsync(SceneIds.Loading.ToString());
        try
        {
            await loadingSceneHandle.Task;
        }
        catch (Exception e)
        {
            Debug.LogError($"[LoadLoadingScene] Error while load loading scene: {e.Message}");
        }
        finally
        {
            loadingSceneHandle.Release();
        }

        var loadingController = ServiceLocator.Instance.GetSceneService<LoadingSceneManager>();
        if (loadingController != null)
            loadingController.StartSceneLoad(sceneId, postLoadAction);
        else
            Debug.LogError("Loading Scene Manager is null");
    }


    public void OnSceneLoaded()
    {
    }
}