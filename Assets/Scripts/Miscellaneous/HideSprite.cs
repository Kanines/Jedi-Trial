using UnityEngine;

public class HideSprite : MonoBehaviour
{
    void Awake()
    {
        this.GetComponent<SpriteRenderer>().enabled = false;
    }
}
