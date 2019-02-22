using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Animation {
    public float angle;
    public float returnSpeedMultiply;
}

public class CannonController : MonoBehaviour {

    public float fireRate;
    public GameObject shot;
    public float delay;
    public Transform shotSpawn;

    private AudioSource audioSource;
    public Animation anim;


    private Transform cannon;
    private Quaternion targetRotation;
    private Quaternion defaultRotation;

    void Start() {
		//ustawienie glosnosci oglosow wystrzalu
        audioSource = GetComponent<AudioSource>();
        float vol = (PlayerPrefs.GetFloat("CannonShotsVolume") + 0.0001f) / 50.0f;
        audioSource.volume = audioSource.volume * vol;

        //pobranie jednego mesha z calego dziala
        cannon = transform.GetChild(0).gameObject.transform;

        //pobranie domyslnej rotacji
        defaultRotation = cannon.localRotation;

		//uruchomienie strzelania w petli
        InvokeRepeating("Fire", delay, fireRate);
    }

    void Update() {
        //powrot do poczatkowej pozycji - opad lufy po wystrzale
        if (cannon.localRotation.x < defaultRotation.x) {
            targetRotation = Quaternion.AngleAxis(anim.angle * Time.deltaTime * anim.returnSpeedMultiply, Vector3.right);
            cannon.localRotation = cannon.localRotation * targetRotation;
        }
    }

    void Fire() {
        Instantiate(shot, shotSpawn.position, shotSpawn.rotation);
        audioSource.Play();

        //przechyl dziala po strzale - odrzut
        targetRotation = Quaternion.AngleAxis(-anim.angle, Vector3.right);
        cannon.localRotation = cannon.localRotation * targetRotation;
    }
}
