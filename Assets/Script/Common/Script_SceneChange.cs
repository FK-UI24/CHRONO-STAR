using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

//基本的に空のオブジェクトにつける
//このスクリプトがついたオブジェクトをボタンコンポーネントのOnClick()につける
//そこで遷移先シーンを入れる
//フェードインはアタッチしたオブジェクトが担う


public class Script_SceneChange : MonoBehaviour
{
    [Header("フェード用のパネルがあるCanvasGroup")]
    [SerializeField] private CanvasGroup canvasGroup;

    [Header("フェードアウトの時間")]
    [SerializeField] private float fadeOutTime = 0f;

    [Header("フェードインの時間")]
    [SerializeField] private float fadeInTime = 0f;

    //ボタンを押したときのSEを格納する用変数
    private AudioSource SE;

    private void Start()
    {
        //一応フェード用オブジェクトをオンにする
        canvasGroup.gameObject.SetActive(true);

        //シーン開始時に自動でフェードインさせる
        StartCoroutine(FadeIn());
    }

    //フェードアウトをするボタン用の関数
    //シーン遷移用のスクリプトを１つのオブジェクトで管理するために、
    //OnClick()から直接遷移先シーン名を調整できるようにしている
    public void ChangeScene(string nextScene)
    {

        //初っ端からSEを取得すると設定した音量を反映する前になってしまうから最大音量になる
        //ボタンを押したときのSEを格納する用変数
        SE = GetComponent<AudioSource>();

        //もしSEが格納されていたら音を鳴らす
        if (SE != null)
        {
            SE.Play();
        }
        
        //シーンチェンジを実行する
        StartCoroutine(FadeOut(nextScene));
    }

    //フェードインする処理
    //ぶっちゃけUpdateでもできるけど、フェードインが終わった後も無駄な処理をしてしまうため
    //つまり毎フレーム処理をしたいけど終わったら使わなくなるからコルーチンで実装する
    private IEnumerator FadeIn()
    {
        //フェードイン中、プレイヤーが誤って裏のボタンを押さないようにパネルにクリック判定を入れる
        canvasGroup.blocksRaycasts = true;

        //フェードインの開始時は、画面の透明度を１にして、表示されるようにする
        canvasGroup.alpha = 1f;

        //時間を計測するためのタイマーの時間を格納する用変数
        float timer = 0f;

        //タイマーの数値がインスペクター側で設定したフェードイン時間になるまで処理を繰り返す
        while (timer < fadeInTime)
        {
            //タイマーの時間を追加する
            timer += Time.deltaTime;

            //タイマーの進行度に合わせてalphaを１から０へ滑らかに変化させる
            ///Mathf.Lerpは第一引数（始まりの位置）から第二引数（終わりの位置）へ滑らかに移動する
            ///第三引数は0から1までが入り、0は初めの位置、1が終わりの位置である
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, timer / fadeInTime);

            //処理を次のフレームまで止める
            ///いわゆるreturnのコルーチン版である
            yield return null;
        }

        //ループが終わったからここで一旦完全に透明にする
        canvasGroup.alpha = 0f;

        //フェードインがおわったから当たり判定をなくして、ボタンをいじれるようにする
        canvasGroup.blocksRaycasts = false;
    }

    //フェードアウトとシーン遷移をする処理
    //フェードアウトそのものは遷移前のシーンで行われる
    //あとぶっちゃけUpdate処理でもできるけど、フラグ管理とかダルダルダルメシアンだし～
    //Updateだとこのフラグ変わったかな～ってずっと処理をし続けてしまう→これって無駄な処理では？
    private IEnumerator FadeOut(string nextScene)
    {
        ///フェードアウト中、プレイヤーが誤って裏のボタンを押さないようにパネルにクリック判定を入れる
        canvasGroup.blocksRaycasts = true;

        //時間を計測するためのタイマーの時間を格納する用変数
        float timer = 0f;

        //タイマーの数値がインスペクター側で設定したフェードアウト時間になるまで処理を繰り返す
        while (timer < fadeOutTime)
        {
            //タイマーの時間を追加する
            timer += Time.deltaTime;

            //タイマーの進行度に合わせてalphaを１から０へ滑らかに変化させる
            ///Mathf.Lerpは第一引数（始まりの位置）から第二引数（終わりの位置）へ滑らかに移動する
            ///第三引数は0から1までが入り、0は初めの位置、1が終わりの位置である
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, timer / fadeOutTime);

            //処理を次のフレームまで止める
            ///いわゆるreturnのコルーチン版である
            yield return null;
        }

        //ループが終わったからここで一旦完全に透明度を1にする
        canvasGroup.alpha = 1f;

        //フェードアウトが終了したらシーンを遷移する
        SceneManager.LoadScene(nextScene);
    }
}