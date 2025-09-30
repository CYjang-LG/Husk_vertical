using UnityEngine;

/// <summary>
/// 적과 아이템의 스폰 정보를 저장하는 클래스
/// GameManager에서 스폰 패턴을 정의할 때 사용
/// </summary>
[System.Serializable]
public class Spawn
{
    [Header("Spawn Timing")]
    [Tooltip("스폰 딜레이 (초 단위)")]
    [Min(0)]
    public float delay = 0f;
    
    [Header("Spawn Target")]
    [Tooltip("스폰할 오브젝트 타입\n예: EnemyS, EnemyM, EnemyL, EnemyB, ItemCoin, ItemPower, ItemBoom")]
    public string type = "EnemyS";
    
    [Header("Spawn Position")]
    [Tooltip("스폰 위치 포인트 (0~9, 좌에서 우로)")]
    [Range(0, 9)]
    public int point = 0;

    // 기본 생성자
    public Spawn()
    {
        delay = 0f;
        type = "EnemyS";
        point = 0;
    }

    // 파라미터 생성자
    public Spawn(float delay, string type, int point)
    {
        this.delay = delay;
        this.type = type;
        this.point = Mathf.Clamp(point, 0, 9);
    }

    /// <summary>
    /// 스폰 데이터 유효성 검사
    /// </summary>
    public bool IsValid()
    {
        if (delay < 0)
        {
            Debug.LogWarning($"Spawn: Invalid delay {delay}");
            return false;
        }

        if (string.IsNullOrEmpty(type))
        {
            Debug.LogWarning("Spawn: Type is empty");
            return false;
        }

        if (point < 0 || point > 9)
        {
            Debug.LogWarning($"Spawn: Invalid point {point}");
            return false;
        }

        return true;
    }

    /// <summary>
    /// 디버그 정보 출력
    /// </summary>
    public override string ToString()
    {
        return $"Spawn[{type}] at Point {point} after {delay}s";
    }
}
