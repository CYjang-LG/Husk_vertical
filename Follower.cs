using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 플레이어를 따라다니며 자동으로 발사하는 팔로워
/// </summary>
public class Follower : MonoBehaviour
{
    [Header("Shooting Settings")]
    public float maxShotDelay = 0.3f;
    public float curShotDelay;

    [Header("References")]
    public ObjectManager objectManager;
    public Transform parent; // 플레이어

    [Header("Follow Settings")]
    public Vector3 followPos;
    public int followDelay = 10; // 프레임 딜레이

    private Queue<Vector3> parentPos;
    private Player playerScript;

    void Awake()
    {
        parentPos = new Queue<Vector3>();

        // 플레이어 스크립트 찾기
        if (parent != null)
        {
            playerScript = parent.GetComponent<Player>();
        }
    }

    void Update()
    {
        Watch();
        Follow();
        Fire();
        Reload();
    }

    void Watch()
    {
        // 부모(플레이어)의 위치를 큐에 저장
        if (parent != null && !parentPos.Contains(parent.position))
        {
            parentPos.Enqueue(parent.position);
        }

        // 딜레이만큼 이전 위치 사용
        if (parentPos.Count > followDelay)
        {
            followPos = parentPos.Dequeue();
        }
        else if (parentPos.Count < followDelay && parent != null)
        {
            followPos = parent.position;
        }
    }

    void Follow()
    {
        transform.position = followPos;
    }

    void Fire()
    {
        // ✅ 플레이어가 발사할 때만 팔로워도 발사
        if (playerScript != null)
        {
            // 플레이어의 발사 버튼 상태 확인
            bool isPlayerShooting = playerScript.isButtonA || Input.GetButton("Fire1");

            if (!isPlayerShooting)
                return;
        }
        else
        {
            // playerScript가 없으면 기본 Fire1 버튼 체크
            if (!Input.GetButton("Fire1"))
                return;
        }

        if (curShotDelay < maxShotDelay)
            return;

        GameObject bullet = objectManager.MakeObj("BulletFollower");
        if (bullet != null)
        {
            bullet.transform.position = transform.position;
            Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();

            if (rigid != null)
            {
                rigid.linearVelocity = Vector2.up * 10; // AddForce 대신 velocity 사용
            }
        }

        curShotDelay = 0;
    }

    void Reload()
    {
        curShotDelay += Time.deltaTime;
    }

    // 팔로워가 비활성화될 때 큐 초기화
    void OnDisable()
    {
        if (parentPos != null)
        {
            parentPos.Clear();
        }
    }
}