using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FadeOutTextChildren : MonoBehaviour {

    List<TextMeshPro> texts = new List<TextMeshPro> ();


    private void Start() {
        foreach (TextMeshPro t in GetComponentsInChildren<TextMeshPro>()) {
            texts.Add(t);
        }
        StartCoroutine(FadeOut());
    }


    IEnumerator FadeOut() {

        yield return new WaitForSeconds(5f);

        while (texts[0].color.a > 0) {

            foreach(TextMeshPro t in texts) {
                var c = t.color;
                c.a -= Time.deltaTime * .05f;
                t.color = c;
            }

            yield return null;
        }


    }

}
