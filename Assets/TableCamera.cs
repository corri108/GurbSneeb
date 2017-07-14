using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableCamera : MonoBehaviour {

    private Vector3 originalPosition;
    private float originalSize;
    private Camera cam;

    private bool isZooming = false;
    private Vector3 targetPosition;
    private float targetSize;
    private float lerpSpeed = 0.1f;
	// Use this for initialization
	void Start ()
    {
        cam = GetComponent<Camera>();
        originalPosition = this.transform.position;
        originalSize = cam.orthographicSize;
	}
	
	// Update is called once per frame
	void Update ()
    {
		if(isZooming)
        {
            this.transform.position = Vector3.Lerp(this.transform.position, targetPosition, lerpSpeed);
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetSize, lerpSpeed);
        }
        else
        {

            this.transform.position = Vector3.Lerp(this.transform.position, originalPosition, lerpSpeed);
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, originalSize, lerpSpeed);
        }
	}

    public void Zoom(Vector3 target, float size)
    {
        this.targetPosition = target;
        this.targetSize = size;
        isZooming = true;
    }

    public void Zoomout()
    {
        isZooming = false;
    }
}
