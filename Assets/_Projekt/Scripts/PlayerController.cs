using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]


public class PlayerController : MonoBehaviour {

    public float smoothingMovement = 20.0f;
    public float pushForce = 3.0f;
    public float turnSpeed = 5.0f;
    public float inputDelay = 0.1f;
    public AudioClip clip_steps;


    private Animator anim;
    private Rigidbody rb;
    private GameController gameController;
    private AudioSource audioSource;
    private Camera cam;


    private float h;
    private float v;


    private bool enableChangeDir;


    void Start() {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();

        enableChangeDir = true;

        //szukanie GameControllera
        GameObject gameControllerObject = GameObject.FindWithTag("GameController");
        if (gameControllerObject != null) {
            gameController = gameControllerObject.GetComponent<GameController>();
        }
        if (gameController == null) {
            Debug.Log("Nie znaleziono GameController");
        }

		//zmiana glosnosci efektow dzwiekowych
        audioSource = GetComponent<AudioSource>();
        float vol = (PlayerPrefs.GetFloat("SoundsEffectsVolume") + 0.0001f) / 50.0f;
        audioSource.volume = audioSource.volume * vol;
    }


    void FixedUpdate(){
        h = Input.GetAxis("Horizontal");
        v = Input.GetAxis("Vertical");

        //dopoki nie zabilo postaci to moze sie poruszac
        if (!gameController.getGameOver()) {
            Move();
        }
    }


    void Update() {
        //jesli postac spadnie to ginie
        if (transform.position.y < -3.0f) {
            gameController.GameOver();
            transform.position = new Vector3(rb.position.x, -3.0f, rb.position.z);
        }

        Run();
    }


    void OnControllerColliderHit(ControllerColliderHit hit) {
        Rigidbody colRb = hit.collider.attachedRigidbody;

        if (hit.gameObject.CompareTag("Crate")) {
            //gdy skrzynia zacznie opadac to postac juz jej nie pcha
            if (colRb.position.y < 0.86f) { }
            else {
                //odblokowuje ruch tylko w jednym kierunku, aby moc poruszyc skrzynie
                if (pushingDirection(colRb)) {
                    //pcha skrzynie
                    Vector3 pushDir = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z);
                    colRb.velocity = pushDir * pushForce;
                }
            }
        }

		//gy dojdziemy do mety do konczymy poziom
        if (hit.gameObject.CompareTag("Finish")) {
            gameController.LevelEnded();
        }		
    }


    //funkcja sprawdza z ktorej strony stoi player, aby okreslic w ktora strone moze pchac skrzynie
    //uniemozliwia pchanie skrzyni w innym kierunku niz w kierunku poruszania sie
    //zwraca false gdy mielibysmy pchac skrzynie mala czescia postaci
    bool pushingDirection(Rigidbody colRb) {
        //pobieram jeden rozmiar, ponieważ to jest kwadrat i kazdy wymiar ma taki sam
        float colliderSize = colRb.gameObject.GetComponent<Transform>().localScale.x;

        if (transform.position.z > colRb.position.z - colliderSize / 2 && transform.position.z < colRb.position.z + colliderSize / 2) {
            colRb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionZ;
            return true;
        }
        else if (transform.position.x > colRb.position.x - colliderSize / 2 && transform.position.x < colRb.position.x + colliderSize / 2) {
            colRb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionX;
            return true;
        }
        else {
            //gdy jestesmy na rogu skrzyni to sie zeslizgujemy
            //mozna dodac rozmiar collidera playera/2 z obu stron, aby pchac skrzynie na calej dlugosci
            //chociaz obecne rozwiazanie umozliwia pchanie tylko jednej skrzyni
            //poprawienie tego pozoli pchac dwie skrzyni jesli staniemy w przewie miedzy nimi
            //Debug.Log("Warning PlayerController -> pushingDirections");	
            return false;
        }
    }


    //funkcja odpowiadajaca za poruszanie sie gracza oraz za reakcje na klawisze sterujace
	//zmienia sterowanie w zalezosci od tego czy mamy kamere izometryczna czy z 3 osoby
    void Move() {
        //obsluga sterowania WSAD lub strzalek
        if (Mathf.Abs(h) > inputDelay || Mathf.Abs(v) > inputDelay) {
            //kamera trzecioosobowa
            cam = gameObject.GetComponentInChildren<Camera>();
            if (cam != null && cam.isActiveAndEnabled) {
                thirdPersonController(h, v);
            }
            else {
				//kamera izometryczna
                isometricController(h, v);
            }

			//uruchomienie animacji chodu
            anim.SetBool("Walk", true);
            audioPlay(audioSource, clip_steps);
        }
        else {
			//zatrzymanie animacji
            anim.SetBool("Walk", false);
            anim.SetBool("Run", false);
            audioStop(audioSource);
        }
    }


	//funkcja obslugujaca bieg
    void Run() {
        //obsluga biegania
        if (Input.GetKeyDown(KeyCode.LeftShift)) {
            anim.SetBool("Run", true);
            audioStop(audioSource);
            audioPlay(audioSource, clip_steps, 1.15f);
        }
        if (Input.GetKeyUp(KeyCode.LeftShift)) {
            anim.SetBool("Run", false);
            audioStop(audioSource);
        }
    }


    void audioPlay(AudioSource Source, AudioClip audioClip, float speed = 1.0f) {
        if (!Source.isPlaying) {
            Source.clip = audioClip;
            Source.pitch = speed;
            Source.Play();
        }
    }

    void audioStop(AudioSource Source) {
        Source.Stop();
    }


    //sterowanie postacia w widoku z gory - izometrycznym
    void isometricController(float horizontal, float vertical) {
        Vector3 targetDirection = Vector3.zero;

        //ograniczenie ruchu tylko do 4 kierunkow (1 na raz)
        if (Mathf.Abs(horizontal) > Mathf.Abs(vertical)) {
            targetDirection = new Vector3(horizontal, 0.0f, 0.0f);
        }
        else if (Mathf.Abs(horizontal) < Mathf.Abs(vertical)) {
            targetDirection = new Vector3(0.0f, 0.0f, vertical);
        }

        //ruch w 8 kierunkach (krzyz + po skosie)
        //targetDirection = new Vector3(horizontal, 0.0f, vertical);

        //obrot
        if (!targetDirection.Equals(Vector3.zero)) {
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection, Vector3.up);
            Quaternion newRotation = Quaternion.Lerp(rb.rotation, targetRotation, smoothingMovement * Time.deltaTime);
            rb.MoveRotation(newRotation);
        }
    }


    //sterowanie postacia w widoku trzecioosobowym
    void thirdPersonController(float horizontal, float vertical) {
        Quaternion targetRotation = Quaternion.identity;

        //gdy idziemy prosto
        if (vertical > 0) rb.velocity = transform.forward * vertical;

        //gdy zawracamy
        else if (vertical < 0 && enableChangeDir) {
            targetRotation = Quaternion.AngleAxis(180.0f, Vector3.up);

            transform.rotation = transform.rotation * targetRotation;
            StartCoroutine(delayChangeDir(1.0f));
        }

        //obrot
        targetRotation = Quaternion.AngleAxis(turnSpeed * horizontal, Vector3.up);
        transform.rotation = transform.rotation * targetRotation;
    }


    //opoznia wykonanie kolejnego obrotu o 180 stopni w widoku trzecioosobowym
    IEnumerator delayChangeDir(float delay) {
        enableChangeDir = false;
        yield return new WaitForSeconds(delay);
        enableChangeDir = true;
    }
}