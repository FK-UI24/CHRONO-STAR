using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;


public class Script_VolumeSettings : MonoBehaviour
{
    [Header("メインのオーディオミキサー")]
    [SerializeField] private AudioMixer audioMixer;

    [Header("音量調整用のスライダー")]
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider seSlider;

    private void Start()
    {
        //保存されている音量(0～1)を取得する（データがなければ0.5f）
        float savedBGM = PlayerPrefs.GetFloat("BGMVolume", 0.5f);
        float savedSE = PlayerPrefs.GetFloat("SEVolume", 0.5f);

        //スライダーの見た目に保存されたデータを反映する
        if (bgmSlider != null) bgmSlider.value = savedBGM;
        if (seSlider != null) seSlider.value = savedSE;

        //起動時の実際の音量に反映する
        SetBGMVolume(savedBGM);
        SetSEVolume(savedSE);

        //スライダーを動かしたときに、リアルタイムで音量関数が実行されるようにする
        if (bgmSlider != null) bgmSlider.onValueChanged.AddListener(SetBGMVolume);
        if (seSlider != null) seSlider.onValueChanged.AddListener(SetSEVolume);

    }

    public void SetBGMVolume(float value)
    {
        //Mixerの音量はデシベル（-80～20）で計算するため、スライダーの０～１をMathf.Log10で変換する
        //０を代入するとバグるから、Mathf.Clampで最小値を0.0001fに固定する
        ///Mathf.Clampは指定した範囲の値を返す。最大値を上回っていたら最大値を、最小値を下回っていたら最小値を返す
        float volume = Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20f;
        audioMixer.SetFloat("BGMVol", volume);

        //データを保存する
        PlayerPrefs.SetFloat("BGMVolume", value);

    }

    public void SetSEVolume(float value)
    {
        //Mixerの音量はデシベル（-80～20）で計算するため、スライダーの０～１をMathf.Log10で変換する
        //０を代入するとバグるから、Mathf.Clampで最小値を0.0001fに固定する
        ///Mathf.Clampは指定した範囲の値を返す。最大値を上回っていたら最大値を、最小値を下回っていたら最小値を返す
        float volume = Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20f;
        audioMixer.SetFloat("SEVol", volume);

        //データを保存する
        PlayerPrefs.SetFloat("SEVolume", value);

    }

    //タイトルシーンから離れるときにデータを保存する
    private void OnDisable()
    {
        PlayerPrefs.Save();
    }

}
