using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    private Transform targetTransform; // 공격 목표
    private Animator animator;

    [SerializeField] private GameObject robotDeath;

    private HpBar hpBar; // 체력바
    [SerializeField] private float maxHp; // 최대체력
    private float hp; // 현재 체력
    [SerializeField] private float detectRange = 3f; // 타워 감지 범위
    [SerializeField] private float defaultAttackDistanceX = 1f; // X축 공격사거리
    [SerializeField] private float defaultAttackDistanceY = 1f; // Y축 공격사거리
    [SerializeField] private float attackDamage = 10f; // 공격력
    [SerializeField] private float attackCoolDown = 2f; // 공속
    private float lastAttackTime;
    [SerializeField] private float moveSpeed = 2f;

    private SpriteRenderer spriteRenderer;
    [SerializeField] private Color hitColor;
    private Color originalColor;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        targetTransform = GameObject.FindGameObjectWithTag("Base").transform;

        hpBar = GetComponentInChildren<HpBar>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        hp = maxHp;
        originalColor = spriteRenderer.color;
    }

    // Update is called once per frame
    void Update()
    {
        float attackDistanceX = defaultAttackDistanceX;
        float attackDistanceY = defaultAttackDistanceY;

        targetTransform = GameObject.FindGameObjectWithTag("Base").transform;

        float distanceToBase = Vector3.Distance(transform.position, targetTransform.position);

        GameObject nearestTower = FindNearestTower();

        if (nearestTower != null && distanceToBase > detectRange)
        {
            targetTransform = nearestTower.transform;
        }

        if (targetTransform.tag == "Base")
        {
            attackDistanceX += 1f;
            attackDistanceY += 1f;
        }

        // 사거리 안에 목표가 있으면 공격
        if (Mathf.Abs(targetTransform.position.x - transform.position.x) < attackDistanceX
        && Mathf.Abs(targetTransform.position.y - transform.position.y) < attackDistanceY)
        {
            animator.SetBool("isWalk", false);
            if (Time.time >= lastAttackTime + attackCoolDown)
            {
                animator.SetBool("isAttack", true); // 여기서 애니메이션 재생되면서 Attack() 호출
                lastAttackTime = Time.time;
            }
            else
            {
                animator.SetBool("isAttack", false);
            }
        }
        else
        {
            animator.SetBool("isAttack", false);
            animator.SetBool("isWalk", true);
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
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Bullet")
        {
            Bullet bullet = other.gameObject.GetComponent<Bullet>();
            hp -= bullet.damage;
            bullet.Remove();

            if (hp <= 0)
            {
                Instantiate(robotDeath, transform.position, Quaternion.identity);
                Destroy(gameObject);
                ScoreManager.instance.AddScore();
            }
            else
            {
                // 맞았을 때 잠깐동안 색이 바뀜
                spriteRenderer.color = hitColor;
                Invoke("ResetColor", 0.1f);
            }

            hpBar.SetHp(hp, maxHp);
        }
    }

    private void ResetColor()
    {
        spriteRenderer.color = originalColor;
    }

    GameObject FindNearestTower()
    {
        Collider2D[] towers = Physics2D.OverlapCircleAll(transform.position, detectRange, LayerMask.GetMask("Tower"));

        GameObject nearestTower = null;
        float shortestDistance = Mathf.Infinity;

        foreach (Collider2D tower in towers)
        {
            float distanceToTower = Vector3.Distance(transform.position, tower.transform.position);
            if (distanceToTower < shortestDistance)
            {
                shortestDistance = distanceToTower;
                nearestTower = tower.gameObject;
            }
        }

        return nearestTower;
    }

    private void Attack()
    {
        if (targetTransform.tag == "Base")
        {
            Base target = targetTransform.GetComponent<Base>();
            target.GetDamage(attackDamage);
        }
        else if (targetTransform.tag == "Tower")
        {
            Tower target = targetTransform.GetComponent<Tower>();
            target.GetDamage(attackDamage);
        }
    }
}
