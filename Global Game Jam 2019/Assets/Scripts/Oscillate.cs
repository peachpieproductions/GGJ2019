using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Oscillate : MonoBehaviour {

    public bool absoluteValue;
    public float startOffsetValue;
    public float generalSpeedMult = 1;
    [Header("Rotation")]
    public Vector3 rotate;
    public float rotSpeedMult = 1;
    [Header("Scale")]
    public Vector3 scale;
    public float scaleSpeedMult = 1;

    RectTransform rect;
    float osc;
    Vector3 startingRot;
    Vector3 startingScale;

    private void Awake() {
        rect = GetComponent<RectTransform>();
        startingRot = transform.eulerAngles;
        startingScale = transform.localScale;
    }

    private void FixedUpdate() {

        if (rotate.magnitude > 0) {
            osc = Mathf.Sin(startOffsetValue + Time.time * rotSpeedMult * generalSpeedMult);
            transform.eulerAngles = startingRot + rotate * GetOscValue() ;
        }
        if (scale.magnitude > 0) {
            osc = Mathf.Sin(startOffsetValue + Time.time * scaleSpeedMult * generalSpeedMult);
            transform.localScale = startingScale + scale * GetOscValue();
        }
    }

    public float GetOscValue() {
        if (absoluteValue) return Mathf.Abs(osc);
        else return osc;
    }






}
