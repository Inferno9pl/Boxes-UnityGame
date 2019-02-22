using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mover : MonoBehaviour {
    public float speed;
    public float lifeTime;
    private GameController gameController;

    void Start() {
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.velocity = transform.forward * speed;

        //ustawienie samodetrukcji po pewnym czasie
        Destroy(gameObject, lifeTime);

        //szukanie GameControllera
        GameObject gameControllerObject = GameObject.FindWithTag("GameController");

        if (gameControllerObject != null){
            gameController = gameControllerObject.GetComponent<GameController>();
        }
        if (gameController == null) {
            Debug.Log("Nie znaleziono GameController");
        }
    }


    void OnTriggerEnter(Collider collider) {
        //jesli uderzy w sciane to kula sie niszczy
        if (collider.CompareTag("Wall")) {
            Destroy(gameObject);
        }

        //jesli uderzy w skrzynie to tez niszczy
        if (collider.CompareTag("Crate")) {
            Destroy(gameObject);
        }

        //jesli uderzy w playera to go zabija
        if (collider.CompareTag("Player")) {
            Destroy(gameObject);
            gameController.GameOver();
        }
    }
}