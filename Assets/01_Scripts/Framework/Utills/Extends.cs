using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public static class Extends
{
    public static bool IsArrive(this NavMeshAgent agent, Transform target)
    {
        Vector2 agentPosition = agent.transform.position;
        Vector2 targetPosition = target.position;
        return Vector2.SqrMagnitude(agentPosition - targetPosition) <= Mathf.Sqrt(agent.stoppingDistance);
    }

    public static bool IsArrive(this NavMeshAgent agent, Vector2 target)
    {
        Vector2 agentPosition = agent.transform.position;
        return Vector2.SqrMagnitude(agentPosition - target) <= Mathf.Sqrt(agent.stoppingDistance);
    }

    public static InteractPermission WorkTypeToPermission(this WorkType workType)
    {
        switch (workType)
        {
            case WorkType.All:
                var permission = InteractPermission.None;
                permission |= InteractPermission.HallEmployee;
                permission |= InteractPermission.KitchenEmployee;
                permission |= InteractPermission.PaymentEmployee;
                permission |= InteractPermission.Player;
                return permission;
            case WorkType.Payment:
                return InteractPermission.PaymentEmployee;
            case WorkType.Hall:
                return InteractPermission.HallEmployee;
            case WorkType.Kitchen:
                return InteractPermission.KitchenEmployee;
        }

        return InteractPermission.None;
    }

    public static void InitializeLocalTransform(this Transform obj)
    {
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localScale = Vector3.one;
    }

    public static void SetParentAndInitialize(this Transform obj, Transform parent)
    {
        obj.transform.SetParent(parent);
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localScale = Vector3.one;
    }

    public static void PopupAnimation(this Transform transform, float scale = 1.0f, float duration = 0.5f)
    {
        transform.gameObject.SetActive(true);
        transform.DOScale(scale, duration).SetEase(Ease.InOutElastic);
    }

    public static void PopdownAnimation(this Transform transform, float scale = 0f, float duration = 0.5f,
        Action onComplete = null)
    {
        transform.DOScale(scale, duration).SetEase(Ease.InOutElastic)
            .OnComplete(() => transform.gameObject.SetActive(false))
            .OnComplete(() => onComplete?.Invoke());
    }

    public static void FadeInAnimation(this Image image, float opacity = 0.5f, float duration = 0.5f)
    {
        image.gameObject.SetActive(true);
        image.DOFade(opacity, duration).SetEase(Ease.InOutQuad);
    }

    public static void FadeOutAnimation(this Image image, float duration = 0.5f, Action onComplete = null)
    {
        image.DOFade(0f, duration).SetEase(Ease.InOutQuad)
            .OnComplete(() => image.gameObject.SetActive(false))
            .OnComplete(() => onComplete?.Invoke());
    }
    
    
    public static bool CheckOverlap(this RectTransform rect, RectTransform canvas)
    {
        Vector3[] corners = new Vector3[4];
        rect.GetWorldCorners(corners);

        foreach (var corner in corners)
        {
            Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(Camera.main, corner);
            if(RectTransformUtility.RectangleContainsScreenPoint(canvas, screenPoint,Camera.main))
            {
                return true;
            }
        }
        return false;
    }
    
   
}