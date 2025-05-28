using System;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AddressableAssets;
using Slider = UnityEngine.UI.Slider;

public class LoadingSceneManager : MonoBehaviour
{
    private static readonly string titleSceneId = "TitleScene";
    private static readonly string loadingStringFormat = "LOADING...{0}%";
    [SerializeField] private Slider progressBar;
    [SerializeField] private TextMeshProUGUI progressText;
    
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

    private async UniTask LoadTargetSceneAsync(
        SceneIds targetSceneId, 
        Func<UniTask> postLoadAction = null)
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
                    progressBar.value = sceneHandle.PercentComplete * 100;
                    progressText.text = String.Format(loadingStringFormat, Math.Truncate(progressBar.value));
                    Debug.Log(progressBar.value);
                    await UniTask.Yield();
                }

                await UniTask.Delay(TimeSpan.FromSeconds(2));
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