using UnityEngine;

public class AutoDestroy : MonoBehaviour
{
    [SerializeField] private float destroyAfter;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Destroy(gameObject, destroyAfter);
    }
}
