using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
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
    }

    public enum Satisfaction
    {
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
    private List<Color> Colors = new()
    {
        new Color(Mathf.InverseLerp(0, 255, 202), Mathf.InverseLerp(0, 255, 235), Mathf.InverseLerp(0, 255, 255)),
        new Color(Mathf.InverseLerp(0, 255, 255), Mathf.InverseLerp(0, 255, 249), Mathf.InverseLerp(0, 255, 159)),
        new Color(Mathf.InverseLerp(0, 255, 255), Mathf.InverseLerp(0, 255, 128), Mathf.InverseLerp(0, 255, 125))
    };

    [SerializeField] public ConsumerState currentStatus = ConsumerState.Waiting;
    [SerializeField] private Satisfaction currentSatisfaction = Satisfaction.High;
    [SerializeField] private PaymentText paymentTextPrefab;
    [SerializeField] public SatisfactionIcon satisfactionIcon;

    private NavMeshAgent agent;
    private CashierCounter cashierCounter;
    private Consumer consumer;
    public ConsumerData consumerData = new();
    private SPUM_Prefabs model;
    public SPUM_Prefabs Model => model;
    private bool isOnSeat;
    private bool isPaying;
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
                    break;
                case Satisfaction.Middle:
                    // TODO : Serving Delay Script Play
                    break;
                case Satisfaction.Low:
                    consumer.currentTable.HideIcon();
                    satisfactionIcon.SetIcon(currentSatisfaction);
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
                        targetPivot = consumer.pairData.partner.currentTable.GetInteractablePoints(permission)[0].transform.position;
                        consumer.pairData.partner.GetComponent<NavMeshAgent>().SetDestination(targetPivot);
                    }
                    targetPivot = consumer.currentTable.GetInteractablePoints(permission)[0].transform.position;
                    agent.SetDestination(targetPivot);
                    break;
                case ConsumerState.AfterOrder:
                    consumerManager.OnChangeConsumerState(consumer, ConsumerState.AfterOrder);
                    consumer.currentTable.IconBubble.SetColorSatisfaction(Color.white, Color.blue, 1f);
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
                            break;
                        case Satisfaction.Low:
                            UserDataManager.Instance.AddConsumerCnt(false);
                            if (consumerData.GuestType == GuestType.BadGuest)
                            {
                                consumerManager.workFlowController.CancelOrder(consumer);
                                consumerManager.workFlowController.ReturnTable(consumer.currentTable);
                            }
                            break;
                    }
                    agent.SetDestination(consumerManager.spawnPoint.position);
                    break;
                case ConsumerState.WaitingPayLine:

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
        if (prevXPos > transform.position.x)
            model.transform.localScale = new Vector3(1, 1, 1);
        else
            model.transform.localScale = new Vector3(-1, 1, 1);
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
        model = GetComponentInChildren<SPUM_Prefabs>();
        model.OverrideControllerInit();

        currentSatisfaction = Satisfaction.High;
        consumerData.Init();
        isPaying = false;
        isOnSeat = false;
    }


    public void OnStartPayment()
    {
        CurrentStatus = ConsumerState.Paying;
    }
    private bool IsTip()
    {
        if (consumerData.GuestType != GuestType.BadGuest)
        {
            if (consumer.needFood.FoodID == consumerData.LoveFoodId)
            {
                if (CurrentSatisfaction == Satisfaction.High)
                {
                    if (UnityEngine.Random.Range(0, 4) == 0)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }
    public void OnEndPayment()
    {
        bool isTip = IsTip();
        //bool isTip = true;
        int Cost = consumer.needFood.SellingCost;
        int rankPoint = consumer.needFood.GetRankPoints;
        if (isTip)
        {
            Cost += Mathf.CeilToInt(consumer.needFood.SellingCost * (consumerData.SellTipPercent / 100f));
        }
        UserDataManager.Instance.AdjustMoneyWithSave(Cost).Forget();
        Vector3 paymentTextPosition = new Vector3(transform.position.x, transform.position.y + 1f, 0);
        var paymentText = Instantiate(paymentTextPrefab, paymentTextPosition, Quaternion.identity).GetComponent<PaymentText>();
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

        var deltaTime = buffManager.GetBuff(BuffType.TimerSpeed)?.isOnBuff ?? false ? Time.deltaTime * buffManager.GetBuff(BuffType.TimerSpeed).BuffEffect : Time.deltaTime;

        consumerData.orderWaitTimer -= deltaTime;

        if (consumerData.GuestType == GuestType.BadGuest)
            consumerData.orderWaitTimer -= deltaTime;

        switch (consumerData.orderWaitTimer)
        {
            case var t when t < satisfactionChangeLimit[0] && t > satisfactionChangeLimit[1]:
                CurrentSatisfaction = Satisfaction.High;
                break;
            case var t when t < satisfactionChangeLimit[1] && t > satisfactionChangeLimit[2] &&
                            CurrentSatisfaction != Satisfaction.Middle:
                CurrentSatisfaction = Satisfaction.Middle;
                break;
            case var t when t < satisfactionChangeLimit[2]:
                CurrentSatisfaction = Satisfaction.Low;
                if (consumerData.GuestType == GuestType.BadGuest)
                {
                    CurrentStatus = ConsumerState.Exit;
                    break;
                }
                if(consumer.pairData?.owner == consumer)
                {
                    consumer.pairData.partner.FSM.CurrentSatisfaction = Satisfaction.Low;
                }
                else if (consumer.pairData?.partner == consumer)
                {
                    consumer.pairData.owner.FSM.CurrentSatisfaction = Satisfaction.Low;
                }
                break;
        }
        FillSatisfactionGage();
    }

    private void FillSatisfactionGage()
    {
        float normalTimer = Mathf.Lerp(0.25f, 0.75f, Mathf.InverseLerp(0, consumerData.MaxOrderWaitLimit, consumerData.orderWaitTimer));
        var iconBubble = consumer.currentTable.IconBubble;
        iconBubble.FillingSatisfation(normalTimer);
        switch (consumerData.orderWaitTimer)
        {
            case var t when t < satisfactionColorChangeFlagTimes[0] && t > satisfactionColorChangeFlagTimes[1]:
                var highLerp = Mathf.InverseLerp(satisfactionColorChangeFlagTimes[0], satisfactionColorChangeFlagTimes[1], consumerData.orderWaitTimer);
                iconBubble.SetColorSatisfaction(Colors[0], Colors[1], highLerp);
                break;
            case var t when t < satisfactionColorChangeFlagTimes[1] && t > satisfactionColorChangeFlagTimes[2]:
                var middleLerp = Mathf.InverseLerp(satisfactionColorChangeFlagTimes[1], satisfactionColorChangeFlagTimes[2], consumerData.orderWaitTimer);
                iconBubble.SetColorSatisfaction(Colors[1], Colors[2], middleLerp);
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