using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 오브젝트 풀링을 관리하는 매니저
/// 모든 게임 오브젝트(적, 총알, 아이템 등)를 사전에 생성하고 재사용
/// </summary>
public class ObjectManager : MonoBehaviour
{
    [Header("Enemy Prefabs")]
    [SerializeField] private GameObject enemyBPrefab;
    [SerializeField] private GameObject enemyLPrefab;
    [SerializeField] private GameObject enemyMPrefab;
    [SerializeField] private GameObject enemySPrefab;

    [Header("Item Prefabs")]
    [SerializeField] private GameObject itemCoinPrefab;
    [SerializeField] private GameObject itemPowerPrefab;
    [SerializeField] private GameObject itemBoomPrefab;

    [Header("Bullet Prefabs")]
    [SerializeField] private GameObject bulletPlayerAPrefab;
    [SerializeField] private GameObject bulletPlayerBPrefab;
    [SerializeField] private GameObject bulletEnemyAPrefab;
    [SerializeField] private GameObject bulletEnemyBPrefab;
    [SerializeField] private GameObject bulletFollowerPrefab;
    [SerializeField] private GameObject bulletBossAPrefab;
    [SerializeField] private GameObject bulletBossBPrefab;

    [Header("Effect Prefabs")]
    [SerializeField] private GameObject explosionPrefab;

    // Dictionary로 최적화 (switch문보다 O(1) 검색)
    private Dictionary<string, GameObject[]> poolDictionary;

    private void Awake()
    {
        InitializePools();
    }

    /// <summary>
    /// 모든 오브젝트 풀 초기화
    /// </summary>
    private void InitializePools()
    {
        poolDictionary = new Dictionary<string, GameObject[]>
        {
            // 적 풀
            { "EnemyB", CreatePool(enemyBPrefab, 1, "Boss") },
            { "EnemyL", CreatePool(enemyLPrefab, 10, "Large") },
            { "EnemyM", CreatePool(enemyMPrefab, 10, "Medium") },
            { "EnemyS", CreatePool(enemySPrefab, 20, "Small") },
            
            // 아이템 풀
            { "ItemCoin", CreatePool(itemCoinPrefab, 20, "Coin") },
            { "ItemPower", CreatePool(itemPowerPrefab, 10, "Power") },
            { "ItemBoom", CreatePool(itemBoomPrefab, 10, "Boom") },
            
            // 총알 풀
            { "BulletPlayerA", CreatePool(bulletPlayerAPrefab, 100, "PlayerBulletA") },
            { "BulletPlayerB", CreatePool(bulletPlayerBPrefab, 100, "PlayerBulletB") },
            { "BulletEnemyA", CreatePool(bulletEnemyAPrefab, 100, "EnemyBulletA") },
            { "BulletEnemyB", CreatePool(bulletEnemyBPrefab, 100, "EnemyBulletB") },
            { "BulletFollower", CreatePool(bulletFollowerPrefab, 100, "FollowerBullet") },
            { "BulletBossA", CreatePool(bulletBossAPrefab, 50, "BossBulletA") },
            { "BulletBossB", CreatePool(bulletBossBPrefab, 1000, "BossBulletB") },
            
            // 이펙트 풀
            { "Explosion", CreatePool(explosionPrefab, 20, "Explosion") }
        };

        Debug.Log($"ObjectManager: {poolDictionary.Count} pools initialized successfully");
    }

    /// <summary>
    /// 오브젝트 풀 생성
    /// </summary>
    /// <param name="prefab">프리팹</param>
    /// <param name="size">풀 크기</param>
    /// <param name="poolName">풀 이름</param>
    /// <returns>생성된 오브젝트 배열</returns>
    private GameObject[] CreatePool(GameObject prefab, int size, string poolName)
    {
        if (prefab == null)
        {
            Debug.LogError($"ObjectManager: Prefab for {poolName} is null!");
            return new GameObject[0];
        }

        GameObject[] pool = new GameObject[size];
        
        // 풀 부모 오브젝트 생성 (계층 구조 정리)
        Transform poolParent = new GameObject($"{poolName}_Pool").transform;
        poolParent.SetParent(transform);

        for (int i = 0; i < size; i++)
        {
            pool[i] = Instantiate(prefab, poolParent);
            pool[i].name = $"{poolName}_{i:D3}"; // 이름 포맷팅
            pool[i].SetActive(false);
        }

        return pool;
    }

    /// <summary>
    /// 풀에서 사용 가능한 오브젝트 가져오기
    /// </summary>
    /// <param name="type">오브젝트 타입</param>
    /// <returns>활성화된 게임 오브젝트 (없으면 null)</returns>
    public GameObject MakeObj(string type)
    {
        if (!poolDictionary.TryGetValue(type, out GameObject[] pool))
        {
            Debug.LogWarning($"ObjectManager: Pool type '{type}' not found!");
            return null;
        }

        // 비활성화된 오브젝트 찾기
        for (int i = 0; i < pool.Length; i++)
        {
            if (!pool[i].activeSelf)
            {
                pool[i].SetActive(true);
                return pool[i];
            }
        }

        Debug.LogWarning($"ObjectManager: Pool '{type}' is exhausted! Consider increasing pool size.");
        return null;
    }

    /// <summary>
    /// 특정 타입의 전체 풀 가져오기
    /// </summary>
    /// <param name="type">오브젝트 타입</param>
    /// <returns>오브젝트 배열</returns>
    public GameObject[] GetPool(string type)
    {
        if (poolDictionary.TryGetValue(type, out GameObject[] pool))
        {
            return pool;
        }

        Debug.LogWarning($"ObjectManager: Pool type '{type}' not found!");
        return null;
    }

    /// <summary>
    /// 모든 풀 초기화 (게임 재시작 시 사용)
    /// </summary>
    public void ResetAllPools()
    {
        foreach (var pool in poolDictionary.Values)
        {
            foreach (GameObject obj in pool)
            {
                if (obj.activeSelf)
                {
                    obj.SetActive(false);
                }
            }
        }
        Debug.Log("ObjectManager: All pools reset");
    }

    /// <summary>
    /// 특정 풀 초기화
    /// </summary>
    public void ResetPool(string type)
    {
        if (poolDictionary.TryGetValue(type, out GameObject[] pool))
        {
            foreach (GameObject obj in pool)
            {
                if (obj.activeSelf)
                {
                    obj.SetActive(false);
                }
            }
            Debug.Log($"ObjectManager: Pool '{type}' reset");
        }
    }

    /// <summary>
    /// 활성화된 오브젝트 개수 확인 (디버깅용)
    /// </summary>
    public int GetActiveCount(string type)
    {
        if (!poolDictionary.TryGetValue(type, out GameObject[] pool))
        {
            return 0;
        }

        int count = 0;
        foreach (GameObject obj in pool)
        {
            if (obj.activeSelf)
            {
                count++;
            }
        }
        return count;
    }
}
