using UnityEngine;

public class Script_WishShot : MonoBehaviour
{
    [Header("発射する球のプレハブ")]
    [SerializeField] private GameObject wish;

    [Header("発射位置")]
    [SerializeField] private Transform firePoint;

    [Header("弾速")]
    [SerializeField] private float bulletSpeed;

    [Header("連射間隔")]
    [SerializeField] private float fireInterval;

    //射撃間の経過時間を計測する用変数
    private float fireTimer;

    //発射用のSEを格納する用変数
    private AudioSource fireSE;

    private void Start()
    {
        //同じオブジェクト内のSEを取得する
        fireSE = GetComponent<AudioSource>();

        //最初にあらかじめ発射許可が出る時間に設定する
        fireTimer = fireInterval;
    }

    private void Update()
    {
        //時間を加算する
        fireTimer += Time.deltaTime;


        //もし長押しをしたら
        if (Input.GetMouseButton(0))
        {
            //もし弾の射撃間隔時間が設定した間隔より大きければ
            if (fireTimer >= fireInterval)
            {
                //弾を生成
                bulletShot();

                //タイマーを0にする
                fireTimer = 0f;
            }
        }
    }

    //弾を発射する関数
    private void bulletShot()
    {
        //クリック位置にレイを飛ばす
        //カメラからマウス位置に向けて見えない線(Ray)を飛ばす
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        //弾が向かう目標地点を扱う用変数
        Vector3 targetPoint;

        //Rayが何かに当たったかを判定し、当たった情報をhitに入れる
        if(Physics.Raycast(ray,out RaycastHit hit))
        {
            //当たった座標をターゲット地点にする
            targetPoint = hit.point;
        }
        else
        {
            //何にも当たらなかった場合、前方50m先に設定する
            targetPoint = ray.origin + ray.direction * 50f;
        }

        //弾を発射位置に生成する（回転なし）
        GameObject bullet = Instantiate(wish, firePoint.position, Quaternion.identity);

        //発射位置から目標地点への方向ベクトルを作って長さ１にする
        Vector3 dir = (targetPoint - firePoint.position).normalized;

        //弾のRigidbodyを取得する
        Rigidbody rb = bullet.GetComponent<Rigidbody>();

        //方向×速度で移動速度を設定する
        rb.linearVelocity = dir * bulletSpeed;

        //弾の向きを進行方向に合わせる（見た目の向き）
        bullet.transform.forward = dir;

        //3秒後に弾を削除
        Destroy(bullet, 3f);

        //fireSEがあったら
        if (fireSE != null)
        {
            //音のピッチをランダムにする
            fireSE.pitch = Random.Range(0.9f, 1.1f);

            //鳴らす
            fireSE.Play();
        }

    }

}
