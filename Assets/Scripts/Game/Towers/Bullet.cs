using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private GameObject explosion;
    [SerializeField] private float moveSpeed = 15f;
    private SpriteRenderer spriteRenderer;
    private float attackDamage;
    public float destroyAfter;
    public bool isSlowBullet = false;
    public bool isSplash = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Invoke("Remove", destroyAfter);
    }

    // Update is called once per frame
    void Update()
    {
        // 바라보고 있는 방향으로 이동
        transform.Translate(Vector3.up * moveSpeed * Time.deltaTime);
    }

    public void SetDamage(float newDamage)
    {
        attackDamage = newDamage;
    }

    public float GetDamage()
    {
        return attackDamage;
    }

    public void Remove()
    {
        Instantiate(explosion, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
