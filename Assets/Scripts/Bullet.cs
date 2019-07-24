using System.Collections;
using UnityEngine;

public class Bullet : MonoBehaviour, IDamageDealer
{
    [SerializeField]
    private float _speed = 13.0f;
    [SerializeField]
    private int _damageAmount = 1;
    private GameObject _damageSource;
    private Rigidbody2D _rigid;

    public Bullet(GameObject damageSource)
    {
        _damageSource = damageSource;
    }

    public float Speed
    {
        get { return _speed; }
    }

    public Rigidbody2D Rigid
    {
        get { return _rigid; }
    }

    public int DamageAmount
    {
        get { return _damageAmount; }
        set { _damageAmount = value; }
    }

    public GameObject DamageSource
    {
        get { return _damageSource; }
        set { _damageSource = value; }
    }

    void Awake()
    {
        _rigid = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        StartCoroutine(VanishBullet());
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Player player = other.GetComponent<Player>();
        if (player != null)
        {
            player.Damage(_damageAmount, _damageSource);
            Destroy(gameObject);
        }
    }

    private IEnumerator VanishBullet()
    {
        yield return new WaitForSeconds(3.0f);
        if (gameObject != null)
        {
            Destroy(gameObject);
        }
    }
}
