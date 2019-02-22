using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateController : MonoBehaviour {

    public float buttonSpeedMultiplier = 1.0f;
    public float gateSpeedMultiplier = 1.0f;

    public GameObject Gate;
    private float defaultGateYPosition;
    private AudioSource gateAudio;
    private AudioSource buttonAudio;

    private GameObject Button;
    private float defaultButtonYPosition;

    int gateMoving; // 1 - porusza sie w gore, 0 - nie porusza sie, -1 - porusza sie w dol
    int buttonMoving;   // 1 - porusza sie w gore, 0 - nie porusza sie, -1 - porusza sie w dol

    private float buttonMinY = -0.064f;
    private float gateMinY = -0.75f;

    void Start() {
		//szukam przycisku
        Button = transform.GetChild(0).gameObject;

        //jesli pobrany obiekt nie jest buttonem to szukam dalej
        if (!(Button.name.Equals("Button"))) {
            Button = transform.GetChild(1).gameObject;
        }

		//zapisuje domyslne wartosci do ktorych beda powracac przycisk i brama
        defaultButtonYPosition = Button.transform.position.y;
        defaultGateYPosition = Gate.transform.position.y;

		//ustawienie glosnosci
        gateAudio = Gate.GetComponent<AudioSource>();
        buttonAudio = Button.GetComponent<AudioSource>();
        float vol = (PlayerPrefs.GetFloat("SoundsEffectsVolume") + 0.0001f) / 50.0f;
        gateAudio.volume = gateAudio.volume * vol;
        buttonAudio.volume = buttonAudio.volume * vol;

        gateMoving = 0;
        buttonMoving = 0;
    }

    void Update() {
        //jesli przycisk opada
        if (buttonMoving == -1) {
            buttonDown();
            audioPlay(buttonAudio);
        }

        //jesli brama opada
        if (gateMoving == -1) {
            gateDown();
            audioPlay(gateAudio);
        }

        //jesli przycisk podnosci sie
        if (buttonMoving == 1) {
            buttonUp();
            audioPlay(buttonAudio);
        }

        //jesli brama podnosci sie
        if (gateMoving == 1) {
            gateUp();
            audioPlay(gateAudio);
        }

		//brama nieruchoma
        if (gateMoving == 0) {
            audioStop(gateAudio);
        }

		//przycisk nieruchomy
        if (buttonMoving == 0) {
            audioStop(buttonAudio);
        }
    }


	//funkcja ktora opuszcza przycisk gdy go wciskamy
    void buttonDown() {
        if (Button.transform.position.y > buttonMinY) {
            float temp = 0.001f * buttonSpeedMultiplier;
            Button.transform.position = new Vector3(Button.transform.position.x, Button.transform.position.y - temp, Button.transform.position.z);
        }
        else buttonMoving = 0;
    }

	//funkcja ktora podnosci przycisk
    void buttonUp() {
        if (Button.transform.position.y < defaultButtonYPosition) {
            float temp = 0.001f * buttonSpeedMultiplier;
            Button.transform.position = new Vector3(Button.transform.position.x, Button.transform.position.y + temp, Button.transform.position.z);
        }
        else buttonMoving = 0;
    }

	//opuszczenie bramy
    void gateDown() {
        if (Gate.transform.position.y > gateMinY) {
            float temp = 0.001f * gateSpeedMultiplier;
            Gate.transform.position = new Vector3(Gate.transform.position.x, Gate.transform.position.y - temp, Gate.transform.position.z);
        }
        else gateMoving = 0;
    }

	//podnoszenie bramy
    void gateUp() {
        if (Gate.transform.position.y < defaultGateYPosition) {
            float temp = 0.001f * gateSpeedMultiplier;
            Gate.transform.position = new Vector3(Gate.transform.position.x, Gate.transform.position.y + temp, Gate.transform.position.z);
        }
        else gateMoving = 0;
    }

	//wykrywanie nacisniecia przycisku
    void OnTriggerEnter(Collider other){
        if (!other.gameObject.name.Equals("Cannonball(Clone)")) {
            gateMoving = -1;
            buttonMoving = -1;
        }
    }

	//wykrycie gdy przycisk zostaje zwolniony
    void OnTriggerExit(Collider other) {
        if (!other.gameObject.name.Equals("Cannonball(Clone)")) {
            gateMoving = 1;
            buttonMoving = 1;
        }
    }

	//otworzenie odglosow
    void audioPlay(AudioSource Source) {
        if (!Source.isPlaying) {
            Source.Play();
        }
    }

	//zatrzymanie audio
    void audioStop(AudioSource Source) {
        Source.Stop();
    }
}