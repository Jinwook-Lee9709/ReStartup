using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AI;

public class ConsumerFSM : MonoBehaviour
{
    public enum ConsumerState
    {
        None,
        Waiting,
        BeforeOrder,
        AfterOrder,
        Eatting,
        WaitForPay,
        Paying,
        Exit,

        WaitingPairMealEnd,
        WaitingPayLine,
        TalkingAbout,
    }

    public enum Satisfaction
    {
        None = -1,
        High,
        Middle,
        Low
    }

    public event Action<Consumer> OnSeatEvent;
    public ConsumerManager consumerManager;
    public BuffManager buffManager;

    [SerializeField]
    private List<float> satisfactionChangeLimit = new()
    {
        15f,
        0f
    };

    [SerializeField]
    private List<float> satisfactionColorChangeFlagTimes = new()
    {
        30f,
        25f,
        15f,
    };


    [SerializeField] public ConsumerState currentStatus = ConsumerState.Waiting;
    [SerializeField] private Satisfaction currentSatisfaction = Satisfaction.None;
    [SerializeField] private PaymentText paymentTextPrefab;
    [SerializeField] public SatisfactionIcon satisfactionIcon;
    [SerializeField] public GameObject modelParent;
    [SerializeField] private GameObject textBubblePrefab;
    private NavMeshAgent agent;
    private CashierCounter cashierCounter;
    private Consumer consumer;
    public ConsumerData consumerData = new();
    private SPUM_Prefabs model;

    public SPUM_Prefabs Model
    {
        get => model;
        set => model = value;
    }

    private bool isOnSeat;
    private bool isPaying;
    public bool isTip;
    public bool alreadyTip;
    private Vector2 targetPivot;
    private float prevXPos;

    public Satisfaction CurrentSatisfaction
    {
        get => currentSatisfaction;
        set
        {
            var prevSatisfaction = currentSatisfaction;
            currentSatisfaction = value;
            switch (currentSatisfaction)
            {
                case Satisfaction.High:
                    switch (consumerData.GuestType)
                    {
                        case GuestType.Influencer:
                        case GuestType.BadGuest:
                        case GuestType.PromotionGuest:
                            ConsumerScriptActive(
                                string.Format(Strings.orderTextFormat, UnityEngine.Random.Range(0, 2),
                                    consumerData.GuestId), () => { consumer.currentTable.HideIcon(); },
                                () => { consumer.currentTable.ShowIcon(); });
                            break;
                    }

                    break;
                case Satisfaction.Middle:
                    ConsumerScriptActive(
                        string.Format(Strings.servingDelayTextFormat, UnityEngine.Random.Range(0, 2),
                            consumerData.GuestId), () => { consumer.currentTable.HideIcon(); },
                        () => { consumer.currentTable.ShowIcon(); });
                    break;
                case Satisfaction.Low:
                    break;
            }
        }
    }

    public ConsumerState CurrentStatus
    {
        get => currentStatus;
        set
        {
            var prevStatus = currentStatus;

            switch (value)
            {
                case ConsumerState.Waiting:
                    model.PlayAnimation(PlayerState.IDLE, 0);
                    break;
                case ConsumerState.BeforeOrder:
                    model.PlayAnimation(PlayerState.MOVE, 0);
                    consumerManager.OnChangeConsumerState(consumer, ConsumerState.BeforeOrder);
                    if (consumer.pairData?.partner == consumer)
                    {
                        break;
                    }

                    consumerManager.OnWaitingLineUpdate(consumer);
                    var permission = InteractPermission.Consumer;
                    if (consumer.pairData?.owner == consumer)
                    {
                        consumer.pairData.partner.FSM.CurrentStatus = ConsumerState.BeforeOrder;
                        targetPivot = consumer.pairData.partner.currentTable.GetInteractablePoints(permission)[0]
                            .transform.position;
                        consumer.pairData.partner.GetComponent<NavMeshAgent>().SetDestination(targetPivot);
                    }

                    targetPivot = consumer.currentTable.GetInteractablePoints(permission)[0].transform.position;
                    agent.SetDestination(targetPivot);

                    break;
                case ConsumerState.AfterOrder:
                    consumerManager.OnChangeConsumerState(consumer, ConsumerState.AfterOrder);
                    break;
                case ConsumerState.Eatting:
                    consumerManager.OnChangeConsumerState(consumer, ConsumerState.Eatting);
                    StartCoroutine(EattingCoroutine());
                    break;
                case ConsumerState.WaitForPay:
                    transform.localScale = new Vector3(1, 1, 1);
                    model.PlayAnimation(PlayerState.MOVE, 0);
                    consumerManager.OnChangeConsumerState(consumer, ConsumerState.WaitForPay);
                    consumerManager.OnEndMeal(consumer);
                    break;
                case ConsumerState.Paying:
                    consumerManager.OnChangeConsumerState(consumer, ConsumerState.Paying);
                    break;
                case ConsumerState.Exit:
                    model.PlayAnimation(PlayerState.IDLE, 0);
                    model.PlayAnimation(PlayerState.MOVE, 0);
                    consumerManager.OnChangeConsumerState(consumer, ConsumerState.Exit);
                    switch (currentSatisfaction)
                    {
                        case Satisfaction.High:
                            UserDataManager.Instance.AddConsumerCnt(true);
                            ServiceLocator.Instance.GetSceneService<GameManager>().MissionManager
                                .OnEventInvoked(MissionMainCategory.GuestSatisfied, 1);
                            break;
                        case Satisfaction.Low:
                            UserDataManager.Instance.AddConsumerCnt(false);
                            if (consumer.pairData == null)
                            {
                                consumerManager.workFlowController.CancelOrder(consumer);
                                consumerManager.workFlowController.ReturnTable(consumer.currentTable);
                            }

                            break;
                    }

                    agent.SetDestination(consumerManager.spawnPoint.position);
                    break;
                case ConsumerState.TalkingAbout:
                    consumer.currentTable.HideIcon();
                    ConsumerScriptActive(
                        string.Format(Strings.badTextFormat, UnityEngine.Random.Range(0, 2), consumerData.GuestId)
                        , null, () => { satisfactionIcon.SetIcon(currentSatisfaction); });
                    break;
                case ConsumerState.None:
                    model.PlayAnimation(PlayerState.MOVE, 0);
                    break;
            }

            if (currentStatus == prevStatus)
                currentStatus = value;
        }
    }

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        consumer = GetComponent<Consumer>();
    }

    private void Update()
    {
        if (model != null)
        {
            if (prevXPos > transform.position.x)
                model.transform.localScale = new Vector3(1, 1, 1);
            else
                model.transform.localScale = new Vector3(-1, 1, 1);
        }
        prevXPos = transform.position.x;

        switch (currentStatus)
        {
            case ConsumerState.Waiting:
                UpdateWaiting();
                break;
            case ConsumerState.BeforeOrder:
                UpdateBeforeOrder();
                break;
            case ConsumerState.AfterOrder:
                UpdateAfterOrder();
                break;
            case ConsumerState.Eatting:
                UpdateEatting();
                break;
            case ConsumerState.WaitForPay:
                UpdateWaitForPay();
                break;
            case ConsumerState.Paying:
                UpdatePaying();
                break;
            case ConsumerState.Exit:
                UpdateExit();
                break;
            case ConsumerState.WaitingPayLine:
                UpdateWaitingPayLine();
                break;
        }
    }

    private void OnEnable()
    {
        currentSatisfaction = Satisfaction.High;
        consumerData.Init();
        isPaying = false;
        isOnSeat = false;
        isTip = false;
        alreadyTip = false;
    }

    public async UniTask SetModel()
    {
        var handle = Addressables.InstantiateAsync(consumerData.GuestPrefab, modelParent.transform);
        var go = await handle;
        Model = handle.Result.GetComponent<SPUM_Prefabs>();
        Model.OverrideControllerInit();
    }

    public void ConsumerScriptActive(string script, Action startAction = null, Action endAction = null)
    {
        var text = Instantiate(textBubblePrefab, transform).GetComponent<TextBubble>();
        text.Init(LZString.GetUIString(script), startAction, endAction);
    }


    public void OnStartPayment()
    {
        CurrentStatus = ConsumerState.Paying;
    }

    public bool IsTip()
    {
        alreadyTip = true;
        if (consumerData.GuestType != GuestType.BadGuest)
        {
            if (consumer.needFood.FoodID == consumerData.LoveFoodId)
            {
                if (CurrentSatisfaction == Satisfaction.High)
                {
                    if (UnityEngine.Random.Range(0, 4) == 0)
                    {
                        ConsumerScriptActive(string.Format(Strings.paidverygoodTextFormat,
                            UnityEngine.Random.Range(0, 2), consumerData.GuestId));
                        return true;
                    }
                }
            }
        }

        return false;
    }

    public void OnEndPayment()
    {
        int Cost = consumer.needFood.SellingCost;
        int rankPoint = consumer.needFood.GetRankPoints;
        ServiceLocator.Instance.GetSceneService<GameManager>().MissionManager
            .OnEventInvoked(MissionMainCategory.GainMoney, consumer.needFood.SellingCost);
        ServiceLocator.Instance.GetSceneService<GameManager>().MissionManager
            .OnEventInvoked(MissionMainCategory.SellingFood, 1, (int)consumer.needFood.FoodID);
        ServiceLocator.Instance.GetSceneService<GameManager>().MissionManager
            .OnEventInvoked(MissionMainCategory.SellingFood, 1);
        if (isTip)
        {
            Cost += Mathf.CeilToInt(consumer.needFood.SellingCost * (consumerData.SellTipPercent / 100f));
            ServiceLocator.Instance.GetSceneService<GameManager>().MissionManager
                .OnEventInvoked(MissionMainCategory.GetTip, 1);
            //TODO : Get Tip
        }

        UserDataManager.Instance.AdjustMoneyWithSave(Cost).Forget();
        UserDataManager.Instance.OnSellingFood(consumer.needFood.FoodID).Forget();
        Vector3 paymentTextPosition = new Vector3(transform.position.x, transform.position.y + 1f, 0);
        var paymentText = Instantiate(paymentTextPrefab, paymentTextPosition, Quaternion.identity)
            .GetComponent<PaymentText>();
        paymentText.Init(consumer, isTip);


        CurrentStatus = ConsumerState.Exit;
    }

    public void SetCashierCounter(CashierCounter cashierCounter)
    {
        this.cashierCounter = cashierCounter;
    }

    private IEnumerator EattingCoroutine()
    {
        var eattingTimer = 0f;
        StartCoroutine(model.PlayLoopAnim(PlayerState.OTHER, 1));
        if (CurrentSatisfaction != Satisfaction.Low)
        {
            consumer.currentTable.HideIcon();
            satisfactionIcon.SetIcon(currentSatisfaction);
        }

        while (eattingTimer < consumerData.MaxEattingLimit)
        {
            eattingTimer += Time.deltaTime;

            yield return null;
        }

        consumerManager.workFlowController.CreateDirtyOnTable(consumer.currentTable);

        if (consumer.pairData != null)
        {
            consumer.isEndMeal = true;
            if (consumer.pairData.owner == consumer)
            {
                if (!consumer.pairData.partner.isEndMeal)
                {
                    CurrentStatus = ConsumerState.WaitingPairMealEnd;
                }
                else
                {
                    if (consumerManager.IsPayWaitingLineVacated)
                    {
                        CurrentStatus = ConsumerState.WaitForPay;
                        consumer.pairData.partner.FSM.CurrentStatus = ConsumerState.WaitForPay;
                    }
                    else
                    {
                        CurrentStatus = ConsumerState.WaitingPayLine;
                        consumer.pairData.partner.FSM.CurrentStatus = ConsumerState.WaitingPayLine;
                    }
                }
            }
            else if (consumer.pairData.partner == consumer)
            {
                if (!consumer.pairData.owner.isEndMeal)
                {
                    CurrentStatus = ConsumerState.WaitingPairMealEnd;
                }
                else
                {
                    if (consumerManager.IsPayWaitingLineVacated)
                    {
                        CurrentStatus = ConsumerState.WaitForPay;
                        consumer.pairData.owner.FSM.CurrentStatus = ConsumerState.WaitForPay;
                    }
                    else
                    {
                        CurrentStatus = ConsumerState.WaitingPayLine;
                        consumer.pairData.owner.FSM.CurrentStatus = ConsumerState.WaitingPayLine;
                    }
                }
            }
        }
        else
        {
            if (consumerManager.IsPayWaitingLineVacated)
                CurrentStatus = ConsumerState.WaitForPay;
            else
                CurrentStatus = ConsumerState.WaitingPayLine;
        }

        StopAllCoroutines();
    }

    public void OnOrderComplete()
    {
        CurrentStatus = ConsumerState.AfterOrder;
    }


    public void OnGetFood()
    {
        if (consumer.pairData == null)
            CurrentStatus = ConsumerState.Eatting;
        else
        {
            consumer.isFoodReady = true;
            if (consumer.pairData.owner == consumer)
            {
                if (!consumer.pairData.partner.isFoodReady)
                {
                    CurrentStatus = ConsumerState.WaitingPairMealEnd;
                }
                else
                {
                    CurrentStatus = ConsumerState.Eatting;
                    consumer.pairData.partner.FSM.CurrentStatus = ConsumerState.Eatting;
                }
            }
            else if (consumer.pairData.partner == consumer)
            {
                if (!consumer.pairData.owner.isFoodReady)
                {
                    CurrentStatus = ConsumerState.WaitingPairMealEnd;
                }
                else
                {
                    CurrentStatus = ConsumerState.Eatting;
                    consumer.pairData.owner.FSM.CurrentStatus = ConsumerState.Eatting;
                }
            }
        }
    }

    private void UpdateWaiting()
    {
        if (agent.IsArrive(agent.destination))
        {
            model.PlayAnimation(PlayerState.IDLE, 0);
            consumer.pairData?.partner.FSM.Model.PlayAnimation(PlayerState.IDLE, 0);
        }
    }

    private void UpdateBeforeOrder()
    {
        if (consumer.pairData != null)
        {
            model.PlayAnimation(PlayerState.IDLE, 2);
            consumer.pairData.partner.FSM.Model.PlayAnimation(PlayerState.IDLE, 2);
            if (agent.IsArrive(targetPivot))
            {
                consumer.pairData.partner.FSM.transform.localScale = new Vector3(-1, 1, 1);
                if (!isOnSeat)
                {
                    isOnSeat = true;
                    OnSeatEvent?.Invoke(consumer);
                    consumer.pairData.partner.FSM.Model.PlayAnimation(PlayerState.OTHER, 0);
                    model.PlayAnimation(PlayerState.OTHER, 0);
                }
            }

            return;
        }

        if (agent.IsArrive(targetPivot))
        {
            model.PlayAnimation(PlayerState.IDLE, 2);
            if (!isOnSeat)
            {
                isOnSeat = true;
                OnSeatEvent?.Invoke(consumer);
                model.PlayAnimation(PlayerState.OTHER, 0);
            }
        }
    }

    private void UpdateAfterOrder()
    {
        if (CurrentSatisfaction == Satisfaction.Low)
        {
            return;
        }

        switch (consumerData.GuestType)
        {
            case GuestType.Guest:
            case GuestType.Regular1:
            case GuestType.Regular2:
            case GuestType.Regular3:
            case GuestType.BadGuest:
                var deltaTime = buffManager.GetBuff(BuffType.TimerSpeed)?.isOnBuff ?? false
                    ? Time.deltaTime * buffManager.GetBuff(BuffType.TimerSpeed).BuffEffect
                    : Time.deltaTime;

                consumerData.orderWaitTimer -= deltaTime;

                if (consumerData.GuestType == GuestType.BadGuest)
                    consumerData.orderWaitTimer -= deltaTime;
                break;
        }

        switch (consumerData.orderWaitTimer)
        {
            case var t when t <= satisfactionChangeLimit[0] && t > satisfactionChangeLimit[1]:
                if (CurrentSatisfaction != Satisfaction.High)
                {
                    CurrentSatisfaction = Satisfaction.High;
                }

                break;
            case var t when t < satisfactionChangeLimit[1] && t > satisfactionChangeLimit[2] &&
                            CurrentSatisfaction != Satisfaction.Middle:
                CurrentSatisfaction = Satisfaction.Middle;
                break;
            case var t when t < satisfactionChangeLimit[2]:
                CurrentStatus = ConsumerState.TalkingAbout;
                CurrentSatisfaction = Satisfaction.Low;
                if (consumer.pairData == null)
                    CurrentStatus = ConsumerState.Exit;
                if (consumer.pairData?.owner == consumer)
                {
                    consumer.pairData.partner.FSM.CurrentStatus = ConsumerState.TalkingAbout;
                    consumer.pairData.partner.FSM.CurrentSatisfaction = Satisfaction.Low;
                }
                else if (consumer.pairData?.partner == consumer)
                {
                    consumer.pairData.owner.FSM.CurrentStatus = ConsumerState.TalkingAbout;
                    consumer.pairData.owner.FSM.CurrentSatisfaction = Satisfaction.Low;
                }

                break;
        }

        FillSatisfactionGage();
    }

    private void FillSatisfactionGage()
    {
        float normalTimer = Mathf.Lerp(0.25f, 0.75f,
            Mathf.InverseLerp(0, consumerData.MaxOrderWaitLimit, consumerData.orderWaitTimer));
        var iconBubble = consumer.currentTable.IconBubble;
        iconBubble.FillingSatisfation(normalTimer);
        switch (consumerData.orderWaitTimer)
        {
            case var t when t <= satisfactionColorChangeFlagTimes[0] && t > satisfactionColorChangeFlagTimes[1]:
                var highLerp = Mathf.InverseLerp(satisfactionColorChangeFlagTimes[0],
                    satisfactionColorChangeFlagTimes[1], consumerData.orderWaitTimer);
                iconBubble.SetColorSatisfaction(Colors.satisfactionColors[0], Colors.satisfactionColors[1], highLerp);
                break;
            case var t when t < satisfactionColorChangeFlagTimes[1] && t > satisfactionColorChangeFlagTimes[2]:
                var middleLerp = Mathf.InverseLerp(satisfactionColorChangeFlagTimes[1],
                    satisfactionColorChangeFlagTimes[2], consumerData.orderWaitTimer);
                iconBubble.SetColorSatisfaction(Colors.satisfactionColors[1], Colors.satisfactionColors[2], middleLerp);
                break;
        }
    }

    private void UpdateEatting()
    {
        //�Ļ����� ����.
    }


    private void UpdateBeforePay()
    {
    }

    private void UpdateWaitForPay()
    {
        if (agent.IsArrive(agent.destination))
        {
            model.PlayAnimation(PlayerState.IDLE, 0);
        }
    }

    private void UpdatePaying()
    {
        var destination = agent.destination;
        if (agent.IsArrive(destination) && !isPaying)
        {
            isPaying = true;
            model.PlayAnimation(PlayerState.IDLE, 1);
            consumerManager.OnPayStart(consumerData);
        }
    }

    private void UpdateExit()
    {
        //��� �� �����ϴ� ����.
        //���� ������ƮǮ�� ��ȯ��.
        if (agent.IsArrive(consumerManager.spawnPoint)) consumerManager.consumerPool.Release(gameObject);
    }

    private void UpdateWaitingPayLine()
    {
        if (consumerManager.IsPayWaitingLineVacated)
        {
            CurrentStatus = ConsumerState.WaitForPay;
            if (consumer.pairData != null && consumer.pairData.owner == consumer)
            {
                consumer.pairData.partner.FSM.CurrentStatus = ConsumerState.WaitForPay;
            }
        }
    }
}