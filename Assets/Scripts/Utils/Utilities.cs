using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using System.Text.RegularExpressions;


static public class Utilities
{
    public static GameObject FindGameObject(string path)
    {
        GameObject obj = GameObject.Find(path);
        if (obj == null)
        {
            Debug.LogError($"{path} 오브젝트를 찾을 수 없습니다.");
        }
        return obj;
    }


    static public T FindAndAssign<T>(string path) where T : Component
    {
        GameObject obj = GameObject.Find(path);
        if (obj != null)
        {
            T component = obj.GetComponent<T>();
            if (component == null)
            {
                Debug.LogError($"{path} 오브젝트에서 {typeof(T).Name} 컴포넌트를 찾을 수 없습니다.");
            }
            return component;
        }
        else
        {
            Debug.LogError($"{path} 오브젝트를 찾을 수 없습니다.");
            return null;
        }
    }
    static public void WriteErrorLog(Exception e)
    {
        string path = Application.persistentDataPath + "/OmokLog.txt";
        try
        {
            using (StreamWriter writer = new StreamWriter(path, true))
            {
                writer.WriteLine(DateTime.Now.ToString());
                writer.WriteLine(e.Message);
                writer.WriteLine(e.StackTrace);
                writer.WriteLine();
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Failed to write to log file: " + ex.Message);
        }
    }

    /*---------------------------------------------
     [프리팹 등록]
 ---------------------------------------------*/
    static public async Task RegisterPrefab(string key, Dictionary<string, GameObject> prefabMap)
    {
        // 키를 간소화 (예: Prefab/Enemy/Robot1 -> Robot1)
        string shortKey = ExtractShortKey(key);

        // 이미 등록된 프리팹은 무시
        if (prefabMap.ContainsKey(shortKey))
        {
            Debug.LogWarning($"Prefab '{shortKey}' is already registered.");
            return;
        }

        // 프리팹 로드
        GameObject prefab = await AssetManager.LoadAsset<GameObject>(key);

        if (prefab != null)
        {
            prefabMap[shortKey] = prefab;
            //Debug.Log($"Prefab '{shortKey}' loaded and registered.");
        }
        else
        {
            Debug.LogError($"Failed to load prefab: {key}");
        }
    }


    /*---------------------------------------------
     [프리팹 키 추출]
  ---------------------------------------------*/
    static string ExtractShortKey(string key)
    {
        // 슬래시로 분리하여 마지막 부분만 반환
        return key.Substring(key.LastIndexOf('/') + 1);
    }
}

