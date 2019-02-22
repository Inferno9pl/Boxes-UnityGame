using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxController : MonoBehaviour{

    public float minY;
    private Rigidbody rb;
    AudioSource audioSource;

    private GameController gameController;
    private float tileSize;
    private int tileLevels;

    void Start(){
        rb = GetComponent<Rigidbody>();

		//ustawiamy glosnosc efektow dzwiekowych
        audioSource = GetComponent<AudioSource>();
        float vol = (PlayerPrefs.GetFloat("SoundsEffectsVolume") + 0.0001f) / 50.0f;
        audioSource.volume = audioSource.volume * vol;

        GameObject gameControllerObject = GameObject.FindWithTag("GameController");
        if (gameControllerObject != null){
            gameController = gameControllerObject.GetComponent<GameController>();
            tileSize = gameController.getTileSize();
            tileLevels = gameController.getTileLevels();
        }
        if (gameController == null){
            Debug.Log("Nie znaleziono GameController");
        }
    }


    void FixedUpdate(){
        //gdy opada to juz porusza sie tylko w jednym kierunku (Y)
        if (rb.position.y < 0.86f){
            rb.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
        }

        //minimalna wartosc y na jaka moze opasc skrzynia
        if (rb.position.y < minY){
            rb.isKinematic = true;
            rb.position = new Vector3(rb.position.x, minY, rb.position.z);
        }

        //gdy sie skrzynia porusza to wydaje dzwiek
        if (rb.velocity.x != 0 || rb.velocity.z != 0){
            if (!audioSource.isPlaying) audioSource.Play();
        }
        else audioSource.Stop();

        //gdy skrzynia sie zatrzyma to uniemozliwiam jej ruch
        if (rb.velocity == Vector3.zero){
            rb.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
        }
    }


    void LateUpdate(){
        //poprawiam pozycje skrzyni wg ustalonej siatki
        if (rb.velocity == Vector3.zero){
            //poprawienie pozycji
            Vector3 pos = new Vector3(correctPosition(rb.position.x), rb.position.y, correctPosition(rb.position.z));
            transform.position = pos;
        }
    }


    //funkcja ktora przesuwa obiekt do najblizszej poprawnej pozycji.
    //poprawnej, czyli takiej ze na jednym kaflu moze byc w tileLevels^2 pozycji, a nie w dowolnym polozeniu
    //ma to na celu ulatwienie manewrowania skrzynia
    float correctPosition(float x){
        float result;
        float step = tileSize / tileLevels;

        float temp = x / step;
        temp = Mathf.Round(temp);
        result = temp * step;

        return result;
    }
}
