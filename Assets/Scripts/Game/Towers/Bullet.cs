using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private GameObject explosion;
    [SerializeField] private float moveSpeed = 5f;
    private SpriteRenderer spriteRenderer;
    public float attackDamage;

    public bool isSlowBullet = false;
    public bool isSplash = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        // 7초 후 사라짐
        Destroy(gameObject, 7f);
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
