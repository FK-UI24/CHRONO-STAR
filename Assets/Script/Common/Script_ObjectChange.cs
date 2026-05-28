using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Script_ObjectChange : MonoBehaviour
{
    //設定したオブジェクトからオブジェクトに順々に変化させるスクリプト
    //ボタンなどを利用して手動で切り替える(マニュアルみたいに)のと
    //時間を設定して自動で切り替わる(カウントダウンとか)ようにする

    [Header("順番に表示したいゲームオブジェクトたち")]
    [SerializeField] private GameObject[] gameObjects;

    [Header("前に戻るボタン")]
    [SerializeField] private Button backButton;
    [Header("次に進むボタン")]
    [SerializeField] private Button nextButton;

    [Header("切り替わりにかかる時間")]
    [SerializeField] private float fadeTime = 0f;

    [Header("自動で切り替えるようにするか")]
    [SerializeField] private bool autoCahnge = false;

    [Header("自動で切り替わる秒数")]
    [SerializeField] private float autoChangeTime = 0f;

    //現在表示しているオブジェクトの番号
    private int currentIndex = 0;

    //連打防止や多重起動を防ぐためのコルーチン記録用
    private Coroutine currentFadeCoroutine;

    //自動切換え用のタイマー
    private float autoTimer = 0f;

    //ボタンを押したときのSEを格納する用変数
    private AudioSource SE;


    void Start()
    {
        //ボタンを押したときのSEを格納する用変数
        SE = GetComponent<AudioSource>();

        //最初のゲームオブジェクト以外を非表示にする
        for (int i = 0; i < gameObjects.Length; i++)
        {
            //最初のゲームオブジェクトは有効にする
            if (i == 0)
            {
                gameObjects[0].SetActive(true);
            }
            //それ以外は無効にする
            else
            {
                gameObjects[i].SetActive(false);
            }
        }

        //ゲーム開始時に戻る/次にボタンの更新をする
        UpdateButtonInteractable();
    }

    void Update()
    {
        //自動切り替えモードがオンの時だけタイマーを動かすから、手動切り替えの時は何もしない
        if (!autoCahnge) return;

        //フェード処理中はタイマーを動かさない
        if (currentFadeCoroutine != null) return;

        //タイマーを追加する
        autoTimer += Time.deltaTime;

        //もしタイマーが自動切り替えの時間になったら
        if (autoTimer >= autoChangeTime)
        {
            //タイマーを0にする
            autoTimer = 0f;

            //次のオブジェクトを表示する
            nextObject();

        }

    }

    //次に進むボタンを押したときの処理
    //ボタンの有効/無効の判定はSwitchObjectの最後に行われている
    public void nextObject()
    {
        //すでにフェード中なら何もしない
        if (currentFadeCoroutine != null) return;

        //もし既に最後のオブジェクトなら何もしない
        if (currentIndex >= gameObjects.Length - 1) return;

        //もしSEが格納されていて、かつ自動遷移が無効だったら音を鳴らす
        if (SE != null && autoCahnge == false)
        {
            SE.Play();
        }

        //次のオブジェクトに進めるので現在表示しているオブジェクトの番号を１増やす
        //あとコルーチンも記録もする
        currentFadeCoroutine = StartCoroutine(SwitchObject(currentIndex, currentIndex + 1));

        //オブジェクトを進めたので、インデックスを増やす
        currentIndex += 1;
    }

    //前に戻るボタンを押したときの処理
    //ボタンの有効/無効の判定はSwitchObjectの最後に行われている
    public void backObject()
    {
        //既にフェード中なら何もしない
        if (currentFadeCoroutine != null) return;

        //もし既に最初のオブジェクトなら何もしない
        if (currentIndex <= 0) return;

        //もしSEが格納されていて、かつ自動遷移が無効だったら音を鳴らす
        if (SE != null && autoCahnge == false)
        {
            SE.Play();
        }

        //前のオブジェクトに進めるので現在表示しているオブジェクトの番号を１減らす
        //あとコルーチンも記録する
        currentFadeCoroutine = StartCoroutine(SwitchObject(currentIndex, currentIndex - 1));

        //オブジェクトを戻したので、インデックスを減らす
        currentIndex -= 1;

    }
    

    //現在のオブジェクト番号を見て、ボタンの有効/無効を切り替える
    private void UpdateButtonInteractable()
    {
        //もし戻るボタンがあったら
        if (backButton != null)
        {
            //今表示されているオブジェクトの番号が0以上だったら有効に、そうじゃなかったら無効にする
            backButton.interactable = (currentIndex > 0);
        }
        //もし次にボタンがあったら
        if (nextButton != null)
        {
            //今表示されているオブジェクトの番号が最大だったら無効に、そうじゃなかったら有効にする
            nextButton.interactable = (currentIndex < gameObjects.Length - 1);
        }
    }

    //ページを切り替える用コルーチン
    //フェードアウトするオブジェクトとフェードインしてくるオブジェクトのインデックス番号を引数にする
    private IEnumerator SwitchObject(int fadeOutIdx,int fadeInIndx)
    {
        //フェードが始まった瞬間、両方のボタンを押せなくする
        if (backButton != null) backButton.interactable = false;
        if (nextButton != null) nextButton.interactable = false;

        //それぞれのオブジェクトのCanvasGroupを取得する
        ///オブジェクトの透明度を変えるにはCanvasGroupが楽なんだね～
        CanvasGroup fadeOutCg = gameObjects[fadeOutIdx].GetComponent<CanvasGroup>();
        CanvasGroup fadeInCg = gameObjects[fadeInIndx].GetComponent<CanvasGroup>();

        //１．今のオブジェクトをフェードアウトさせる
        //フェード用タイマーを格納する用変数
        float timer = 0f;

        //タイマーがフェード時間より短い間
        while (timer < fadeTime)
        {
            //タイマーを追加する
            timer += Time.deltaTime;

            //もしフェードアウトするオブジェクトのCanvasGroupがあったら
            if (fadeOutCg != null)
            {
                //透明度1～0に滑らかに変化させる
                fadeOutCg.alpha = Mathf.Lerp(1f, 0f, timer / fadeTime);
            }

            //ここで次のループに移行する
            yield return null;
        }

        //終わったら念のため完全に透明にする
        if (fadeOutCg != null) fadeOutCg.alpha = 0f;
        //ついでに無効にする
        gameObjects[fadeOutIdx].SetActive(false);

        //２．次のオブジェクトをフェードインさせる
        gameObjects[fadeInIndx].SetActive(true);

        //さらにフェードインさせるオブジェクトは最初透明にしておく
        //これでフェードインさせるときに最初は透明になる
        if (fadeInCg != null) fadeInCg.alpha = 0f;

        //フェードアウトで使ってたタイマーを再利用するため、初期化する
        timer = 0f;

        //初期化したタイマーがフェード時間より短い間
        while (timer < fadeTime)
        {
            //タイマーを追加する
            timer += Time.deltaTime;

            //もしフェードアウトするオブジェクトのCanvasGroupがあったら
            if (fadeInCg != null)
            {
                //透明度を0～1に滑らかに変化させる
                fadeInCg.alpha = Mathf.Lerp(0f, 1f, timer / fadeTime);
            }

            //ここで次のループに移行する
            yield return null;
        }

        //終わったら念のため完全に表示する
        if (fadeInCg != null) fadeInCg.alpha = 1f;

        //有効化は２の冒頭でやっているので特にしない

        //フェード処理が終わったからコルーチンの記録を初期化する
        currentFadeCoroutine = null;

        //新しいオブジェクトの表示をしたのでボタンの有効/無効のロードをする
        UpdateButtonInteractable();
    }

}
