using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Revolve : MoveForward {

    [SerializeField] private float spinRateRange = 10;
    [SerializeField, Tooltip("Set to random value between positive and negative Spin Rate Range in Start method")] private float spinRate;

    // Awake is called before the first frame update
    void Awake() {
        spinRate = Random.Range(-spinRateRange, spinRateRange);
    }

    protected override void Move() {
        //turn
        transform.Rotate(Vector3.up, spinRate * Time.deltaTime, Space.World);

        //move in forward direction on xz plane
        Vector3 moveDirection = new Vector3(transform.forward.x, 0, transform.forward.z);
        transform.Translate(moveDirection.normalized * speed * Time.deltaTime, Space.World);
    }
}
