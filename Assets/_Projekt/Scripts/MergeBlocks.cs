using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MergeBlocks : MonoBehaviour {

    void Start() {
        Merge();
    }

	//funkcja laczaca pojedyncze bloki i na ich podstawie tworzy jeden mesh
	//optymalizacja
    void Merge() {
        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();

        CombineInstance[] combine = new CombineInstance[meshFilters.Length];
        int i;

        for (i = 0; i < meshFilters.Length; i++) {
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;

            //niszcze bloki poza pierwszym ktory bedzie corem
            if (i > 0) Destroy(meshFilters[i].gameObject);
        }

        //polacznie blokow
        transform.GetChild(0).GetComponent<MeshFilter>().mesh = new Mesh();
        transform.GetChild(0).GetComponent<MeshFilter>().mesh.CombineMeshes(combine);

        //polaczenie colliderow
        transform.GetChild(0).gameObject.AddComponent<MeshCollider>();
        transform.GetChild(0).GetComponent<MeshCollider>().sharedMesh.CombineMeshes(combine);
        Destroy(transform.GetChild(0).GetComponent<BoxCollider>());

        //reset pozycji, rotacji i scali
        transform.GetChild(0).gameObject.transform.position = Vector3.zero;
        transform.GetChild(0).gameObject.transform.rotation = Quaternion.identity;
        transform.GetChild(0).gameObject.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
    }
}