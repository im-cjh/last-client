using Unity.VisualScripting;
using UnityEngine;

public class Tower : MonoBehaviour
{
    [SerializeField] private Transform cannon;
    private HpBar hpBar;
    [SerializeField] private float attackRange = 5f;
    [SerializeField] private float attackCoolDown = 2f;
    private float lastAttackTime;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        hpBar = GetComponentInChildren<HpBar>();
    }

    // Update is called once per frame
    void Update()
    {
        GameObject nearestEnemy = FindNearestEnemy();

        if(nearestEnemy != null)
        {
            if(Time.time >= lastAttackTime + attackCoolDown)
            {
                Attack(nearestEnemy);
                lastAttackTime = Time.time;
            }

            Vector3 targetPos = nearestEnemy.transform.position;
            targetPos.z = 0;

            Vector3 direction = targetPos - cannon.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            cannon.rotation = Quaternion.Euler(0, 0, angle - 90);
        }
        
    }

    GameObject FindNearestEnemy()
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, attackRange, LayerMask.GetMask("Enemy"));

        GameObject nearestEnemy = null;
        float shortestDistance = Mathf.Infinity;

        foreach(Collider2D enemy in enemies)
        {
            float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);
            if(distanceToEnemy < shortestDistance)
            {
                shortestDistance = distanceToEnemy;
                nearestEnemy = enemy.gameObject;
            }
        }

        return nearestEnemy;
    }

    void Attack(GameObject target)
    {
        
    }
}
