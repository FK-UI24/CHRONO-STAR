using UnityEngine;

public class Script_BGMManager : MonoBehaviour
{

    //BGMを管理するインスタンス
    private static Script_BGMManager BGMinstance;

    private void Awake()
    {
        //もしBGMインスタンスが既に存在していて、それがこのオブジェクトでない場合は、このオブジェクトを破壊する
        //じゅーふくぼーし
        if (BGMinstance != null && BGMinstance != this)
        {
            Destroy(gameObject);
            return;
        }
        //もしまだインスタンスがなかったら、このオブジェクトをインスタンスとして設定する
        BGMinstance = this;

        //シーンを切り替えても消えないようにする
        DontDestroyOnLoad(gameObject);
    }
}
