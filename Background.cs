using UnityEngine;

/// <summary>
/// 배경 무한 스크롤링 관리
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
    private float spriteHeight;

    private void Awake()
    {
        viewHeight = Camera.main.orthographicSize * 2;

        // 스프라이트 높이 자동 계산
        if (sprites != null && sprites.Length > 0 && sprites[0] != null)
        {
            SpriteRenderer sr = sprites[0].GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                spriteHeight = sr.bounds.size.y;
                Debug.Log($"Background: Sprite height = {spriteHeight}");
            }
            else
            {
                spriteHeight = 10f; // 기본값
                Debug.LogWarning("Background: No SpriteRenderer found, using default height 10");
            }
        }

        ValidateSettings();
    }

    private void Update()
    {
        Move();
        Scrolling();
    }

    /// <summary>
    /// 배경을 아래로 이동
    /// </summary>
    private void Move()
    {
        transform.position += Vector3.down * (speed * Time.deltaTime);
    }

    /// <summary>
    /// 화면 밖으로 나간 스프라이트를 위로 재배치
    /// </summary>
    private void Scrolling()
    {
        // 화면 아래로 완전히 벗어났는지 체크
        if (sprites[endIndex].position.y < -viewHeight)
        {
            // ✅ 수정: spriteHeight를 사용하여 정확한 위치에 재배치
            Vector3 backSpritePos = sprites[startIndex].localPosition;
            sprites[endIndex].localPosition = backSpritePos + Vector3.up * spriteHeight;

            // 인덱스 순환
            int tempStartIndex = startIndex;
            startIndex = endIndex;
            endIndex = (tempStartIndex - 1 + sprites.Length) % sprites.Length;

            Debug.Log($"Background scrolled: new start={startIndex}, end={endIndex}");
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

        // 스프라이트가 null인지 체크
        for (int i = 0; i < sprites.Length; i++)
        {
            if (sprites[i] == null)
            {
                Debug.LogError($"Background: Sprite at index {i} is null!");
                enabled = false;
            }
        }
    }
}