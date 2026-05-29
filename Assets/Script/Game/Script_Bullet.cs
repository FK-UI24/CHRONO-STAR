using UnityEngine;

public class Script_Bullet : MonoBehaviour
{    private void OnCollisionEnter(Collision collision)
    {
        //もし当たった相手のタグがTargetなら
        if (collision.gameObject.CompareTag("Target"))
        {
            //当たった情報をゲームオブジェクト変数に格納する
            GameObject target = collision.gameObject;

            //もし当たったオブジェクトの名前に「Score」が入っていたら
            if (target.name.Contains("Score"))
            {
                Debug.Log("加点ターゲットに当たった");
                Script_GameManager.AddScore(50);
            }
            //もし「Penalty」が入っていたら
            else if (target.name.Contains("Penalty"))
            {
                Debug.Log("減点ターゲットに当たった");
                Script_GameManager.PenaltyScore(100);
            }
            //もし「Time」が入っていたら
            else if (target.name.Contains("Time"))
            {
                Debug.Log("時間ターゲットに当たった");
                Script_GameManager.AddTime(2);
            }
            //念のため例外も
            else
            {
                Debug.Log("不明なターゲットに当たった！ダレコレ!?");
            }

            //当たったターゲットの削除
            Destroy(target);

            //当たった弾の削除
            Destroy(gameObject);
        }
    }
}
