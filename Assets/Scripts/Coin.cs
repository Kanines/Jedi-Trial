using UnityEngine;

public class Coin : MonoBehaviour
{
    [SerializeField]
    private int _scorePoints;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            Player player = other.GetComponent<Player>();
            if (player != null)
            {
                player.AddScore(_scorePoints);
                Destroy(this.gameObject);
            }
        }
    }
}
