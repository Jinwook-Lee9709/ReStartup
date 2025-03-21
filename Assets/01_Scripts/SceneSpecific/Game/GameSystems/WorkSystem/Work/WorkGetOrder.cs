using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class WorkGetOrder : WorkBase
{
    
    
    public WorkGetOrder(WorkManager workManager) : base(workManager, WorkType.Hall)
    {
        
    }
    
    //의자에 앉아 -> WorkManager, GameManager -> 손님 -> WorkGetOrder() -> Init

    private void Init()
    {
        //TODO:
        //주문을 받기위해서 할일
        //1. 손님위에 아이콘 팝업
        //2. 손님 컨텍스트 받기
        //3. 주문 받는 시간 고려 (아마도?)
    }

    public override void OnWorkStopped()
    {
        workManager.AddCanceledWork(workType, this);
    }

    public override void OnWorkFinished()
    {
    }
}
