using UnityEngine.UI;
using UnityEngine;

public class SFXCtrl : MonoBehaviour
{
    [Header("Sound")]
    [SerializeField] GameObject soundOn;
    [SerializeField] GameObject soundOff;
    [Header("Music")]
    [SerializeField] GameObject musicOn;
    [SerializeField] GameObject musicOff;
    [Header("Vibrate")]
    [SerializeField] GameObject vibrateOn;
    [SerializeField] GameObject vibrateOff;
    [Header("Button")]
    [SerializeField] Button btnSound;
    [SerializeField] Button btnMusic;
    [SerializeField] Button btnVibrate;
    private void Start()
    {
        btnSound.onClick.AddListener(ChangeSound);
        btnMusic.onClick.AddListener(ChangeMusic);
        btnVibrate.onClick.AddListener(ChangeVibrate);
        if (!SFX.Instance.SoundEnable)
        {
            soundOn.SetActive(false);
            soundOff.SetActive(true);
        }
        if (!SFX.Instance.MusicEnable)
        {
            musicOn.SetActive(false);
            musicOff.SetActive(true);
        }
        if (!SFX.Instance.VibrateEnable)
        {
            vibrateOn.SetActive(false);
            vibrateOff.SetActive(true);
        }
    }
    private void ChangeSound()
    {
        SFX.Instance.SoundEnable = !SFX.Instance.SoundEnable;
        soundOn.SetActive(SFX.Instance.SoundEnable);
        soundOff.SetActive(!SFX.Instance.SoundEnable);
    }
    private void ChangeMusic()
    {
        SFX.Instance.MusicEnable = !SFX.Instance.MusicEnable;
        musicOn.SetActive(SFX.Instance.MusicEnable);
        musicOff.SetActive(!SFX.Instance.MusicEnable);
    }
    private void ChangeVibrate()
    {
        SFX.Instance.VibrateEnable = !SFX.Instance.VibrateEnable;
        vibrateOn.SetActive(SFX.Instance.VibrateEnable);
        vibrateOff.SetActive(!SFX.Instance.VibrateEnable);
    }
}
