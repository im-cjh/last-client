using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    private Transform targetTransform;
    private NavMeshAgent agent;
    private float lastAttackTime;
    private HpBar hpBar;
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private float attackDamage = 10f;
    [SerializeField] private float attackCoolDown = 2f;
    [SerializeField] private float moveSpeed = 2f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        hpBar = GetComponentInChildren<HpBar>();
    }

    // Update is called once per frame
    void Update()
    {
        targetTransform = GameObject.FindGameObjectWithTag("Base").transform;
        float distanceToBase = Vector3.Distance(transform.position, targetTransform.position);

        GameObject nearestTower = FindNearestTower();
        
        if(nearestTower != null && distanceToBase > attackRange)
        {
            targetTransform = nearestTower.transform;
        }

        Vector3 moveTo = (targetTransform.position - transform.position).normalized;
        transform.position += moveTo * moveSpeed * Time.deltaTime;

        // 바라보는 방향에 따라서 이미지 바뀌게
        Vector3 curScale = transform.localScale;
        Vector3 curHpBarScale = hpBar.transform.localScale;
        if (moveTo.x > 0)
        {
            transform.localScale = new Vector3(-Mathf.Abs(curScale.x), curScale.y, curScale.z);
            hpBar.transform.localScale = new Vector3(-Mathf.Abs(curHpBarScale.x), curHpBarScale.y, curHpBarScale.z);
        } 
        else 
        {
            transform.localScale = new Vector3(Mathf.Abs(curScale.x), curScale.y, curScale.z);
            hpBar.transform.localScale = new Vector3(Mathf.Abs(curHpBarScale.x), curHpBarScale.y, curHpBarScale.z);
        }
    }

    GameObject FindNearestTower()
    {
        Collider2D[] towers = Physics2D.OverlapCircleAll(transform.position, attackRange, LayerMask.GetMask("Tower"));

        GameObject nearestTower = null;
        float shortestDistance = Mathf.Infinity;

        foreach(Collider2D tower in towers)
        {
            float distanceToTower = Vector3.Distance(transform.position, tower.transform.position);
            if(distanceToTower < shortestDistance)
            {
                shortestDistance = distanceToTower;
                nearestTower = tower.gameObject;
            }
        }

        return nearestTower;
    }
}
