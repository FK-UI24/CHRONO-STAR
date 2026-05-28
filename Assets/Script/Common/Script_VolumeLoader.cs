using UnityEngine;
using UnityEngine.Audio;

public class Script_VolumeLoader : MonoBehaviour
{
    [Header("メインのオーディオミキサー")]
    [SerializeField] private AudioMixer audioMixer;

    private void Start()
    {
        //保存されている音量(0～1)を取得する（データがなければ0.5f）
        float savedBGM = PlayerPrefs.GetFloat("BGMVolume", 0.5f);
        float savedSE = PlayerPrefs.GetFloat("SEVolume", 0.5f);

        //デシベルに変換して、このシーンのミキサーに即時に適用する
        float bgmVol = Mathf.Log10(Mathf.Clamp(savedBGM, 0.0001f, 1f)) * 20f;
        audioMixer.SetFloat("BGMVol", bgmVol);

        float seVol = Mathf.Log10(Mathf.Clamp(savedSE, 0.0001f, 1f)) * 20f;
        audioMixer.SetFloat("SEVol", seVol);
    }
}
