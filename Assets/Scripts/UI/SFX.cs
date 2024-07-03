using UnityEngine;
using MoreMountains.NiceVibrations;

public class SFX : MonoBehaviour
{
    public static SFX Instance;
    public AudioSource bgAudioSource;
    public AudioSource audioSource;
    public AudioClip clickSound;
    public bool SoundEnable
    {
        get { return UserData.SoundEnable; }
        set { UserData.SoundEnable = value; }
    }
    public bool MusicEnable
    {
        get { return UserData.MusicEnable; }
        set { 
            UserData.MusicEnable = value;
            if (value) PlayBGM();
            else PauseBGM();
        }
    }
    public bool VibrateEnable
    {
        get { return UserData.VibrateEnable; }
        set { UserData.VibrateEnable = value; }
    }
    private void Awake()
    {
        Instance = this;
    }
    public void PlayClickSound()
    {
        if (UserData.SoundEnable) { 
            audioSource.PlayOneShot(clickSound);
        }
    }
    public void Vibrate(bool heavy = false)
    {
        if (!VibrateEnable) return;
        if (!heavy) MMVibrationManager.Vibrate();
        else MMVibrationManager.Haptic(HapticTypes.HeavyImpact);
    }
    public void Haptic()
    {
        if (!VibrateEnable) return;
        MMVibrationManager.Haptic(HapticTypes.LightImpact);
    }
    public void PlayBGM()
    {
        if (!MusicEnable) return;
        bgAudioSource.Play();
    }
    public void PauseBGM()
    {
        if (MusicEnable) return;
        bgAudioSource.Pause();
    }
    public void PlaySound(AudioClip clip, bool loop = false)
    {
        if (!SoundEnable || audioSource == null || clip == null) return;
        if (loop)
        {
            audioSource.Stop();
            audioSource.clip = clip;
            audioSource.loop = true;
            audioSource.Play();
        }
        else
        {
            audioSource.Stop();
            audioSource.PlayOneShot(clip);
        }
    }
    public void StopAudio()
    {
        if (audioSource == null) return;
        audioSource.Stop();
        audioSource.clip = null;
        audioSource.loop = false;
    }
}
