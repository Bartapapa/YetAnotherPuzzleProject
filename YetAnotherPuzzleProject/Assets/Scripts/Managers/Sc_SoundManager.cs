using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class Sc_SoundManager : MonoBehaviour
{
    public static Sc_SoundManager instance { get; private set; }

    [Header("Mixers")]
    [SerializeField] AudioMixer MasterMixer;

    [Header("Parameters")]
    public float MasterVolume = 1f;
    public float MusicVolume = 1f;
    public float SFXVolume = 1f;

    [Header("Sounds")]
    public AudioSource MusicSource;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != null)
        {
            Destroy(gameObject);
        }
    }

    public void InitializePresets()
    {
        MusicSource = Camera.main.GetComponent<AudioSource>();

        SetMasterVolume(MasterVolume);
        SetMusicVolume(MusicVolume);
        SetSFXVolume(SFXVolume);
    }

    public void SetMasterVolume(float volume)
    {
        MasterVolume = volume;
        MasterMixer.SetFloat("masterVolume", Mathf.Log10(volume) * 20);
    }

    public void SetMusicVolume(float volume)
    {
        MusicVolume = volume;
        MasterMixer.SetFloat("musicVolume", Mathf.Log10(volume) * 20);
    }

    public void SetSFXVolume(float volume)
    {
        SFXVolume = volume;
        MasterMixer.SetFloat("sfxVolume", Mathf.Log10(volume) * 20);
    }

    public void PlaySFX(AudioSource sfxSource, AudioClip clip)
    {
        sfxSource.pitch = 1f;
        if (clip == null)
        {
            Debug.Log("Clip that needed to be played is nonexistent!");
        }
        sfxSource.PlayOneShot(clip);
    }

    public void PlaySFX(AudioSource sfxSource, AudioClip clip, Vector2 randomMinMaxPitch)
    {
        sfxSource.pitch = 1f;
        if (clip == null)
        {
            Debug.Log("Clip that needed to be played is nonexistent!");
        }

        float randomPitch = UnityEngine.Random.Range(randomMinMaxPitch.x, randomMinMaxPitch.y);
        sfxSource.pitch = randomPitch;

        sfxSource.PlayOneShot(clip);
    }

    public void PlayRandomSFX(AudioSource sfxSource, List<AudioClip> clips)
    {
        if (clips.Count <= 0)
        {
            Debug.Log("No clips in clip list sent!");
        }

        int randomIndex = Random.Range(0, clips.Count);
        AudioClip chosenClip = clips[randomIndex];

        PlaySFX(sfxSource, chosenClip);
    }

    public void PlayRandomSFX(AudioSource sfxSource, List<AudioClip> clips, Vector2 randomMinMaxPitch)
    {
        if (clips.Count <= 0)
        {
            Debug.Log("No clips in clip list sent!");
        }

        int randomIndex = Random.Range(0, clips.Count);
        AudioClip chosenClip = clips[randomIndex];

        PlaySFX(sfxSource, chosenClip, randomMinMaxPitch);
    }

    public void PlayMusic(AudioClip music)
    {
        if (music == null)
        {
            Debug.Log("Music that needed to be played is nonexistent!");
        }
        MusicSource.clip = music;
        MusicSource.loop = true;
        MusicSource.Play();
    }

    public void StopMusic()
    {
        MusicSource.Stop();
    }
}
