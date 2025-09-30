using UnityEngine;

public class Item : MonoBehaviour
{
    public string type;  // "Coin", "Power", "Boom"

    private Rigidbody2D rigid;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        // 아이템이 활성화되면 아래로 떨어짐
        rigid.linearVelocity = Vector2.down * 1.5f;
    }
}