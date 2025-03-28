using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using Slider = UnityEngine.UI.Slider;

public class LoadingSceneManager : MonoBehaviour
{
    [SerializeField] private Slider progressBar;

    private void Awake()
    {
        ServiceLocator.Instance.RegisterSceneService(this);
    }

    private void OnDestroy()
    {
        ServiceLocator.Instance.UnRegisterSceneService<LoadingSceneManager>();
    }

    public void StartSceneLoad(SceneIds targetSceneId)
    {
        ServiceLocator.Instance.ClearSceneServices();
        LoadTargetSceneAsync(targetSceneId).Forget();
    }

    public async UniTask LoadTargetSceneAsync(SceneIds targetSceneId)
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

        var sceneIndex = (int)targetSceneId;
        if (sceneIndex >= (int)SceneIds.Theme1 && sceneIndex <= (int)SceneIds.Dev3)
        {
            await UniTask.WaitUntil(() => SceneManager.GetActiveScene().isLoaded);
            var obj = GameObject.FindWithTag(Strings.GameManagerTag);
            if (obj == null)
            {
                Debug.LogError("GameManager object not found in the scene!");
                return;
            }

            ServiceLocator.Instance.RegisterSceneService(obj.GetComponent<GameManager>());
        }
    }
}