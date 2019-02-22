using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class MenuController : MonoBehaviour {

    private GameObject Main;
    private GameObject Levels;
    private GameObject Options;
    private GameObject About;
    private Animator anim;


    void Awake() {
        Main = GameObject.FindWithTag("Menu_Main");
        Levels = GameObject.FindWithTag("Menu_Levels");
        Options = GameObject.FindWithTag("Menu_Options");
        About = GameObject.FindWithTag("Menu_About");

        anim = GameObject.FindWithTag("Player").GetComponent<Animator>();

        //inicjalizacja domyslnych ustawien w opcjach
        //jesli TileLevels = 0, to oznacza ze nie ma zadnych zmiennych jeszcze zapisanych
        //w tym przypadku ladujemy zmienne domyslne
        if (PlayerPrefs.GetInt("TileLevels") != 0) {
            GameObject temp = GameObject.FindWithTag("Menu_Opt_Music");
            temp.GetComponent<Slider>().value = PlayerPrefs.GetFloat("MusicVolume");

            temp = GameObject.FindWithTag("Menu_Opt_SoundsEffects");
            temp.GetComponent<Slider>().value = PlayerPrefs.GetFloat("SoundsEffectsVolume");

            temp = GameObject.FindWithTag("Menu_Opt_CannonShots");
            temp.GetComponent<Slider>().value = PlayerPrefs.GetFloat("CannonShotsVolume");

            temp = GameObject.FindWithTag("Menu_Opt_TileLevels");
            temp.GetComponent<Slider>().value = PlayerPrefs.GetInt("TileLevels");
        }

        //ukrycie kart
        Levels.SetActive(false);
        Options.SetActive(false);
        About.SetActive(false);

		//co jakis czas uruchomi sie animacja dodatkowa
        InvokeRepeating("booringAnimation", 20.0f, Random.Range(20.0f, 40.0f));
    }

    /*----- zmiana karty -----*/
    public void newGame(string name) {
        SceneManager.LoadScene(name);
    }

    public void levels()  {
        Main.SetActive(false);
        Levels.SetActive(true);
    }

    public void options() {
        Main.SetActive(false);
        Options.SetActive(true);
    }

    public void about() {
        Main.SetActive(false);
        About.SetActive(true);
    }

    public void exit() {
        Application.Quit();
    }

    public void back() {
        if (About.activeSelf) {
            About.SetActive(false);
        }
        else if (Levels.activeSelf) {
            Levels.SetActive(false);
        }
        else if (Options.activeSelf) {
            Options.SetActive(false);
        }
        else {
            Debug.LogError("Nie moge znalesc obiektu");
        }

        Main.SetActive(true);
    }

    /*----- levels -----*/
    public void loadLevel(string level) {
        SceneManager.LoadScene(level);
    }

    /*----- options -----*/
    public void updateSlider(Slider slider) {
        //pobranie indexu tego slidera
        int sliderIndex = slider.transform.GetSiblingIndex();

        //label tego slidera ma index i jeden mniejszy
        Transform label = slider.transform.parent.GetChild(sliderIndex - 1);

        //zmiana nazwy i dodanie wartosci
        string labelText = (label.GetComponent<Text>().text.Split('-'))[0];
        label.GetComponent<Text>().text = labelText + "- " + slider.value;

        //zmiana zmiennej w PlayerPrefs
        switch (slider.name) {
            case "SliderMusic":
                GameObject.FindWithTag("Music").GetComponent<AudioSource>().volume = (slider.value + 0.0001f) / 200.0f;
                PlayerPrefs.SetFloat("MusicVolume", slider.value);
                break;
            case "SliderSoundsEffects":
                PlayerPrefs.SetFloat("SoundsEffectsVolume", slider.value);
                break;
            case "SliderCannonShots":
                PlayerPrefs.SetFloat("CannonShotsVolume", slider.value);
                break;
            case "SliderTileLevels":
                PlayerPrefs.SetInt("TileLevels", (int)slider.value);
                break;
        }
    }

    /*----- animacje -----*/
    public void playAnimation(int index) {
		//uruchomienie animacji o zadanym indeksie
        anim.SetInteger("animNumber", index);

        if (index == 1 || index == 3 || index == 4)
            StartCoroutine(endAnimationLoop(0.5f));
    }

    //zmienia zmienna w animacji, aby ta sie nie zapetlala, tylko wykonala sie 1 raz
    IEnumerator endAnimationLoop(float delay) {
        yield return new WaitForSeconds(delay);
        anim.SetInteger("animNumber", 0);
    }

    //uruchamia dodatkowa animacje gdy odpowiednio dlugo sie poczeka
    void booringAnimation() {
        anim.SetInteger("animNumber", 5);
        StartCoroutine(endAnimationLoop(0.5f));
    }
}