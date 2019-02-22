using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour {

    public GameObject player;

    private float tileSize = 2.0f;
    private int tileLevels;
    private bool gameOver;

    private GameObject PauseMenu;
    private GameObject DeadOrAlive;
    private Camera[] cameras;
    private AudioSource audioSource;
    int cameraIndex;

    void Awake() {
        gameOver = false;

        //ustawienie glosnosci muzyki
        audioSource = GetComponent<AudioSource>();
        float vol = (PlayerPrefs.GetFloat("MusicVolume") + 0.0001f) / 50.0f;
        audioSource.volume = audioSource.volume * vol;

        //inicjalizacja menu
        PauseMenu = GameObject.FindWithTag("Menu_Main");

        //inicjalizacja tekstow na zakonczenie poziomu lub smierci postaci
        DeadOrAlive = GameObject.FindWithTag("DeadOrAlive");

        //dodatkowo ustawiam aby guziki w menu ignorowaly wyciszenie dzwieku
        Component[] temp;
        temp = PauseMenu.GetComponentsInChildren<AudioSource>();
        foreach (AudioSource aud in temp)
            aud.ignoreListenerPause = true;

		//ukrycie GUI
        PauseMenu.SetActive(false);
        DeadOrAlive.SetActive(false);

        //inicjalizacja zmiennych do kamery
        cameraIndex = 0;
        cameras = Camera.allCameras;

        //ustawiam jako aktywna tylko jedna kamere, a reszte dezaktywuje
        cameras[0].enabled = true;
        cameras[0].targetDisplay = 0;
        for (int i = 1; i < cameras.Length; i++) {
            cameras[i].enabled = false;
        }

        tileLevels = PlayerPrefs.GetInt("TileLevels");
    }

    void Update() {
        changeCamera();
        restart();
        pause();
    }

    //restart gry pod klawiszem R
    void restart() {
        if (Input.GetKeyDown(KeyCode.R)) {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            if (Time.timeScale == 0) Time.timeScale = 1;
        }
    }

	//pauza uruchamiana pod ESC
    void pause() {
        //gdy damy ESC to zatrzymujemy gre i wyswietlamy menu
        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (Time.timeScale == 0) {
                menuResume();
            }
            else {
                //ponowny escate wylacza pauze gry
                Time.timeScale = 0;
                PauseMenu.SetActive(true);
                AudioListener.pause = true;
            }
        }
    }


    /*----- buttony w menu -----*/
    public void menuRestart() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        menuResume();
    }

    public void menuResume() {
        PauseMenu.SetActive(false);
        Time.timeScale = 1;
        AudioListener.pause = false;
    }

    public void menuMenu() {
        SceneManager.LoadScene("MainMenu");
        Time.timeScale = 1;
        AudioListener.pause = false;
    }


    //obsluga zmiany kamery pod klawiszem C
    void changeCamera() {
        if (Input.GetKeyDown(KeyCode.C)) {
            //dezaktywuje obecna kamere
            cameras[cameraIndex].enabled = false;

            //ustawiam indeks kamery ktora chce uzywac
            cameraIndex = (cameraIndex + 1) % cameras.Length;

            //aktywuje kamere o tym indeksie
            cameras[cameraIndex].targetDisplay = 0;
            cameras[cameraIndex].enabled = true;
        }
    }


    //obsluga porazki
    public void GameOver() {
        gameOver = true;
        Debug.Log("Porażka");

        player.GetComponent<Animator>().SetBool("Die", true);
        player.GetComponent<AudioSource>().Stop();

        Time.timeScale = 0;

        //komunikat porazki
        DeadOrAlive.SetActive(true);
        //ustawienie pierwszego komunikatu
        DeadOrAlive.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = "You're dead! Try again";
        //ustawienie drugiego komunikatu
        DeadOrAlive.transform.GetChild(0).GetChild(1).GetComponent<Text>().text = "Press R to restart";
    }


    //obsluga sukcesu
    public void LevelEnded() {
        //komunikat zwyciestwa
        DeadOrAlive.SetActive(true);
        //ustawienie pierwszego komunikatu
        DeadOrAlive.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = "Congratulations!";
        //ustawienie drugiego komunikatu
        DeadOrAlive.transform.GetChild(0).GetChild(1).GetComponent<Text>().text = "Next level will be loaded soon";

		//zatrzymanie animacji i dzwieku
        player.GetComponent<AudioSource>().Stop();
        player.GetComponent<Animator>().SetBool("Walk", false);
        gameOver = true;

		//zaladowanie poziomu po chwili opoznienia
        StartCoroutine(delayLoadLevel());
    }


    //opoznienie zaladowania kolejnego poziomu
    IEnumerator delayLoadLevel() {
        yield return new WaitForSeconds(2.0f);
        if (SceneManager.GetActiveScene().buildIndex == 10) SceneManager.LoadScene("MainMenu");
        else SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public bool getGameOver() {
        return gameOver;
    }

    public int getTileLevels() {
        return tileLevels;
    }

    public float getTileSize() {
        return tileSize;
    }
}