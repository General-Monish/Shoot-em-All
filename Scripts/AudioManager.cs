using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
   float masterVolPercent=1;
    float sfxVolPercent=1;
    float musicVolpercent=1;

    AudioSource[] musicSources;
    int activeMusicSourceIndex;

    Transform audioListner;
    Transform playert;
    public static AudioManager instance;
    private void Awake()
    {
       
            instance = this;
        
        musicSources = new AudioSource[2];
        for(int i = 0; i < 2; i++)
        {
            GameObject newMusicSource = new GameObject("Music Source" + (i + 1));
            musicSources[i] = newMusicSource.AddComponent<AudioSource>();
            newMusicSource.transform.parent = transform;
        }
        audioListner = FindObjectOfType<AudioListener>().transform;
        playert = FindObjectOfType<Player>().transform;
    }

    private void Update()
    {
        if (playert != null)
        {
            audioListner.position = playert.position;
        }
    }

    public void PlayMusic(AudioClip clip,float fadeDuration = 1) 
    {
        activeMusicSourceIndex = 1 - activeMusicSourceIndex;
        musicSources[activeMusicSourceIndex].clip = clip;
        musicSources[activeMusicSourceIndex].Play();
        StartCoroutine(AnimateMusicCrossFire(fadeDuration));
    }
    public void PlaySound(AudioClip clip , Vector3 position)
    {
        if (clip != null)
        {
            AudioSource.PlayClipAtPoint(clip, position, sfxVolPercent * masterVolPercent);
        }
        
    }

    public void PlaySound(string soundname , Vector3 position)
    {

    }

    IEnumerator AnimateMusicCrossFire(float duration)
    {
        float percent = 0;
        while (percent < 1)
        {
            percent += Time.deltaTime * 1 / duration;
            musicSources[activeMusicSourceIndex].volume = Mathf.Lerp(0, musicVolpercent * masterVolPercent, percent);
            musicSources[1 - activeMusicSourceIndex].volume = Mathf.Lerp(musicVolpercent * masterVolPercent, 0, percent);
            yield return null;
        }
    }
}
