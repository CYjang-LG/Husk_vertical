using UnityEngine;

/// <summary>
/// 총알 동작을 관리하는 스크립트
/// 플레이어/적 총알 모두 사용 가능
/// </summary>
public class Bullet : MonoBehaviour
{
    [Header("Bullet Settings")]
    [Tooltip("총알 데미지")]
    [SerializeField] private int dmg = 1;
    
    [Tooltip("회전 여부")]
    [SerializeField] private bool isRotate = false;
    
    [Tooltip("회전 속도 (초당 각도)")]
    [SerializeField] private float rotateSpeed = 10f;

    private static readonly Vector3 ROTATION_AXIS = Vector3.forward;
    private const string BORDER_TAG = "BorderBullet";

    private void Update()
    {
        if (isRotate)
        {
            // 최적화: Time.deltaTime 적용하여 프레임 독립적 회전
            transform.Rotate(ROTATION_AXIS * (rotateSpeed * Time.deltaTime));
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 최적화: CompareTag가 string 비교보다 성능 우수
        if (collision.CompareTag(BORDER_TAG))
        {
            ReturnToPool();
        }
    }

    /// <summary>
    /// 오브젝트 풀로 반환 (비활성화)
    /// </summary>
    private void ReturnToPool()
    {
        gameObject.SetActive(false);
    }

    /// <summary>
    /// 총알 데미지 반환
    /// </summary>
    public int GetDamage() => dmg;

    /// <summary>
    /// 총알 데미지 설정
    /// </summary>
    public void SetDamage(int newDamage)
    {
        dmg = newDamage;
    }
}
