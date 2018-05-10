using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Oscillator : MonoBehaviour {

    [SerializeField] private Vector3 movementVector = new Vector3(10f, 10f, 10f);

    [Range(0, 1)] [SerializeField] private float movementFactor;
    [SerializeField] private float period = 2f;

    private Vector3 startingPosition;

    private void Start () {
	    startingPosition = transform.position;
	}

    private void Update () {
	    if(period <= Mathf.Epsilon) return;

	    float cycles = Time.time / period;

	    const float tau = Mathf.PI * 2;
	    float rawSinWave = Mathf.Sin(cycles * tau);

	    movementFactor = rawSinWave / 2f + 0.5f;

	    Vector3 offset = movementVector * Mathf.Sin(movementFactor);
	    transform.position = startingPosition + offset;
	}
}
