using UnityEngine;
using UnityEngine.Audio;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class MusicPlayer : MonoBehaviour {

    //public AudioClip gameoverSongIntro;
    public AudioClip levelSongIntro;
    public AudioClip gameoverIntro;
    public AudioClip[] songs;

    AudioSource audio;

    void Start () {
        audio = GetComponent<AudioSource>();

        if(levelSongIntro != null) {
            PlayIntro();
        }
        else { 
            PlayAudio();
        }
    }

    void PlayIntro() {
        audio.clip = levelSongIntro;
        audio.Play();
        StartCoroutine("RealSecondsInvoke", audio.clip.length);
        //Debug.Log("Played Intro");
    }

    void PlayAudio() {
        audio.clip = songs[0];
        audio.loop = true;
        audio.Play();
    }

    public void PlayGameOverIntro() {
        StopCoroutine("RealSecondsInvoke");
        audio.clip = gameoverIntro;
        audio.Play();
        StartCoroutine("RealSecondsInvoke", audio.clip.length);
    }

    void GameOver() {
        audio.clip = songs[1];
        audio.loop = true;
        audio.Play();
    }

    private IEnumerator RealSecondsInvoke(float time)
    {
        yield return StartCoroutine(ManagerScript.WaitForRealSeconds(time));

        if(ManagerScript.gameOver) {
            GameOver();
        }
        else {
            PlayAudio();
        }
    }
}
