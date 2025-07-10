using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BGMType
{
    None, //なし
    Title, //タイトル
    InGame, //ゲーム中
    InBoss //ボス戦
}

public enum SEType
{
    GameClear, //ゲームクリア
    GameOver, //ゲームオーバー
}


public class SoundController : MonoBehaviour
{
    public AudioClip bgmInTitle;
    public AudioClip bgmInGame;
    public AudioClip bgmInBoss;

    public AudioClip seGameClear;
    public AudioClip seGameOver;

    public static SoundController soundController;
    public static BGMType playingBGM = BGMType.None;

    public float fadeTime = 2.0f;
    void Awake()
    {
        if (soundController == null)
        {
            soundController = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }


    }
    // Start is called before the first frame update
    void Start()
    {
        PlayBgm(BGMType.InGame);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayBgm(BGMType type)
    {
        if (type != playingBGM)
        {
            playingBGM = type;
            AudioSource audio = GetComponent<AudioSource>();

            if (type == BGMType.Title)
            {
                audio.clip = bgmInTitle;
            }
            else if (type == BGMType.InGame)
            {
                audio.clip = bgmInGame;
            }
            else if (type == BGMType.InBoss)
            {
                audio.clip = bgmInBoss;
            }
            //audio.Play();
            StartCoroutine(audio.PlayWithFadeIn(audio.clip));
        }

    }

    public void StopBgm()
    {
        AudioSource audio = GetComponent<AudioSource>();
        StartCoroutine(audio.StopWithFadeOut(fadeTime));

        //GetComponent<AudioSource>().Stop();
        playingBGM = BGMType.None;
    }

    public void SEPlay(SEType type)
    {
        if (type == SEType.GameClear)
        {
            GetComponent<AudioSource>().PlayOneShot(seGameClear);
        }
        else if (type == SEType.GameOver)
        {
            GetComponent<AudioSource>().PlayOneShot(seGameOver);
        }
    }





    }
