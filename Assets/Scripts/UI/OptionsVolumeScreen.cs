using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class OptionsVolumeScreen : MonoBehaviour
{
    [SerializeField] private int sensibility = 20;
    [SerializeField] private VolumeSlider[] volumeSliders;

    [Serializable]
    private struct VolumeSlider
    {
        public AudioMixerGroup volumeMixerGroup;
        public Slider volumeSlider;
        public string volumeName;
        public bool useVolumeMixerGroupName;
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        NameVolumeSliders();
    }

#endif

    private void Awake()
    {
        NameVolumeSliders();
    }

    private void OnEnable()
    {
        InitializeVolumeSliders();
    }

    private void OnDisable()
    {
        foreach (var volumeSlider in volumeSliders)
        {
            volumeSlider.volumeSlider.onValueChanged.RemoveAllListeners();
        }
    }

    private void NameVolumeSliders()
    {
        for (int i = 0; i < volumeSliders.Length; i++)
        {
            if (volumeSliders[i].volumeMixerGroup != null && volumeSliders[i].useVolumeMixerGroupName)
            {
                volumeSliders[i].volumeName = volumeSliders[i].volumeMixerGroup.name;
            }
        }
    }

    // Also loads the volume from the player prefs and sets the volume of the mixers
    private void InitializeVolumeSliders()
    {
        foreach (var volumeSlider in volumeSliders)
        {
            float volume = PlayerPrefs.GetFloat(volumeSlider.volumeName, 0.5f);
            volumeSlider.volumeSlider.value = volume;
            SaveSliderValue(volume, volumeSlider.volumeName, volumeSlider.volumeMixerGroup);
            volumeSlider.volumeSlider.onValueChanged.AddListener(
                (newValue) => SaveSliderValue(newValue, volumeSlider.volumeName, volumeSlider.volumeMixerGroup)
                );
        }
    }

    private void SaveSliderValue(float value, string volumeName, AudioMixerGroup mixer)
    {
        PlayerPrefs.SetFloat(volumeName, value);

        if (value <= 0)
        {
            value = -80;
        }
        else
        {
            value = Mathf.Log10(value) * sensibility;
        }

        mixer.audioMixer.SetFloat(volumeName, value);
    }
}
