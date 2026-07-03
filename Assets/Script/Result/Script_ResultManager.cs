using TMPro;
using UnityEngine;

public class Script_ResultManager : MonoBehaviour
{
    [Header("結果表示用テキスト")]
    [SerializeField] private TMP_Text scoreText;

    [Header("ベストスコア報告テキスト")]
    [SerializeField] private GameObject bestScoreText;

    [Header("アドバイステキスト")]
    [SerializeField] private GameObject adviceText;

    void Start()
    {
        //最初は諸々非表示にする
        bestScoreText.SetActive(false);
        adviceText.SetActive(false);

        //スコアを参照する
        scoreText.text = Script_GameManager.score.ToString();

        //もしスコアが500以下ならアドバイスを表示する
        if (Script_GameManager.score <= 500)
        {
            adviceText.SetActive(true);
        }

        //もしベストスコアだったらデータ更新と報告テキストの表示
        UpdateScore(Script_GameManager.score);
        

    }

    private void UpdateScore(int score)
    {
        //現在のベストスコアを取得する
        int bestScore = PlayerPrefs.GetInt("bestscore", 0);

        //比較する
        if (score > bestScore)
        {
            //報告テキスト表示
            bestScoreText.SetActive(true);

            //更新
            PlayerPrefs.SetInt("bestscore", score);

            //セーブ
            PlayerPrefs.Save();
        }
    }

}
