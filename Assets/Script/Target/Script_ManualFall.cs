using System.Collections;
using UnityEngine;

//マニュアル用のターゲットを流すスクリプト
//空オブジェクトにアタッチする
//また同じオブジェクトにSEを複数付ける
//①ターゲットをランダムに出す
//②種類（加点/減点/時間）を決める
//③見た目・音を変える
//④左下に流す
//⑤画面外で消す

public class Script_ManualFall : MonoBehaviour
{
    [Header("加点ターゲット")]
    [SerializeField] private GameObject scoreTarget;

    [Header("加点オブジェクトの色：16進数")]
    [SerializeField] private string[] hexColors;

    [Header("減点ターゲット")]
    [SerializeField] private GameObject penaltyTarget;

    [Header("時間ターゲット")]
    [SerializeField] private GameObject timeTarget;

    //出現確立は合計100じゃなくてもok
    [Header("加点ターゲットの出現確率")]
    [SerializeField] private float scoreRate;

    [Header("減点ターゲットの出現確率")]
    [SerializeField] private float penaltyRate;

    [Header("時間ターゲットの出現確率")]
    [SerializeField] private float timeRate;

    [Header("出現間隔")]
    [SerializeField] private float spawnInterval;

    [Header("最大同時出現数")]
    [SerializeField] private int maxTargets;

    [Header("スポーン範囲：右側")]
    [SerializeField] private Vector2 xRange;

    [Header("スポーン範囲：上側")]
    [SerializeField] private Vector2 yRange;

    [Header("スポーン範囲：奥行")]
    [SerializeField] private Vector2 zRange;

    [Header("最大落下速度")]
    [SerializeField] private float maxSpeed;

    [Header("最低落下速度")]
    [SerializeField] private float minSpeed;

    //時間をカウントし、いつスポーンするかを管理する用変数
    private float timer;

    //次にスポーンするまでの目標時間を管理する用変数
    private float nextSpawnTime;

    //同じオブジェクトにアタッチされているSEを格納しておく用変数
    private AudioSource[] SEs;

    private void Start()
    {
        //SEを取得する
        SEs=GetComponents<AudioSource>();

        //最初の生成までの時間をランダムで設定する
        SetNextSpawn();
    }

    private void Update()
    {
        //毎フレームの時間を加算して経過時間を更新する
        timer += Time.deltaTime;

        //もし時間経過かつ最大数未満なら
        if (timer >= nextSpawnTime && CountTargets() < maxTargets)
        {
            //ターゲット生成処理を呼び出す
            Spawn();

            //タイマーのリセット
            timer = 0f;

            //次の出現時間を再度ランダムに決める
            SetNextSpawn();
        }
    }

    //次の出現時間を決める
    private void SetNextSpawn()
    {
        //基準の0.5～1.5倍でランダム時間設定
        nextSpawnTime = Random.Range(spawnInterval * 0.5f, spawnInterval * 1.5f);
    }

    //現在生成されているターゲットの数を数える関数
    private int CountTargets()
    {
        //タグが「Target」のオブジェクトを全て探して個数を返す
        return GameObject.FindGameObjectsWithTag("Target").Length;
    }

    //ターゲットを生成する関数
    private void Spawn()
    {
        //3D座標の生成
        Vector3 pos = new Vector3(
            Random.Range(xRange.x, xRange.y),       //x位置をランダムで決定
            Random.Range(yRange.x, yRange.y),       //y位置をランダムで決定
            Random.Range(zRange.x, zRange.y)        //z位置をランダムで決定
            );

        //確率に応じて生成するターゲット種類を決定
        GameObject prefab = GetRandomTarget();

        //Instatiateでクローンを作成。pos位置に回転(identity=0)で実体化
        GameObject obj = Instantiate(prefab, pos, Quaternion.identity);

        //加点ターゲット限定の装飾
        if (prefab == scoreTarget)
        {
            //色の変更
            if (hexColors.Length > 0)
            {
                //ランダムで色のリストから１つ選ぶ
                string hex = hexColors[Random.Range(0, hexColors.Length)];

                //もし16進数の文字列をカラーコードに変換できたら
                ///TryParseHtmlStringの第一引数に変換元、第二引数には正しく変換できた時の色が保存される
                if(ColorUtility.TryParseHtmlString(hex,out Color color))
                {
                    //見た目を管理するRendererを取得する
                    Renderer rend = obj.GetComponent<Renderer>();

                    //マテリアルをコピーして個別設定を可能にする
                    Material mat = new Material(rend.material);

                    //基本色を正しく変換できた時の色にする
                    mat.color = color;
                    //金属感を最大に設定
                    mat.SetFloat("_Metallic", 1f);
                    //表面の滑らかさを最大に設定
                    mat.SetFloat("_Smoothness", 1f);
                    //発光を有効化
                    mat.EnableKeyword("_EMISSION");

                    //マテリアルをオブジェクトに適用する
                    rend.material = mat;
                }
            }

            //音が設定されている場合のみSEを鳴らす処理
            if (SEs.Length > 0)
            {
                //ランダムに15回に1回だけSE鳴らす
                if (Random.Range(0, 15) == 0)
                {
                    //SEをランダムに１つ選ぶ
                    AudioSource SE = SEs[Random.Range(0, SEs.Length)];

                    //音のピッチをランダムにする
                    SE.pitch = Random.Range(0.8f, 1.2f);

                    //音を再生する
                    ///PlayOneShotはPlayと違い重複再生が可能で音が途切れない
                    SE.PlayOneShot(SE.clip);

                }
            }
        }

        //基本の移動方向（右上から左下）
        Vector3 dir = new Vector3(-1f, -1f, 0f);

        //X方向にランダムな揺らぎを追加する
        dir.x += Random.Range(-0.3f, 0.3f);

        //Y方向にランダムな揺らぎを追加する
        dir.y += Random.Range(-0.3f, 0.3f);

        //ベクトルの長さを１にして方向のみを使用する
        dir.Normalize();

        //指定範囲からランダムに速度を取得する
        float randomSpeed = Random.Range(minSpeed, maxSpeed);

        //Z距離を0～1に変換する
        float t = Mathf.InverseLerp(zRange.x, zRange.y, pos.z);

        //手前のオブジェクトは速く、奥は遅く補正する
        float speed = randomSpeed * Mathf.Lerp(1.2f, 0.7f, t);

        //実際に移動処理を開始する
        StartCoroutine(Move(obj, dir, speed));

    }

    //ランダムにターゲットの種類を決める関数
    private GameObject GetRandomTarget()
    {
        //設定された３つの比率を分母にする
        float total = scoreRate + timeRate + penaltyRate;

        //0から３つの比率の合計の中で乱数を取る
        float rand = Random.Range(0, total);

        //数値の境界線(累積)で判定し、該当するプレハブを返す
        //もし加点ターゲットの確率より小さかったら
        if (rand < scoreRate) return scoreTarget;
        //あるいは、加点ターゲットと減点ターゲットを足した確率より小さかったら
        //上のif文で加点ターゲットの場合は除外されている
        else if (rand < scoreRate + penaltyRate) return penaltyTarget;
        //ほかの場合は全て時間ターゲットを返す
        else return timeTarget;
    }

    //ターゲットの移動の処理を行う関数
    //Updateでやると今表示されている全てのターゲットの座標をリスト化して計算しないといけない
    //IEnumeratorなら１つのターゲットに対して、この関数が１つ付き、ターゲットの消滅と同時に消えるため効率的
    private IEnumerator Move(GameObject obj, Vector3 dir, float speed)
    {
        //ターゲットオブジェクトが消えるまで
        while (obj != null)
        {
            //現在の座標を更新
            //現在地に方向と速度と１フレームの秒数をかけたものを足す
            obj.transform.position += dir * speed * Time.deltaTime;

            //画面の左端（-30）または下端（-40）を越えたらターゲットオブジェクトを破棄する
            if (obj.transform.position.x < -30f || obj.transform.position.y < -40f)
            {
                //ターゲットオブジェクトの破棄
                Destroy(obj);

                //このターゲットオブジェクト用のループを終了させる
                yield break;
            }

            //それ以外は1フレーム待機して、再度続ける
            yield return null;
        }
    }
}
