using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using CsvHelper;
using CsvHelper.Configuration;
using UnityEngine;

public static class CsvToDictionaryLoader
{
    public static Dictionary<int, List<int>> LoadCsvToDictionary(string assetId)
    {
        var result = new Dictionary<int, List<int>>();

        // Addressables로 CSV 파일 로드
        var handle = UnityEngine.AddressableAssets.Addressables.LoadAssetAsync<TextAsset>(assetId);
        handle.WaitForCompletion();

        if (handle.Status != UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
        {
            Debug.LogError($"Failed to load CSV file: {assetId}");
            return result;
        }

        var csv = handle.Result.text;

        using (var reader = new StringReader(csv))
        using (var csvReader = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            int idx = 0;
            // 1. CSV 읽기
            while (csvReader.Read())
            {
                if(idx == 0)
                {
                    idx++;
                    continue;
                }
                // 첫 번째 열은 키로 사용
                var key = csvReader.GetField<int>(1);

                // 나머지 열은 리스트로 저장
                var values = new List<int>();
                for (var i = 2; csvReader.TryGetField<int>(i, out var value); i++)
                {
                    values.Add(value);
                }

                // Dictionary에 할당
                result[key] = values;
            }
        }

        return result;
    }
}
