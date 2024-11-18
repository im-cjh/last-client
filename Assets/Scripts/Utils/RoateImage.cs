using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RotateImage : MonoBehaviour
{
    private void Update()
    {
        transform.Rotate(0, 0, 2); // Z축 기준으로 회전
    }
}