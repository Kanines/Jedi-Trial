using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private static UIManager _instance;
    [SerializeField]
    private Sprite _healthUnitSprite;
    [SerializeField]
    private Sprite _healthUnitMissingSprite;
    [SerializeField]
    private Image[] _healthBar;
    [SerializeField]
    private Text _scoreText;

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

    void Awake()
    {
        _instance = this;
    }

    public void UpdateHealth(int remainingHealth)
    {
        for (int i = 0; i < _healthBar.Length; i++)
        {
            if (i < remainingHealth)
            {
                _healthBar[i].sprite = _healthUnitSprite;
            }
            else
            {
                _healthBar[i].sprite = _healthUnitMissingSprite;
            }
        }
    }

    public void UpdateScore(int newScore)
    {
        _scoreText.text = "" + newScore;
    }
}
