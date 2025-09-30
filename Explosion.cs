// ============================================
// Explosion.cs (최적화)
// 폭발 이펙트를 관리하는 스크립트
// ============================================
using UnityEngine;

public class Explosion : MonoBehaviour
{
    private Animator anim;
    private const float DISABLE_DELAY = 2f;
    private const string EXPLOSION_TRIGGER = "OnExplosion";

    // 스케일 상수화 (메모리 절약)
    private static readonly Vector3 SCALE_SMALL = Vector3.one * 0.7f;
    private static readonly Vector3 SCALE_MEDIUM = Vector3.one * 1f;
    private static readonly Vector3 SCALE_LARGE = Vector3.one * 2f;
    private static readonly Vector3 SCALE_BOSS = Vector3.one * 3f;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        
        if (anim == null)
        {
            Debug.LogError("Explosion: Animator component missing!");
        }
    }

    private void OnEnable()
    {
        // 2초 후 자동으로 비활성화
        Invoke(nameof(Disable), DISABLE_DELAY);
    }

    private void OnDisable()
    {
        // Invoke 취소 (메모리 누수 방지)
        CancelInvoke(nameof(Disable));
    }

    private void Disable()
    {
        gameObject.SetActive(false);
    }

    /// <summary>
    /// 폭발 시작 (대상 크기에 따라 스케일 조정)
    /// </summary>
    /// <param name="target">S=Small, M/P=Medium, L=Large, B=Boss</param>
    public void StartExplosion(string target)
    {
        anim.SetTrigger(EXPLOSION_TRIGGER);

        // switch expression 사용 (C# 8.0+)
        transform.localScale = target switch
        {
            "S" => SCALE_SMALL,      // 소형 적
            "M" or "P" => SCALE_MEDIUM, // 중형 적, 플레이어
            "L" => SCALE_LARGE,      // 대형 적
            "B" => SCALE_BOSS,       // 보스
            _ => SCALE_MEDIUM        // 기본값
        };
    }
}
