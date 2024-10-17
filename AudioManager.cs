using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    private const string AUDIO_MIXER_PATH = "Audio/Main";
    private const string MUSIC_PATH = "Audio/Musics/";
    private const string SFX_PATH = "Audio/SFXs/";
    private const string AUDIO_MIXER_MUSIC = "Music";
    private const string AUDIO_MIXER_SFX = "Sfx";

    private const string MUSIC_GAMEOBJECT_NAME = "Music - [{0}]";  // {0} = Music channel
    public const string SFX_GAMEOBJECT_NAME = "SFX - [{0}]";      // {0} = SFX name
    
    private static Dictionary<int, GameObject> musicChannels = new Dictionary<int, GameObject>();
    private static List<GameObject> sfxPool = new List<GameObject>();
    private static AudioMixerGroup musicMixerGroup;
    private static AudioMixerGroup sfxMixerGroup;
    private static GameObject musicParent;
    private static GameObject sfxParent;
    
    public static AudioManager instance;

    private void Awake() {
        if(instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else {
            Destroy(gameObject);
        }
        
        InitializeAudioMixers();
    }

    private void InitializeAudioMixers() {
        AudioMixer mainMixer = Resources.Load<AudioMixer>(AUDIO_MIXER_PATH);
        
        
        musicMixerGroup = mainMixer.FindMatchingGroups(AUDIO_MIXER_MUSIC)[0];
        sfxMixerGroup = mainMixer.FindMatchingGroups(AUDIO_MIXER_SFX)[0];
        
        GameObject audioSourcesParent = new GameObject("Audio Sources");
        audioSourcesParent.transform.SetParent(transform);
        
        musicParent = new GameObject("Music");
        musicParent.transform.SetParent(audioSourcesParent.transform);
        
        sfxParent = new GameObject("SFX");
        sfxParent.transform.SetParent(audioSourcesParent.transform);
    }

    public static AudioSource PlaySfx(string sfxName, float volume = 1f, float pitch = 1f) {
        AudioClip clip = Resources.Load<AudioClip>(SFX_PATH + sfxName);
        if(!clip) {
            Debug.LogError($"SFX file '{sfxName}' not found at path '{SFX_PATH + sfxName}'");
            return null;
        }

        return PlaySfx(clip, volume, pitch);
    }

    public static AudioSource PlaySfx(AudioClip sfxClip, float volume = 1f, float pitch = 1f) {
        if(!sfxClip) {
            Debug.LogError("SFX clip is null!");
            return null;
        }

        (AudioSource, AutoReturnToPool) newSfxObject = GetSfxSourceFromPool();
        AudioSource source = newSfxObject.Item1;
        source.name = string.Format(SFX_GAMEOBJECT_NAME, sfxClip.name);
        source.outputAudioMixerGroup = sfxMixerGroup;
        source.clip = sfxClip;
        source.volume = volume;
        source.pitch = pitch;
        source.Play();
        newSfxObject.Item2.Initialize(source);

        return source;
    }

    public static void StopSfx(AudioSource source) {
        if(!source) {
            Debug.LogError("AudioSource is null!");
            return;
        }
        source.Stop();
        ReturnToSfxPool(source);
    }

    public static void PlayMusic(string musicName,int channel,  float volume = 1f, float pitch = 1f, bool loop = true, bool instant = false, float fadeDuration = 1f) {
        AudioClip clip = Resources.Load<AudioClip>(MUSIC_PATH + musicName);
        if(!clip) {
            Debug.LogError($"Music file '{musicName}' not found at path '{MUSIC_PATH + musicName}'");
            return;
        }
        
        PlayMusic(clip, channel, volume, pitch, loop, instant, fadeDuration);
    }

    public static void PlayMusic(AudioClip clip, int channel, float volume = 1f, float pitch = 1f, bool loop = true, bool instant = false, float fadeDuration = 1f) {
        if(!clip) {
            Debug.LogError("Music clip is null!");
            return;
        }

        if (musicChannels.TryGetValue(channel, out GameObject currentMusicChannel)) {
            AudioSource currentSource = currentMusicChannel.GetComponent<AudioSource>();
            
            if (!instant) {
                GameObject currentMusicObject = musicChannels[channel];
                currentSource = currentMusicObject.GetComponent<AudioSource>();

                GameObject newMusicObject = new GameObject("TempMusicChannel_" + channel);
                newMusicObject.transform.parent = musicParent.transform;
                AudioSource newSource = newMusicObject.AddComponent<AudioSource>();
                newSource.outputAudioMixerGroup = musicMixerGroup;
                newSource.loop = true;
                newSource.clip = clip;
                newSource.volume = 0f;
                newSource.Play();

                newSource.DOFade(volume, fadeDuration);
                currentSource.DOFade(0f, fadeDuration).OnComplete(() =>
                {
                    currentSource.Stop();
                    string currentObjectName = currentMusicObject.name;
                    GameObject.Destroy(currentMusicObject);
                    musicChannels[channel] = newMusicObject;
                    newMusicObject.name = currentObjectName;
                });
            }
            else {
                currentSource.Stop();
                currentSource.volume = volume;
                InitializeNewMusicSource(currentSource, clip, pitch, loop);
            }
        }
        else {
            GameObject newMusicChannel = new GameObject(string.Format(MUSIC_GAMEOBJECT_NAME, channel));
            newMusicChannel.transform.SetParent(musicParent.transform);
            AudioSource newSource = newMusicChannel.AddComponent<AudioSource>();
            newSource.outputAudioMixerGroup = musicMixerGroup;
            InitializeNewMusicSource(newSource, clip, pitch, loop);
            if (!instant) {
                newSource.volume = 0f;
                newSource.DOFade(volume, fadeDuration);
            }
            else {
                newSource.volume = volume;
            }
            musicChannels.Add(channel, newMusicChannel);
        }
    }
    
    public static void StopMusic(int channel, bool instant = false, float fadeDuration = 1f) {
        if (musicChannels.TryGetValue(channel, out GameObject currentMusicChannel)) {
            AudioSource currentSource = currentMusicChannel.GetComponent<AudioSource>();
            if (!instant) {
                currentSource.DOFade(0f, fadeDuration).OnComplete(() => {
                    currentSource.Stop();
                });
            }
            else {
                currentSource.Stop();
            }
        }
    }

    private static void InitializeNewMusicSource(AudioSource source, AudioClip clip , float pitch, bool loop) {
        source.clip = clip;
        source.pitch = pitch;
        source.loop = loop;
        source.Play();
    }

    private static (AudioSource, AutoReturnToPool) GetSfxSourceFromPool() {
        AudioSource source = sfxPool.FirstOrDefault(sfxObject => sfxObject && !sfxObject.GetComponent<AudioSource>().isPlaying)?.GetComponent<AudioSource>();
        if (!source) {
            GameObject newSfxObject = new GameObject();
            newSfxObject.transform.SetParent(sfxParent.transform);
            source = newSfxObject.AddComponent<AudioSource>();
            newSfxObject.AddComponent<AutoReturnToPool>();
            sfxPool.Add(newSfxObject);
        }
        return (source, source.GetComponent<AutoReturnToPool>());
    }
    
    private static void ReturnToSfxPool(AudioSource source) {
        GameObject sfxObject = source.gameObject;
        
        if(sfxPool.Contains(source.gameObject)) {
            if(source.isPlaying) source.Stop();

            source.clip = null;
            sfxObject.name = "SFX - [POOL]";
        }
    }
}
