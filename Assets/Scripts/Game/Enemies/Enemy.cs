using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AI;
using System.Runtime.CompilerServices;
using TMPro;

public class Enemy : MonoBehaviour
{
    private Transform targetTransform; // 공격 목표
    private Animator animator;

    [SerializeField] private GameObject robotDeath;
    private string monsterId;

    private HpBar hpBar; // 체력바

    [SerializeField] private float moveSpeed = 2f;
    private SpriteRenderer spriteRenderer;
    [SerializeField] private Color hitColor;
    private Color originalColor;
    private Rigidbody2D rigid;       // Rigidbody2D컴포넌트
    private Vector2? nextPos = null;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        targetTransform = GameObject.FindGameObjectWithTag("Base").transform;

        hpBar = GetComponentInChildren<HpBar>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
    }

    // Update is called once per frame
    void Update()
    {
        if (nextPos != null)
        {
            //this.transform.SetPositionAndRotation(new Vector3(nextPos.Value.x, nextPos.Value.y, 0), Quaternion.identity);
            //rigid.MovePosition(Vector2.Lerp(rigid.position, nextPos.Value, moveSpeed * Time.deltaTime));
            
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(nextPos.Value.x, nextPos.Value.y, 0), Time.deltaTime * this.moveSpeed);
        }
        //     animator.SetBool("isAttack", false);
        //     animator.SetBool("isWalk", true);
        //     Vector3 moveTo = (targetTransform.position - transform.position).normalized;
        //     transform.position += moveTo * currentSpeed * Time.deltaTime;

        //    // 바라보는 방향에 따라서 이미지 바뀌게
        //    Vector3 curScale = transform.localScale;
        //    Vector3 curHpBarScale = hpBar.transform.localScale;
        //    if (moveTo.x > 0)
        //    {
        //        transform.localScale = new Vector3(-Mathf.Abs(curScale.x), curScale.y, curScale.z);
        //        hpBar.transform.localScale = new Vector3(-Mathf.Abs(curHpBarScale.x), curHpBarScale.y, curHpBarScale.z);
        //    }
        //    else
        //    {
        //        transform.localScale = new Vector3(Mathf.Abs(curScale.x), curScale.y, curScale.z);
        //        hpBar.transform.localScale = new Vector3(Mathf.Abs(curHpBarScale.x), curHpBarScale.y, curHpBarScale.z);
        //    }
        //}
    }

    public void SetAttackMode()
    {
        animator.SetBool("isAttack", true);
        animator.SetBool("isWalk", false);
    }

    public void SetMoveMode()
    {
        animator.SetBool("isAttack", false);
        animator.SetBool("isWalk", true);
    }

    public void SetNextPos(Vector2 pos)
    {
        nextPos = pos;
        //transform.position = new Vector2(pos.x, pos.y);
        //MovePosition(Vector2.Lerp(rigid.position, targetPosition, moveSpeed * Time.deltaTime));
    }

    public void SetMonsterId(string uuid)
    {
        monsterId = uuid;
    }

    public void SetHp(float curHp, float maxHp)
    {
        hpBar.SetHp(curHp, maxHp);
        spriteRenderer.color = hitColor;
        Invoke("ResetColor", 0.1f);
    }

    public void Die()
    {
        Instantiate(robotDeath, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    private void ResetColor()
    {
        spriteRenderer.color = originalColor;
    }
}
