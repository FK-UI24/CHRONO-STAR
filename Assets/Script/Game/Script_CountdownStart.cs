using System.Collections;
using TMPro;
using UnityEngine;

public class Script_CountdownStart : MonoBehaviour
{
    [Header("カウントダウン時に使用されるパネル")]
    [SerializeField] private GameObject countdownPanel;

    [Header("カウントダウンをするときに使うテキストオブジェクト")]
    [SerializeField] private TMP_Text countdownText;

    [Header("カウントダウンの秒数")]
    [SerializeField] private int countdown = 3;

    [Header("カウントダウンが終わってから有効化するオブジェクト")]
    [SerializeField] private GameObject[] startUpObject;

    //オブジェクトにアタッチされている「カウントダウン」「開始」のAudioSourceを格納する用変数
    private AudioSource[] SEs;

    private void Start()
    {
        //開始後にスタートさせるオブジェクトは全て無効にしておく
        if (startUpObject.Length > 0)
        {
            for (int i = 0; i < startUpObject.Length; i++)
            {
                startUpObject[i].SetActive(false);
            }
        }

        //カウントダウンパネルがあったら起動しておく
        if (countdownPanel != null) countdownPanel.SetActive(true);

        //カウントダウンをするテキストがあったら初期値を入れとく
        countdownText.text = "画面をタッチしてスタート";

        //アタッチしているSEを格納する
        SEs = GetComponents<AudioSource>();

    }

    //ボタンコンポーネントを付けたパネルが押されたときに呼び出す関数
    public void OnCountdownPanel()
    {
        //連続クリック防止する
        countdownPanel.GetComponent<UnityEngine.UI.Button>().interactable = false;

        //カウントダウンを開始するコルーチンを始める
        StartCoroutine(StartCountdown());

    }

    //タイマーを稼働させカウントダウンをするコルーチン
    private IEnumerator StartCountdown()
    {
        //タイマーに設定した時間を入れる
        int timer = countdown;

        //タイマーがー0より大きい間
        while (timer > 0)
        {
            //タイマーの現在の時間をカウントダウンテキストに入れる
            countdownText.text = timer.ToString();

            //カウントダウンの音を鳴らす
            SEs[0].Play();

            //1秒待つ
            yield return new WaitForSeconds(1f);

            //１秒待ったのでタイマーから１引く
            timer--;
        }

        //0になったらテキストを「START」にする
        countdownText.text = "START";

        //開始の音を鳴らす
        SEs[1].Play();

        //「START」がすぐに消えないように0.5秒まつ
        yield return new WaitForSeconds(1f);
        
        //カウントダウンパネルを消す
        countdownPanel.SetActive(false);

        //一部の開始後に起動するオブジェクトをまとめて起動させる
        if (startUpObject.Length > 0)
        {
            for(int i = 0; i < startUpObject.Length; i++)
            {
                startUpObject[i].SetActive(true);
            }
        }


    }

}
