using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    public SoundEffectsSO SoundEffects;
    public MusicObjectSO MusicObject;
    public AudioSource MusicSource;
    public AudioSource SoundSource;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        MusicSource = gameObject.AddComponent<AudioSource>();
        SoundSource = gameObject.AddComponent<AudioSource>();
    }

    public void PlayMusic(AudioClip music)
    {
        MusicSource.clip = music;
        MusicSource.Play();
        StartCoroutine(RestartLoop(music.length));
    }

    private IEnumerator RestartLoop(float delay = 0)
    {
        yield return new WaitForSeconds(delay);
        LoopOverMusicList(MusicObject.MusicList);
    }

    public void PlaySoundEffectByIndex(int soundEffectIndex)
    {
        if (soundEffectIndex < 0 || soundEffectIndex >= SoundEffects.soundEffectList.Length)
        {
            Debug.LogError("Sound effect index out of range.");
            return;
        }
        SoundSource.PlayOneShot(SoundEffects.soundEffectList[soundEffectIndex]);
    }

    public void LoopOverMusicList(AudioClip[] musicList)
    {
        StartCoroutine(LoopMusicList(ShuffleMusicList(musicList)));
    }

    private AudioClip[] ShuffleMusicList(AudioClip[] musicList)
    {
        System.Random rng = new System.Random();
        List<AudioClip> shuffledList = new List<AudioClip>(musicList);
        int n = shuffledList.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            AudioClip value = shuffledList[k];
            shuffledList[k] = shuffledList[n];
            shuffledList[n] = value;
        }
        return shuffledList.ToArray();
    }

    private IEnumerator LoopMusicList(AudioClip[] musicList)
    {
        while (true)
        {
            foreach (AudioClip music in musicList)
            {
                PlayMusic(music);
                yield return new WaitForSeconds(music.length);
            }
        }
    }

    public void Start()
    {
        MusicSource.playOnAwake = false;
        SoundSource.playOnAwake = false;
        LoopOverMusicList(MusicObject.MusicList);
        MusicSource.volume = Mathf.Pow(PlayerPrefs.GetFloat("MusicVolume", 0.5f), 2);
        SoundSource.volume = Mathf.Pow(PlayerPrefs.GetFloat("SoundVolume", 0.5f), 2);
    }
}
