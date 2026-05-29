using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Script_GameManager : MonoBehaviour
{
    [Header("スコアを表示する用テキスト")]
    [SerializeField] private TMP_Text scoreText;

    [Header("制限時間を表示するテキスト")]
    [SerializeField] private TMP_Text limitTimeText;

    [Header("制限時間")]
    [SerializeField] private int limitTime;

    //これが消えたタイミングで制限時間を指導させるので
    [Header("カウントダウンパネル")]
    [SerializeField]private GameObject countdownPanel;

    [Header("フェードアウト用のスクリプトがアタッチされたオブジェクト")]
    [SerializeField] private Script_SceneChange sceneChange;

    //スコアを格納する用変数
    //結果表示などで参照するのでpublic
    public static int score;

    //制限時間を扱う用の変数
    public static int limit;

    private void Start()
    {
        //最初に制限時間を格納する
        limit = limitTime;

        //最初にスコアを０にする
        score = 0;

        //制限時間を表示するテキストを初期値にする
        limitTimeText.text = limit.ToString();

        //スコアを表示するテキストを０にする
        scoreText.text = score.ToString();

        //制限時間の開始タイミングを監視するコルーチンを起動する
        StartCoroutine(FirstSetUp());
        
    }


    private IEnumerator FirstSetUp()
    {
        //カウントダウンパネルがアクティブ状態かつ存在していたら何もしない
        while (countdownPanel.activeSelf == true && countdownPanel != null) yield return null;

        //カウントダウンパネルの表示が終わったら制限時間のセットアップを行う
        StartCoroutine(LimitTimeCount());

    }



    private void Update()
    {
        //scoreを加減算する関数がstaticにしているからupdateで更新する
        scoreText.text = score.ToString();
    }

    //制限時間を制御するコルーチン
    private IEnumerator LimitTimeCount()
    {
        //残り制限時間が０秒より大きいとき
        while (limit > 0)
        {
            //１秒待機する
            yield return new WaitForSeconds(1);

            //制限時間を更新する
            limit--;

            //制限時間を更新する
            limitTimeText.text = limit.ToString();
        }

        sceneChange.ChangeScene("Result");

    }

    //弾のプレハブにアタッチしている「Script_Bullet」が扱うスコア加算関数
    public static void AddScore(int add)
    {
        //得点を追加する
        score += add;
    }

    //弾のプレハブにアタッチしている「Script_Bullet」が扱うスコア減算関数
    public static void PenaltyScore(int penalty)
    {
        //スコアが0を下回らないようにする
        if (score > 50)
        {
            score -= penalty;
        }
        else
        {
            score = 0;
        }
    }

    //弾のプレハブにアタッチしている「Script_Bullet」が扱う制限時間延長関数
    public static void AddTime(int add)
    {
        //念のため０以上のときだけタイム追加をするようにする
        if (limit >= 0)
        {
            limit += add;
        }

    }
}
