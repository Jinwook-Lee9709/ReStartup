using UnityEngine;
using UnityEngine.Pool;

public class FoodObject : MonoBehaviour, IPoolable
{
    [SerializeField] private SpriteRenderer renderer;
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
}