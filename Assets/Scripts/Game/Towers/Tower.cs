using Protocol;
using System;
using Unity.VisualScripting;
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
    public GameObject atkBuffEffect;
    public GameObject asBuffEffect;

    private AudioSource audioSource; // Inspector에서 할당

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        hpBar = GetComponentInChildren<HpBar>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        cannon = transform.Find("Cannon");
        firePoint = cannon.transform.Find("FirePoint");
        originalColor = spriteRenderer.color;
        audioSource = GetComponent<AudioSource>();
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

        string targetUuid = monsterPos.Uuid;

        Monster targetMonster = MonsterManager.instance.GetMonsterByUuid(targetUuid);
        if (targetMonster != null)
        {
            // 타겟의 방향, 각도 계산
            Vector3 direction = targetMonster.transform.position - cannon.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            // 각도 방향으로 대포 돌려
            cannon.rotation = Quaternion.Euler(0, 0, angle - 90);

            // 총알 발사
            GameObject spawnedBullet = Instantiate(bullet, firePoint.position, cannon.rotation);

            //소리 재생
            if (audioSource != null)
            {
                audioSource.Play(); 
            }

            // travelTime 동안 날아가고 사라짐
            Bullet bulletScript = spawnedBullet.GetComponent<Bullet>();
            bulletScript.destroyAfter = travelTime / 1000;
        }
    }

    private void InterpolatePosition(Vector3 serverPosition, Vector3 clientPosition)
    {

    }

    public void SetHp(float curHp)
    {
        hpBar.SetHp(curHp);
        spriteRenderer.color = hitColor;
        Invoke("ResetColor", 0.1f);
    }

    public void SetBuffEffect(string buffType, bool state)
    {
        switch (buffType)
        {
            case "atkBuff":
                atkBuffEffect.SetActive(state);
                break;
            case "asBuff":
                asBuffEffect.SetActive(state);
                break;
        }
    }

    public void Destroy()
    {
        Instantiate(towerExplosion, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    private void ResetColor()
    {
        spriteRenderer.color = originalColor;
    }
}


