using UnityEngine;
using UnityEngine.Pool;

public class FoodObject : MonoBehaviour, IPoolable
{
    [SerializeField] private SpriteRenderer renderer;
    [SerializeField] private Animator animator;
    private ObjectPool<GameObject> pool;
    private FoodData data;
    

    public void SetSprite(Sprite sprite)
    {
        renderer.sprite = sprite;
    }

    public void SetPool(ObjectPool<GameObject> pool)
    {
        this.pool = pool;
    }

    public void Release()
    {
        pool.Release(gameObject);
    }

    public void ShowEffect()
    {
        animator.gameObject.SetActive(true);
    }
    
    public void HideEffect()
    {
        animator.gameObject.SetActive(false);
    }
}