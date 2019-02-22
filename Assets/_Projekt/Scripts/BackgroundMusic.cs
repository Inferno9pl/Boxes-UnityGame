using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundMusic : MonoBehaviour{
    private AudioSource audioSource;
    private GameObject[] musicPlayers;
    private bool startMusic = true;
    void Awake() {
        //szukamy czy juz znajduje sie taki obiekt
        musicPlayers = GameObject.FindGameObjectsWithTag("Music");

        for (int i = 0; i < musicPlayers.Length; i++) {
            if (musicPlayers[i].GetComponent<AudioSource>().isPlaying) startMusic = false;
        }


        if (startMusic) {
            audioSource = GetComponent<AudioSource>();
            DontDestroyOnLoad(transform.gameObject);
            audioSource.ignoreListenerPause = true;
            audioSource.Play();
        }
    }
}
