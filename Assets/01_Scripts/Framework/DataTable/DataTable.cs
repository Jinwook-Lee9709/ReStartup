using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CsvHelper;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public abstract class DataTable
{
    public abstract void Load();

    public static async Task<List<T>> LoadCsv<T>(string assetId)
    {
        var handle =  Addressables.LoadAssetAsync<TextAsset>(assetId);
        await handle.Task;

        if (handle.Status != AsyncOperationStatus.Succeeded)
        {
            Debug.LogError("Failed to load csv");
        }
        var csv = handle.Result.text;
        using(var reader = new StringReader(csv))
        using (var csvReader = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            return csvReader.GetRecords<T>().ToList();
        }
    }
}
