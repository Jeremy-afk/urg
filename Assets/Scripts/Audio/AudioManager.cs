using Mirror;
using UnityEngine;

public class AudioManager : NetworkBehaviour
{
    public static AudioManager Instance { get; private set; }

    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource ambianceSource;
    [SerializeField] private AudioSource soundEffectSources;

    private void Awake()
    {
        if (!isLocalPlayer) return;

        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    #region Play Methods
    public void PlayMusic(AudioClip clip)
    {
        musicSource.clip = clip;
        musicSource.Play();
    }

    public void PlayAmbiance(AudioClip clip)
    {
        ambianceSource.clip = clip;
        ambianceSource.Play();
    }

    public void PlaySoundEffect(AudioClip clip)
    {
        soundEffectSources.clip = clip;
        soundEffectSources.Play();
    }

    #endregion

    #region Stop Methods

    public void StopMusic()
    {
        musicSource.Stop();
    }

    public void StopAmbiance()
    {
        ambianceSource.Stop();
    }

    public void StopSoundEffect()
    {
        soundEffectSources.Stop();
    }

    #endregion

    #region Pause Methods

    public void PauseMusic()
    {
        musicSource.Pause();
    }

    public void PauseAmbiance()
    {
        ambianceSource.Pause();
    }

    public void PauseSoundEffect()
    {
        soundEffectSources.Pause();
    }

    #endregion

    #region UnPause Methods

    public void UnPauseMusic()
    {
        musicSource.UnPause();
    }

    public void UnPauseAmbiance()
    {
        ambianceSource.UnPause();
    }

    public void UnPauseSoundEffect()
    {
        soundEffectSources.UnPause();
    }

    #endregion

    #region Volume Methods

    public void SetMusicVolume(float volume)
    {
        musicSource.volume = volume;
    }

    public void SetAmbianceVolume(float volume)
    {
        ambianceSource.volume = volume;
    }

    public void SetSoundEffectVolume(float volume)
    {
        soundEffectSources.volume = volume;
    }

    #endregion

    #region Mute Methods

    public void MuteMusic(bool toogle)
    {
        musicSource.mute = toogle;
    }

    public void MuteAmbiance(bool toogle)
    {
        ambianceSource.mute = toogle;
    }

    public void MuteSoundEffect(bool toogle)
    {
        soundEffectSources.mute = toogle;
    }

    #endregion

}
