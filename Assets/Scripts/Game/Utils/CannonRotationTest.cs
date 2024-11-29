using System;
using Protocol;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class CannonRotationTest : MonoBehaviour
{
    [SerializeField] private GameObject bullet;
    private Transform launchTransform;
    [SerializeField] private Transform cannon;

    void Start()
    {
        launchTransform = transform.Find("Cannon").transform.Find("Launch");
    }

    void Update()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;

        Vector3 direction = mousePos - cannon.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        cannon.rotation = Quaternion.Euler(0, 0, angle - 90);

        if(Input.GetMouseButtonDown(0))
        {
            Attack();
        }
    }

    void Attack()
    {
        Instantiate(bullet, launchTransform.position, Quaternion.identity);
    }
}
