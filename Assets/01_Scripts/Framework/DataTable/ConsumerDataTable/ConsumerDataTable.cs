using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ConsumerDataTable : DataTable
{
    public Dictionary<int, ConsumerData> Data = new();

    public ConsumerData GetRandomConsumerForType(GuestType guestType)
    {
        var result = Data.Values.Where(consumerData => consumerData.GuestType == guestType && consumerData.Theme == ServiceLocator.Instance.GetSceneService<GameManager>().CurrentTheme).ToList();
        return result[Random.Range(0, result.Count)];
    }

    public ConsumerData GetConsumerData(List<int> percents)
    {
        int integration = 0;
        int value = Random.Range(0, 100);
        for (int i = 0; i < percents.Count; i++)
        {
            integration += percents[i];
            if(integration >= value)
            {
                return GetRandomConsumerForType((GuestType)i);
            }
        }
        return null;
    }

    public ConsumerData GetConsumerData(int id)
    {
        Data.TryGetValue(id, out var data);
        return data;
    }

    public override void Load()
    {
        var result = LoadCsv<ConsumerData>("guesttable");
        foreach (var row in result)
        {
            if (Data.ContainsKey(row.GuestId)) continue;
            Data.Add(row.GuestId, row);
        }
    }
}
