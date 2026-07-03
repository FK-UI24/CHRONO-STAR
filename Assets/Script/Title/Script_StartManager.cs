using TMPro;
using UnityEngine;

public class Script_StartManager : MonoBehaviour
{

    [Header("ベストスコアテキスト")]
    [SerializeField] private TMP_Text bestScore;

    void Start()
    {
        bestScore.text = PlayerPrefs.GetInt("bestscore", 0).ToString();
    }
}
