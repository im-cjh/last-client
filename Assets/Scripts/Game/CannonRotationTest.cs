using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class CannonRotationTest : MonoBehaviour
{
    public Transform cannon;
    public float roationSpeed = 50f;

    void Update()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;

        Vector3 direction = mousePos - cannon.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        cannon.rotation = Quaternion.Euler(0, 0, angle - 90);
    }
}
