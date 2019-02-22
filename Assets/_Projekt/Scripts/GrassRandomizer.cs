using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassRandomizer : MonoBehaviour {

    public GameObject grass;
    public int density;
    public float size = 1.0f;

    private int maxGrass = 5000; //maksymana liczba trawy

    void Start() {
        float rotation;
        float scale;
        Vector3 targetPosition;
        Quaternion targetRotation;
        Vector3 up;
        RaycastHit hit;

        //stworzenie grupy gdzie bedziemy zapisywac trawe
        GameObject grassGroup = new GameObject();
        grassGroup.transform.parent = transform.parent;
        grassGroup.name = "grassGroup";

        //pobieramy ilosc dzieci
        Transform child;
        int x = transform.childCount;

        //gdy trawy jest tak duzo ze wydajnosc znaczaco spadnie to zmniejszamy gestosc
        if (density * x > maxGrass) {
            density = maxGrass / x;
        }

        //dla kazdego dziecka generuje trawe
        for (int j = 0; j < x; j++) {
            child = transform.GetChild(j);

            //sprawdzam czy na tym bloku trawy nic nie stoi
            //jesli znajduje sie mur lub przycisk to nei tworze trawy
            up = child.TransformDirection(Vector3.up);
            if (Physics.Raycast(child.position, up, out hit, 4.0f) && (hit.transform.tag == "Wall" || hit.transform.tag == "Button"))
                continue;

			//tworzenie poszczegolnych elementow trawy
            for (int i = 0; i < density; i++) {
                //losowy rozmiar
                scale = Random.Range(0.6f, 1.0f);
                grass.transform.localScale = new Vector3(0.0001f, scale * size, scale * size);
   
                //losowa pozycja
                targetPosition = new Vector3(child.position.x + Random.Range(-0.95f, 0.95f), (scale * size) / 2, child.position.z + Random.Range(-0.95f, 0.95f));

                //losowy obrot
                rotation = Random.Range(0f, 90.0f);
                targetRotation = Quaternion.AngleAxis(rotation, Vector3.up);

                //dodanie trawy
                Instantiate(grass, targetPosition, targetRotation, grassGroup.transform);

                //obrot o 90 stopni i dodanie kolejnej trawy
                targetRotation = targetRotation * Quaternion.Euler(0, 90.0f, 0);
                Instantiate(grass, targetPosition, targetRotation, grassGroup.transform);
            }
        }
    }
}
