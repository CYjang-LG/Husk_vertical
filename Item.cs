using UnityEngine;

public class Item : MonoBehaviour
{
    [Header("Item Settings")]
    [Tooltip("아이템 타입 (Coin, Power, Boom)")]
    [SerializeField] private string type = "Coin";
    
    [Tooltip("낙하 속도")]
    [SerializeField] private float fallSpeed = 1.5f;

    private Rigidbody2D rigid;
    private static readonly Vector2 FALL_DIRECTION = Vector2.down;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        
        if (rigid == null)
        {
            Debug.LogError("Item: Rigidbody2D component missing!");
        }
    }

    private void OnEnable()
    {
        // 활성화될 때 낙하 속도 설정
        if (rigid != null)
        {
            rigid.velocity = FALL_DIRECTION * fallSpeed;
        }
    }

    /// <summary>
    /// 아이템 타입 반환
    /// </summary>
    public string GetItemType() => type;

    /// <summary>
    /// 아이템 타입 설정
    /// </summary>
    public void SetItemType(string newType)
    {
        type = newType;
    }

    /// <summary>
    /// 낙하 속도 설정
    /// </summary>
    public void SetFallSpeed(float speed)
    {
        fallSpeed = speed;
        if (gameObject.activeSelf && rigid != null)
        {
            rigid.velocity = FALL_DIRECTION * fallSpeed;
        }
    }
}
