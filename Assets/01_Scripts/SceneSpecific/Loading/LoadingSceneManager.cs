using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using Slider = UnityEngine.UI.Slider;

public class LoadingSceneManager : MonoBehaviour
{
    private static readonly string titleSceneId = "TitleScene";
    [SerializeField] private Slider progressBar;
    
    private void Awake()
    {
        ServiceLocator.Instance.RegisterSceneService(this);
    }

    private void OnDestroy()
    {
        ServiceLocator.Instance.UnRegisterSceneService<LoadingSceneManager>();
    }

    public void StartSceneLoad(SceneIds targetSceneId, Func<UniTask> postLoadAction = null)
    {
        ServiceLocator.Instance.ClearSceneServices();
        LoadTargetSceneAsync(targetSceneId, postLoadAction).Forget();
    }

    public async UniTask LoadTargetSceneAsync(SceneIds targetSceneId, Func<UniTask> postLoadAction = null)
    {
        try
        {
            if (postLoadAction != null)
                await postLoadAction();
        } 
        catch (Exception e)
        {
            return;
        }

        
        if (targetSceneId != SceneIds.Title)
        {
            var sceneHandle = Addressables.LoadSceneAsync(targetSceneId.ToString());
            try
            {
                while (!sceneHandle.IsDone)
                {
                    progressBar.value = sceneHandle.PercentComplete;
                    await UniTask.Yield();
                }

                await UniTask.Delay(TimeSpan.FromSeconds(1));
            }
            catch (Exception e)
            {
                Debug.LogError($"[LoadTargetSceneAsync] Error while loading scene: {e.Message}");
                throw;
            }
        }
        else
        {
            try
            {
                var handle = SceneManager.LoadSceneAsync(titleSceneId);
                while (!handle.isDone)
                {
                    progressBar.value = handle.progress;
                    await UniTask.Yield();
                }
                await UniTask.Delay(TimeSpan.FromSeconds(1));
            }
            catch (Exception e)
            {
                Debug.LogError($"[LoadTargetSceneAsync] Error while loading scene: {e.Message}");
                throw;
            }
        }

        await UniTask.WaitUntil(() => SceneManager.GetActiveScene().isLoaded);
    }
}