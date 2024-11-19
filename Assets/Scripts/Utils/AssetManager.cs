using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public static class AssetManager
{
    /// <summary>
    /// Addressables에서 리소스를 비동기적으로 로드합니다.
    /// </summary>
    /// <typeparam name="T">로드할 리소스의 타입</typeparam>
    /// <param name="key">Addressable 키</param>
    /// <param name="type">리소스 유형 (옵션)</param>
    /// <returns>로드된 리소스</returns>
    public static async Task<T> LoadAsset<T>(string key, eAddressableType type = eAddressableType.Default) where T : Object
    {
        try
        {
            // Addressable 리소스 로드
            AsyncOperationHandle<T> handle = Addressables.LoadAssetAsync<T>(key);
            await handle.Task;

            // 로드 성공
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                return handle.Result;
            }

            // 로드 실패 처리
            Debug.LogError($"Failed to load asset with key: {key}");
            return null;
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Error loading asset: {key}. Exception: {ex}");
            return null;
        }
    }

    /// <summary>
    /// Addressables에서 리소스를 해제합니다.
    /// </summary>
    /// <typeparam name="T">해제할 리소스의 타입</typeparam>
    /// <param name="asset">해제할 리소스</param>
    public static void ReleaseAsset<T>(T asset) where T : Object
    {
        Addressables.Release(asset);
    }
}

/// <summary>
/// Addressable 리소스 유형
/// </summary>
public enum eAddressableType
{
    Default,
    Thumbnail,
    Icon,
    Audio,
    Prefab
}
