using UnityEngine;

public class HideSprite : MonoBehaviour
{
    void Start()
    {
        this.GetComponent<SpriteRenderer>().enabled = false;
    }
}
