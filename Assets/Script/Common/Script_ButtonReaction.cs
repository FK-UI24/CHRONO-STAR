using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class Script_ButtonReaction : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    //カーソルが乗ったボタンのサイズを変更させるスクリプト
    //カーソルが外れると元のサイズに戻る
    //それぞれのボタンに直接アタッチする

    //３つの関数全てUnity側で用意されている関数
    //ポインターが乗ったとき/降りた時、無効になったとき


    [Header("カーソルが乗ったときのボタンのサイズ倍率")]
    [SerializeField] private float sizeScale=1.0f;

    //元のボタンのスケールを格納する用変数
    private Vector3 defaultScale;

    //アタッチされているボタンオブジェクトを格納する用変数
    private Button thisButton;

    private void Start()
    {
        //一番最初にもとのボタンのスケールを格納して覚えとく
        defaultScale = transform.localScale;

        //このスクリプトをアタッチしているオブジェクトのボタンコンポーネントを取得する
        thisButton = GetComponent<Button>();
    }

    //ボタンにカーソルが乗ったときの処理
    public void OnPointerEnter(PointerEventData eventData)
    {
        //ボタンが無効化されていたら動かないようにする
        if (thisButton.interactable == false) return;

        //元のScaleに設定した倍率を変える
        transform.localScale = defaultScale * sizeScale;
    }

    //ボタンからカーソルが降りた時の処理
    public void OnPointerExit(PointerEventData eventData)
    {
        //こっちは常に動いていても問題ないのでreturnを入れない

        //全てのスケールのサイズを戻す
        transform.localScale = defaultScale;
    }

    //もし非アクティブになった時の処理
    private void OnDisable()
    {
        //全てのスケールのサイズを戻す
        transform.localScale = defaultScale;
    }
}
