using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private static UIManager _instance;
    public static UIManager Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.Log("UI Manager is Null!");
            }
            return _instance;
        }
    }

    private void Awake()
    {
        _instance = this;
    }

    public Sprite healthUnitSprite;
    public Sprite healthUnitMissingSprite;
    public Image[] healthBar;
    public Text scoreText;

    public void UpdateHealth(int remainingHealth)
    {
        for (int i = 0; i < healthBar.Length; i++)
        {
            if (i < remainingHealth)
            {
                healthBar[i].sprite = healthUnitSprite;
            }
            else
            {
                healthBar[i].sprite = healthUnitMissingSprite;
            }
        }
    }

    public void UpdateScore(int score)
    {
        scoreText.text = "" + score;
    }
}
