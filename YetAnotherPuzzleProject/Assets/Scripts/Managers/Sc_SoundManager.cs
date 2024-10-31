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
    public float UIVolume = 1f;

    [Header("Sounds")]
    public AudioSource MusicSource;
    public AudioSource UISource;

    [Header("ASO prefab")]
    public Sc_AudioSourceObject ASOprefab;

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
        AudioSource[] audioSources = Camera.main.GetComponents<AudioSource>();
        MusicSource = audioSources[0];
        UISource = audioSources[1];

        SetMasterVolume(MasterVolume);
        SetMusicVolume(MusicVolume);
        SetSFXVolume(SFXVolume);
        SetUIVolume(UIVolume);
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

    public void SetUIVolume(float volume)
    {
        UIVolume = volume;
        MasterMixer.SetFloat("uiVolume", Mathf.Log10(volume) * 20);
    }

    public void StopCos()
    {
        StopAllCoroutines();
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

    public void PlaySFX(AudioSource sfxSource, AudioClip clip, float volume)
    {
        sfxSource.pitch = 1f;
        sfxSource.volume = volume;
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

    public void PlaySFX(AudioSource sfxSource, AudioClip clip, float volume, Vector2 randomMinMaxPitch)
    {
        sfxSource.pitch = 1f;
        sfxSource.volume = volume;
        if (clip == null)
        {
            Debug.Log("Clip that needed to be played is nonexistent!");
        }

        float randomPitch = UnityEngine.Random.Range(randomMinMaxPitch.x, randomMinMaxPitch.y);
        sfxSource.pitch = randomPitch;

        sfxSource.PlayOneShot(clip);
    }

    public void PlayLoopingSFX(AudioSource sfxSource, AudioClip clip)
    {
        sfxSource.pitch = 1f;
        if (clip == null)
        {
            Debug.Log("Clip that needed to be played is nonexistent!");
        }

        sfxSource.clip = clip;
        sfxSource.loop = true;
        sfxSource.Play();
    }

    public void PlayLoopingSFX(AudioSource sfxSource, AudioClip clip, Vector2 randomMinMaxPitch)
    {
        sfxSource.pitch = 1f;
        if (clip == null)
        {
            Debug.Log("Clip that needed to be played is nonexistent!");
        }
        float randomPitch = UnityEngine.Random.Range(randomMinMaxPitch.x, randomMinMaxPitch.y);
        sfxSource.pitch = randomPitch;

        sfxSource.clip = clip;
        sfxSource.loop = true;
        sfxSource.Play();
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

    public void PlayRandomSFX(AudioSource sfxSource, List<AudioClip> clips, float volume)
    {
        if (clips.Count <= 0)
        {
            Debug.Log("No clips in clip list sent!");
        }

        int randomIndex = Random.Range(0, clips.Count);
        AudioClip chosenClip = clips[randomIndex];

        PlaySFX(sfxSource, chosenClip, volume);
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

    public void PlayRandomSFX(AudioSource sfxSource, List<AudioClip> clips, float volume, Vector2 randomMinMaxPitch)
    {
        if (clips.Count <= 0)
        {
            Debug.Log("No clips in clip list sent!");
        }

        int randomIndex = Random.Range(0, clips.Count);
        AudioClip chosenClip = clips[randomIndex];

        PlaySFX(sfxSource, chosenClip, volume, randomMinMaxPitch);
    }

    public void FadeOut(AudioSource source, float overTime)
    {
        Fade(source, overTime, source.volume, 0f);
    }

    public void FadeIn(AudioSource source, float overTime, float toVol)
    {
        Fade(source, overTime, 0f, toVol);
    }

    public void Fade(AudioSource source, float overTime, float fromVol, float toVol)
    {
        if (source == null) return;
        StartCoroutine(FadeCoroutine(source, overTime, fromVol, toVol));
    }

    private IEnumerator FadeCoroutine(AudioSource source, float overTime, float fromVol, float toVol)
    {
        float timer = 0f;
        source.volume = fromVol;

        while(timer < overTime)
        {
            timer += Time.deltaTime;
            float alpha = timer / overTime;
            float volume = Mathf.Lerp(fromVol, toVol, alpha);
            source.volume = volume;
            yield return null;
        }

        source.volume = toVol;
        if (source.volume <= 0f)
        {
            source.Stop();
        }
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

    public Sc_AudioSourceObject CreateAudioSourceObject(AudioClip clip, Vector3 position, float volume = 1f)
    {
        if (Sc_GameManager.instance.CurrentLevel == null) return null;

        Transform parent = Sc_GameManager.instance.CurrentLevel.transform;

        Sc_AudioSourceObject newASO = Instantiate<Sc_AudioSourceObject>(ASOprefab, position, Quaternion.identity, parent);
        float lifetime = clip.length;
        newASO.LifeSpan.Duration = lifetime;

        PlaySFX(newASO.Source, clip, volume);

        return newASO;
    }
}
