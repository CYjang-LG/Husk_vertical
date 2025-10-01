using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Touch Boundaries")]
    public bool isTouchTop;
    public bool isTouchBottom;
    public bool isTouchRight;
    public bool isTouchLeft;

    [Header("Player Stats")]
    public int life;
    public int score;
    public float speed;

    [Header("Power Settings")]
    public int maxPower;
    public int power;

    [Header("Boom Settings")]
    public int maxBoom;
    public int boom;

    [Header("Shooting Settings")]
    public float maxShotDelay;
    public float curShotDelay;

    [Header("References")]
    public GameObject bulletObjA;
    public GameObject bulletObjB;
    public GameObject boomEffect;
    public GameManager gameManager;
    public ObjectManager objectManager;
    public GameObject[] followers;

    [Header("Control States")]
    public bool[] joyControl;   // 조이스틱 방향
    public bool isControl;      // 조이스틱 눌림 상태
    public bool isButtonA;
    public bool isButtonB;

    [Header("Status")]
    public bool isHit;
    public bool isBoomTime;
    public bool isRespawnTime;

    Animator anim;
    SpriteRenderer spriteRenderer;

    void Awake()
    {
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        joyControl = new bool[9]; // 초기화
    }

    void OnEnable()
    {
        Unbeatable();
        Invoke("Unbeatable", 3);
    }

    void Unbeatable()
    {
        isRespawnTime = !isRespawnTime;
        if (isRespawnTime)
        {
            spriteRenderer.color = new Color(1, 1, 1, 0.5f);
            foreach (GameObject follower in followers)
            {
                if (follower.activeSelf)
                    follower.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.5f);
            }
        }
        else
        {
            spriteRenderer.color = new Color(1, 1, 1, 1);
            foreach (GameObject follower in followers)
            {
                if (follower.activeSelf)
                    follower.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
            }
        }
    }

    void Update()
    {
        Move();
        Fire();
        Boom();
        Reload();
    }

    // 조이스틱 패널 입력 (0~8)
    public void JoyPanel(int type)
    {
        for (int i = 0; i < 9; i++)
        {
            joyControl[i] = i == type;
        }
    }

    public void JoyDown()
    {
        isControl = true;
    }

    public void JoyUp()
    {
        isControl = false;
    }

    void Move()
    {
        float h = 0f;
        float v = 0f;

        // ✅ PC 키보드 입력 (항상 활성화)
        float keyboardH = Input.GetAxisRaw("Horizontal");
        float keyboardV = Input.GetAxisRaw("Vertical");

        // 조이스틱 입력
        float joyH = 0f;
        float joyV = 0f;

        if (isControl) // 조이스틱이 눌렸을 때만
        {
            if (joyControl[0]) { joyH = -1; joyV = 1; }
            if (joyControl[1]) { joyH = 0; joyV = 1; }
            if (joyControl[2]) { joyH = 1; joyV = 1; }
            if (joyControl[3]) { joyH = -1; joyV = 0; }
            if (joyControl[4]) { joyH = 0; joyV = 0; }
            if (joyControl[5]) { joyH = 1; joyV = 0; }
            if (joyControl[6]) { joyH = -1; joyV = -1; }
            if (joyControl[7]) { joyH = 0; joyV = -1; }
            if (joyControl[8]) { joyH = 1; joyV = -1; }

            h = joyH;
            v = joyV;
        }
        else
        {
            // 조이스틱을 안 쓸 때는 키보드 입력 사용
            h = keyboardH;
            v = keyboardV;
        }

        // 경계 체크
        if ((isTouchRight && h > 0) || (isTouchLeft && h < 0))
            h = 0;

        if ((isTouchTop && v > 0) || (isTouchBottom && v < 0))
            v = 0;

        // 이동 처리
        Vector3 curPos = transform.position;
        Vector3 nextPos = new Vector3(h, v, 0) * speed * Time.deltaTime;
        transform.position = curPos + nextPos;

        // 애니메이션
        if (Input.GetButtonDown("Horizontal") || Input.GetButtonUp("Horizontal"))
        {
            anim.SetInteger("Input", (int)h);
        }
    }

    public void ButtonADown()
    {
        isButtonA = true;
    }

    public void ButtonAUp()
    {
        isButtonA = false;
    }

    public void ButtonBDown()
    {
        isButtonB = true;
    }

    public void ButtonBUp()
    {
        isButtonB = false;
    }

    void Fire()
    {
        // ✅ PC에서는 스페이스바, 모바일에서는 버튼 A
        if (!isButtonA && !Input.GetButton("Fire1"))
            return;

        if (curShotDelay < maxShotDelay)
            return;

        switch (power)
        {
            case 1:
                GameObject bullet = objectManager.MakeObj("BulletPlayerA");
                if (bullet != null)
                {
                    bullet.transform.position = transform.position;
                    Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
                    rigid.linearVelocity = Vector2.up * 10; // AddForce 대신 velocity 사용
                }
                break;

            case 2:
                GameObject bulletR = objectManager.MakeObj("BulletPlayerA");
                GameObject bulletL = objectManager.MakeObj("BulletPlayerA");

                if (bulletR != null)
                {
                    bulletR.transform.position = transform.position + Vector3.right * 0.1f;
                    bulletR.GetComponent<Rigidbody2D>().linearVelocity = Vector2.up * 10;
                }
                if (bulletL != null)
                {
                    bulletL.transform.position = transform.position + Vector3.left * 0.1f;
                    bulletL.GetComponent<Rigidbody2D>().linearVelocity = Vector2.up * 10;
                }
                break;

            default:
                GameObject bulletRR = objectManager.MakeObj("BulletPlayerA");
                GameObject bulletCC = objectManager.MakeObj("BulletPlayerB");
                GameObject bulletLL = objectManager.MakeObj("BulletPlayerA");

                if (bulletRR != null)
                {
                    bulletRR.transform.position = transform.position + Vector3.right * 0.35f;
                    bulletRR.GetComponent<Rigidbody2D>().linearVelocity = Vector2.up * 10;
                }
                if (bulletCC != null)
                {
                    bulletCC.transform.position = transform.position;
                    bulletCC.GetComponent<Rigidbody2D>().linearVelocity = Vector2.up * 10;
                }
                if (bulletLL != null)
                {
                    bulletLL.transform.position = transform.position + Vector3.left * 0.35f;
                    bulletLL.GetComponent<Rigidbody2D>().linearVelocity = Vector2.up * 10;
                }
                break;
        }

        curShotDelay = 0;
    }

    void Reload()
    {
        curShotDelay += Time.deltaTime;
    }

    void Boom()
    {
        // ✅ PC에서는 Shift 키, 모바일에서는 버튼 B
        if (!isButtonB && !Input.GetButtonDown("Fire2"))
            return;

        if (isBoomTime)
            return;

        if (boom == 0)
            return;

        boom--;
        isBoomTime = true;
        gameManager.UpdateBoomIcon(boom);

        boomEffect.SetActive(true);
        Invoke("OffBoomEffect", 4f);

        // 모든 적 제거
        GameObject[] enemiesL = objectManager.GetPool("EnemyL");
        GameObject[] enemiesM = objectManager.GetPool("EnemyM");
        GameObject[] enemiesS = objectManager.GetPool("EnemyS");

        foreach (GameObject enemy in enemiesL)
        {
            if (enemy.activeSelf)
                enemy.GetComponent<Enemy>().OnHit(1000);
        }
        foreach (GameObject enemy in enemiesM)
        {
            if (enemy.activeSelf)
                enemy.GetComponent<Enemy>().OnHit(1000);
        }
        foreach (GameObject enemy in enemiesS)
        {
            if (enemy.activeSelf)
                enemy.GetComponent<Enemy>().OnHit(1000);
        }

        // 모든 적 총알 제거
        GameObject[] bulletsA = objectManager.GetPool("BulletEnemyA");
        GameObject[] bulletsB = objectManager.GetPool("BulletEnemyB");

        foreach (GameObject bullet in bulletsA)
        {
            if (bullet.activeSelf)
                bullet.SetActive(false);
        }
        foreach (GameObject bullet in bulletsB)
        {
            if (bullet.activeSelf)
                bullet.SetActive(false);
        }
    }

    void OffBoomEffect()
    {
        boomEffect.SetActive(false);
        isBoomTime = false;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Border"))
        {
            switch (collision.gameObject.name)
            {
                case "Top":
                    isTouchTop = true;
                    break;
                case "Bottom":
                    isTouchBottom = true;
                    break;
                case "Right":
                    isTouchRight = true;
                    break;
                case "Left":
                    isTouchLeft = true;
                    break;
            }
        }
        else if (collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("EnemyBullet"))
        {
            if (isRespawnTime || isHit)
                return;

            isHit = true;
            life--;
            gameManager.UpdateLifeIcon(life);
            gameManager.CallExplosion(transform.position, "P");

            if (life == 0)
            {
                gameManager.GameOver();
            }
            else
            {
                gameManager.RespawnPlayer();
            }

            gameObject.SetActive(false);

            if (collision.gameObject.CompareTag("Enemy"))
            {
                Enemy enemy = collision.gameObject.GetComponent<Enemy>();
                if (enemy != null && enemy.enemyName == "B")
                    return;
            }

            collision.gameObject.SetActive(false);
        }
        else if (collision.gameObject.CompareTag("Item"))
        {
            Item item = collision.gameObject.GetComponent<Item>();
            switch (item.type)
            {
                case "Coin":
                    score += 1000;
                    break;
                case "Power":
                    if (power == maxPower)
                        score += 500;
                    else
                    {
                        power++;
                        AddFollower();
                    }
                    break;
                case "Boom":
                    if (boom == maxBoom)
                        score += 500;
                    else
                    {
                        boom++;
                        gameManager.UpdateBoomIcon(boom);
                    }
                    break;
            }
            collision.gameObject.SetActive(false);
        }
    }

    void AddFollower()
    {
        if (power == 4)
            followers[0].SetActive(true);
        else if (power == 5)
            followers[1].SetActive(true);
        else if (power == 6)
            followers[2].SetActive(true);
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Border"))
        {
            switch (collision.gameObject.name)
            {
                case "Top":
                    isTouchTop = false;
                    break;
                case "Bottom":
                    isTouchBottom = false;
                    break;
                case "Right":
                    isTouchRight = false;
                    break;
                case "Left":
                    isTouchLeft = false;
                    break;
            }
        }
    }
}