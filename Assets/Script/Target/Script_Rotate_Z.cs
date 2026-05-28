using UnityEngine;

//１秒間に回転する度数を決めてZ軸回転をするスクリプト
//直接オブジェクトにアタッチする

public class Script_Rotate_Z : MonoBehaviour
{
    [Header("１秒間に回転する度数")]
    [SerializeField] private float rotationSpeed;

    private void Update()
    {
        //Y軸中心に一定速度で回転させる
        //Space.Selfを指定するとオブジェクト自身の軸、Space.Worldだと世界の軸で回る
        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime, Space.Self);
    }

}
