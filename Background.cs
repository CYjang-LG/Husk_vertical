using UnityEngine;

/// <summary>
/// 배경 스크롤링을 관리하는 스크립트
/// 무한 스크롤을 위해 3개의 배경 스프라이트를 재사용합니다
/// </summary>
public class Background : MonoBehaviour
{
    [Header("Scrolling Settings")]
    [Tooltip("배경 스크롤 속도")]
    [SerializeField] private float speed = 1f;
    
    [Header("Sprite References")]
    [Tooltip("배경 스프라이트 배열 (3개 권장)")]
    [SerializeField] private Transform[] sprites;
    
    [Tooltip("첫 번째 스프라이트 인덱스")]
    [SerializeField] private int startIndex = 0;
    
    [Tooltip("마지막 스프라이트 인덱스")]
    [SerializeField] private int endIndex = 2;

    private float viewHeight;
    //private const float SPRITE_HEIGHT = 10f;
    private float spriteHeight;

    private void Awake()
    {
        spriteHeight = sprites[0].GetComponent<SpriteRenderer>().bounds.size.y;
        viewHeight = Camera.main.orthographicSize * 2;
        ValidateSettings();
    }

    private void Update()
    {
        Move();
        Scrolling();
    }

    /// <summary>
    /// 배경을 아래로 이동시킵니다
    /// </summary>
    private void Move()
    {
        // 최적화: Vector3.down 캐싱 및 직접 계산
        transform.position += Vector3.down * (speed * Time.deltaTime);
    }

    /// <summary>
    /// 화면 밖으로 나간 스프라이트를 위로 재배치합니다
    /// </summary>
    private void Scrolling()
    {
        // 화면 밖으로 나간 스프라이트 체크
        if (sprites[endIndex].position.y < -viewHeight)
        {
            // 스프라이트 재사용 - 맨 위로 이동
            Vector3 backSpritePos = sprites[startIndex].localPosition;
            sprites[endIndex].localPosition = backSpritePos + Vector3.up;

            // 인덱스 업데이트 (최적화된 순환 로직)
            int tempStartIndex = startIndex;
            startIndex = endIndex;
            endIndex = (tempStartIndex - 1 + sprites.Length) % sprites.Length;
        }
    }

    /// <summary>
    /// 설정값 유효성 검사
    /// </summary>
    private void ValidateSettings()
    {
        if (sprites == null || sprites.Length == 0)
        {
            Debug.LogError("Background: Sprites array is empty!");
            enabled = false;
            return;
        }

        if (startIndex < 0 || startIndex >= sprites.Length)
        {
            Debug.LogError($"Background: Invalid startIndex {startIndex}");
            enabled = false;
        }

        if (endIndex < 0 || endIndex >= sprites.Length)
        {
            Debug.LogError($"Background: Invalid endIndex {endIndex}");
            enabled = false;
        }
    }
}
