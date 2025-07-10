using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlayerSEPlay : MonoBehaviour
{
    public AudioSource audioSource;
    public List<AudioClip> audioClipList = new List<AudioClip>();
    public float seVol = 0.5f;
    public float range;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void PlaySePitchRdm(string audioClipName)
    {
        AudioClip audioClip =
            audioClipList.FirstOrDefault(clip => clip.name == audioClipName);

        if (audioClip != null)
        {
            audioSource.volume = seVol;
            audioSource.pitch = Random.Range(1f - range, 1f + range);
            audioSource.Play(audioClip);
        }

    }

    public void PlaySe(string audioClipName)
    {
        AudioClip audioClip =
            audioClipList.FirstOrDefault(clip => clip.name == audioClipName);
        if (audioClip != null)
        {
            audioSource.volume = seVol;
            audioSource.pitch = 1f;
            audioSource.Play(audioClip);
        }



    }


}