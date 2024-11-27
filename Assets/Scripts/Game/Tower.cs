using System;
using Unity.VisualScripting;
using UnityEngine;

public class Tower : MonoBehaviour
{
    [SerializeField] private GameObject towerExplosion;

    private string towerId;

    private HpBar hpBar; // 체력바
    [SerializeField] private float maxHp = 100f; // 최대체력
    private float hp; // 현재 체력

    [SerializeField] private float attackRange = 5f; // 공격 범위 (인식 범위)
    [SerializeField] private float attackCoolDown = 2f; // 공격속도
    private float lastAttackTime; // 마지막 공격 시간
    private Transform cannon; // 대포
    [SerializeField] private GameObject bullet; // 총알
    private Transform firePoint; // 총알 나가는 위치

    private SpriteRenderer spriteRenderer;
    [SerializeField] private Color hitColor;
    private Color originalColor;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        hpBar = GetComponentInChildren<HpBar>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        cannon = transform.Find("Cannon");
        firePoint = cannon.transform.Find("FirePoint");
        hp = maxHp;
        originalColor = spriteRenderer.color;
    }

    // Update is called once per frame
    void Update()
    {
        // 가장 가까운 적 탐지
        GameObject nearestEnemy = FindNearestEnemy();

        if (nearestEnemy != null)
        {
            Vector3 targetPos = nearestEnemy.transform.position;
            targetPos.z = 0;

            Vector3 direction = targetPos - cannon.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            // 적 방향으로 대포 방향 돌림
            cannon.rotation = Quaternion.Euler(0, 0, angle - 90);

            // 공격속도에 맞춰서 공격
            if (Time.time >= lastAttackTime + attackCoolDown)
            {
                Instantiate(bullet, firePoint.position, cannon.rotation);
                lastAttackTime = Time.time;
            }
        }

    }

    GameObject FindNearestEnemy()
    {
        // 게임에 있는 모든 적 저장하는 배열
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, attackRange, LayerMask.GetMask("Enemy"));

        GameObject nearestEnemy = null; // 가장 가까운 적
        float shortestDistance = Mathf.Infinity; // 가장 가까운 거리

        foreach (Collider2D enemy in enemies)
        {
            float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);
            if (distanceToEnemy < shortestDistance)
            {
                shortestDistance = distanceToEnemy;
                nearestEnemy = enemy.gameObject;
            }
        }

        return nearestEnemy;
    }

    public void GetDamage(float damage)
    {
        hp -= damage;
        hpBar.SetHp(hp, maxHp);

        if (hp <= 0)
        {
            Instantiate(towerExplosion, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
        else
        {
            // 맞았을 때 잠깐동안 색이 바뀜
            spriteRenderer.color = hitColor;
            Invoke("ResetColor", 0.1f);
        }
    }

    private void ResetColor()
    {
        spriteRenderer.color = originalColor;
    }
}


