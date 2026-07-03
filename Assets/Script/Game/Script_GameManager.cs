using Benjathemaker;
using System.Collections;
using System.ComponentModel;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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

    [Header("クロノゲージ")]
    [SerializeField] private Slider chronoGauge;

    [Header("クロノゲージの数値")]
    [SerializeField] private TMP_Text chronoGaugeText;

    //staticメソッドから触る用
    public static Slider ChronoGauge;

    //スコアを格納する用変数
    //結果表示などで参照するのでpublic
    public static int score;

    //制限時間を扱う用の変数
    public static int limit;

    //SEを格納する変数
    private static AudioSource[] SEs;

    //スキル使用中かのトリガー（True=使用中）
    private bool chronoSkilTrigger;

    //クロノゲージが50%以上の時に一度だけSEを鳴らす監視用トリガー
    private bool chronoHalfTrigger;

    //スローと時止め用で使うスケール
    //最初は通常速度
    public static float globalTimeScale;

    private void Start()
    {
        //最初に制限時間を格納する
        limit = limitTime;

        //最初にスコアを０にする
        score = 0;

        //タイムスケールのリセット
        globalTimeScale = 1.0f;

        //制限時間を表示するテキストを初期値にする
        limitTimeText.text = limit.ToString();

        //スコアを表示するテキストを０にする
        scoreText.text = score.ToString();

        //クロノゲージの初期化
        chronoGauge.value = 0f;

        //トリガーの初期化
        chronoSkilTrigger = false;
        chronoHalfTrigger = false;

        //クロノゲージの参照
        ChronoGauge = chronoGauge;

        //クロノゲージの数値を表示するテキストを初期化
        chronoGaugeText.text = ChronoGauge.value.ToString();

        //SEを格納する
        SEs = GetComponents<AudioSource>();

        //前のゲームでのリスナーが残っている場合を考えて、リセットする
        ChronoGauge.onValueChanged.RemoveAllListeners();

        //値が変わったらクロノゲージの数値を監視する関数を呼ぶように指示する
        ChronoGauge.onValueChanged.AddListener(CheckChronoGauge);

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

        //クロノゲージ50以上100未満であるときスロー発動
        if (Input.GetMouseButtonDown(1) && ChronoGauge.value >= 0.5 && ChronoGauge.value < 1 && !chronoSkilTrigger)
        {
            chronoHalfTrigger = false;
            StartCoroutine(SkillSlow());
        }

        //クロノゲージが100でであるときに時止め発動
        if (Input.GetMouseButtonDown(1) && ChronoGauge.value == 1.0 && !chronoSkilTrigger)
        {
            chronoHalfTrigger = false;
            StartCoroutine(SkillDIO());
        }

    }

    //スローを行う関数
    private IEnumerator SkillSlow()
    {
        //SEを鳴らす
        SEs[5].Play();

        while (ChronoGauge.value > 0)
        {
            //0.25倍速にする
            globalTimeScale = 0.5f;

            //スキル使用中トリガーを切り替える
            chronoSkilTrigger = true;

            //ゲージを少しずつ削る
            ChronoGauge.value -= Time.deltaTime * 0.1f;

            yield return null;

        }

        chronoSkilTrigger = false;
        globalTimeScale = 1.0f;

    }

    //時止めを行う関数
    private IEnumerator SkillDIO()
    {
        //SEを鳴らす
        SEs[6].Play();

        //ターゲットの動きを切り替える
        ChangeSimpleGemAnim(false);

        while (ChronoGauge.value > 0)
        {
            //速度を完全に止める
            globalTimeScale = 0f;

            //スキル使用中のトリガーを切り替える
            chronoSkilTrigger = true;

            //ゲームを少しづつ削る
            ChronoGauge.value -= Time.deltaTime * 0.25f;

            yield return null;

        }

        chronoSkilTrigger = false;
        globalTimeScale = 1.0f;
        //ターゲットの動きを切り替える
        ChangeSimpleGemAnim(true);


    }

    private void ChangeSimpleGemAnim(bool active)
    {
        //タグが「Target」を格納する
        GameObject[] targets = GameObject.FindGameObjectsWithTag("Target");

        //「Target」タグがついているオブジェクトの動きを制御するスクリプトを止める
        foreach (GameObject target in targets)
        {
            //落下の制御以外のアニメを制御するスクリプトを格納する
            SimpleGemsAnim script = target.GetComponent<SimpleGemsAnim>();

            //もしスクリプトがあったら
            if (script != null)
            {
                //「SimpleGemAnim」スクリプトを切り替える
                script.enabled = active;
            }

        }
    }

    //制限時間を制御するコルーチン
    //ここで制限時間表示テキストの更新をしている
    //→なぜ３か所もあるか...絶対制限時間リアルタイム反映マンだから
    //タイミング測るのダルすぎるから死ぬほど毎回確認する
    private IEnumerator LimitTimeCount()
    {
        //残り制限時間が０秒より大きいとき
        while (limit > 0)
        {
            //現在の制限時間を常にテキストに反映する
            limitTimeText.text = limit.ToString();

            //時止めの時は制限時間を直接減らさないようにする
            //多分0で割るとエラーが起きてコルーチンが解除されると思う
            while (globalTimeScale <= 0)
            {
                //現在の制限時間を常にテキストに反映する
                limitTimeText.text = limit.ToString();

                yield return null;
            }

            //１秒待機する
            yield return new WaitForSeconds(1 / globalTimeScale);

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
        //SEのピッチをランダムで変える
        SEs[2].pitch = Random.Range(0.8f, 1.2f);

        //SEを鳴らす
        SEs[2].Play();

        //得点を追加する
        if (globalTimeScale == 0.5)
        {
            score += add * 2;
        }
        else if (globalTimeScale == 0)
        {
            score += add * 5;
        }
        else
        {
            score += add;
        }

        //もしスローや時止めが発動していないときはクロノゲージも追加する
        if (globalTimeScale == 1.0f)
        {
            //クロノゲージを追加する
            ChronoGauge.value += 0.05f;
        }
    }

    //弾のプレハブにアタッチしている「Script_Bullet」が扱うスコア減算関数
    public static void PenaltyScore(int penalty)
    {
        //SEのピッチをランダムで変える
        SEs[3].pitch = Random.Range(0.8f, 1.2f);

        //SEを鳴らす
        SEs[3].Play();


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
        //SEのピッチをランダムで変える
        SEs[4].pitch = Random.Range(0.8f, 1.2f);

        //SEを鳴らす
        SEs[4].Play();

        //念のため０以上のときだけタイム追加をするようにする
        if (limit >= 0)
        {
            limit += add;

            //もしスローや時止めが発動していないときはクロノゲージも追加する
            if (globalTimeScale == 1.0f)
            {
                //クロノゲージを追加する
                ChronoGauge.value += 0.1f;
            }
        }

    }

    //クロノゲージの数値を監視する関数
    private void CheckChronoGauge(float value)
    {
        //クロノゲージの変更を数値に反映する
        chronoGaugeText.text = (ChronoGauge.value * 100).ToString("0");


        //もしクロノゲージが最大値に達したかつ、スキル使用中でなかったら
        if (value >= ChronoGauge.maxValue && !chronoSkilTrigger)
        {
            //効果音を鳴らす
            SEs[1].Play();
        }
        //50%を越えたらかつスキル使用中でなかったら
        else if (value >= (ChronoGauge.maxValue / 2) && !chronoSkilTrigger && !chronoHalfTrigger)
        {
            //50%以上のときに一度だけ鳴らすようにトリガーを切り替える
            chronoHalfTrigger = true;

            SEs[0].Play();
        }

    }

}
