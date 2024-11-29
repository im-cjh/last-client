using System.Collections.Generic;
using UnityEngine;

public class BuffTower : Tower
{
    [SerializeField] private float buffRange = 5f; // 버프 범위
    [SerializeField] private float damageBuffAmount = 2f; // 공격력 버프량
    [SerializeField] private float speedBuffAmount = 0.5f; // 공속 버프량
    private HashSet<Tower> buffedTowers = new HashSet<Tower>();

    void Update()
    {
        ApplyBufftoNearbyTowers();
    }

    private void ApplyBufftoNearbyTowers()
    {
        // 범위 내 타워 탐지
        Collider2D[] towers = Physics2D.OverlapCircleAll(transform.position, buffRange, LayerMask.GetMask("Tower"));

        // 버프 못받은 타워한테 버프
        foreach (Collider2D target in towers)
        {
            Tower tower = target.GetComponent<Tower>();
            if (tower != null && tower != this && !buffedTowers.Contains(tower))
            {
                tower.ApplyBuff(damageBuffAmount, speedBuffAmount);
                buffedTowers.Add(tower);
            }
        }

        buffedTowers.RemoveWhere(tower =>
        {
            if (tower == null || Vector3.Distance(transform.position, tower.transform.position) > buffRange)
            {
                tower?.RemoveBuff(damageBuffAmount, speedBuffAmount);
                return true;
            }
            return false;
        });
    }

    private void OnDestroy()
    {
        foreach (Tower target in buffedTowers)
        {
            if (target != null)
            {
                target.RemoveBuff(damageBuffAmount, speedBuffAmount);
            }
        }
    }
}
