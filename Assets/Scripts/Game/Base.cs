using System;
using UnityEngine;

public class Base : MonoBehaviour
{
    [SerializeField] private GameObject baseExplosion;
    private HpBar hpBar; // 체력바
    [SerializeField] private float maxHp = 300; // 최대체력
    private float hp; // 현재 체력
    public static Base instance = null;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        hpBar = GetComponentInChildren<HpBar>();
        hp = maxHp;
    }

    public void GetDamage(float damage)
    {
        hp -= damage;
        hpBar.SetHp(hp, maxHp);

        if (hp <= 0)
        {
            Instantiate(baseExplosion, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}
