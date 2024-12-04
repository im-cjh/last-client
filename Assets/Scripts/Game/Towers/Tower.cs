using Protocol;
using System;
using Unity.VisualScripting;
using UnityEditor.ShaderGraph;
using UnityEngine;

public class Tower : MonoBehaviour
{
    [SerializeField] private GameObject towerExplosion;
    private string towerId;

    private HpBar hpBar; // 체력바
    private Transform cannon; // 대포
    [SerializeField] private GameObject bullet; // 총알
    private Transform firePoint; // 총알 나가는 위치

    private SpriteRenderer spriteRenderer;
    [SerializeField] private Color hitColor;
    private Color originalColor;
    private GameObject buffEffect;
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        hpBar = GetComponentInChildren<HpBar>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        cannon = transform.Find("Cannon");
        buffEffect = transform.Find("BuffEffect")?.gameObject;
        firePoint = cannon.transform.Find("FirePoint");
        originalColor = spriteRenderer.color;
    }

    public void SetTowerId(string uuid)
    {
        towerId = uuid;
    }

    public void AttackTarget(Protocol.PosInfo monsterPos, float travelTime)
    {
        Vector3 targetPos = new Vector3(
            monsterPos.X,
            monsterPos.Y,
            0
        );

        // 타겟의 방향, 각도 계산
        Vector3 direction = targetPos - cannon.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // 각도 방향으로 대포 돌려
        cannon.rotation = Quaternion.Euler(0, 0, angle - 90);

        // 총알 발사
        GameObject spawnedBullet = Instantiate(bullet, firePoint.position, cannon.rotation);

        // travelTime 동안 날아가고 사라짐
        Bullet bulletScript = spawnedBullet.GetComponent<Bullet>();
        bulletScript.destroyAfter = travelTime / 1000;
    }

    // Update is called once per frame
    // void Update()
    // {
    //     // 가장 가까운 적 탐지
    //     GameObject nearestEnemy = FindNearestEnemy();

    //     if (nearestEnemy != null)
    //     {
    //         Attack(nearestEnemy);
    //     }
    // }

    // private void Attack(GameObject target)
    // {
    //     Vector3 targetPos = target.transform.position;
    //     targetPos.z = 0;

    //     Vector3 direction = targetPos - cannon.position;
    //     float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

    //     // 적 방향으로 대포 방향 돌림
    //     cannon.rotation = Quaternion.Euler(0, 0, angle - 90);

    //     // 공격속도에 맞춰서 공격
    //     if (Time.time >= lastAttackTime + curAttackCoolDown)
    //     {
    //         GameObject spawnedBullet = Instantiate(bullet, firePoint.position, cannon.rotation);

    //         Bullet bulletScript = spawnedBullet.GetComponent<Bullet>();
    //         if (bulletScript != null)
    //         {
    //             bulletScript.SetDamage(curAttackDamage);
    //         }

    //         lastAttackTime = Time.time;
    //     }
    // }

    // GameObject FindNearestEnemy()
    // {
    //     // 게임에 있는 모든 적 저장하는 배열
    //     Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, attackRange, LayerMask.GetMask("Enemy"));

    //     GameObject nearestEnemy = null; // 가장 가까운 적
    //     float shortestDistance = Mathf.Infinity; // 가장 가까운 거리

    //     foreach (Collider2D enemy in enemies)
    //     {
    //         float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);
    //         if (distanceToEnemy < shortestDistance)
    //         {
    //             shortestDistance = distanceToEnemy;
    //             nearestEnemy = enemy.gameObject;
    //         }
    //     }

    //     return nearestEnemy;
    // }

    public void SetHp(float curHp, float maxHp)
    {
        hpBar.SetHp(curHp, maxHp);
        spriteRenderer.color = hitColor;
        Invoke("ResetColor", 0.1f);
    }

    private void ResetColor()
    {
        spriteRenderer.color = originalColor;
    }


    // public void RemoveBuff(float damageBuff, float speedBuff)
    // {
    //     curAttackDamage -= damageBuff;
    //     curAttackCoolDown += speedBuff;

    //     buffEffect.SetActive(false);

    //     // 값 복구
    //     if (curAttackDamage < attackDamage)
    //     {
    //         curAttackDamage = attackDamage;
    //     }
    //     if (curAttackCoolDown > attackCoolDown)
    //     {
    //         curAttackCoolDown = attackCoolDown;
    //     }
    // }
}


