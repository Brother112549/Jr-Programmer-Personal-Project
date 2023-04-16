using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour {
    // ENCAPSULATION
    [SerializeField, Tooltip("Engine thrust")] private float force = 10f;
    [SerializeField, Tooltip("Engine rotation speed")] private float rotatSpeed = 50f;
    [SerializeField] private float spinTime = 1.0f;
    [SerializeField] private float recoveryTime = 0.5f;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField, Range(0.375f, 15f), Tooltip("Projectiles fired per second")] private float fireRate = 1.5f;

    private Rigidbody playerRb;
    private GameObject engine;
    private GameObject blaster;
    private GameObject projectileSpawnPoint;
    private Camera m_cam;
    private GameManager gameManager;
    private bool isSpinning = false;
    private bool isRecovering = false;
    private float spinTimer = 0f;
    private float fireTimer = 0f;

    // Start is called before the first frame update
    void Start() {
        playerRb = GetComponent<Rigidbody>();
        engine = transform.Find("Engine").gameObject;
        blaster = transform.Find("Blaster").gameObject;
        projectileSpawnPoint = blaster.transform.GetChild(0).gameObject;
        m_cam = Camera.main;
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update() {// ABSTRACTION
        EngineControl();

        WeaponControl();
    }

    // FixedUpdate is called once per physics step
    void FixedUpdate() {// ABSTRACTION
        ThrustControl();
    }

    void LateUpdate() {// ABSTRACTION
        SpinControl();
        //Height Control (No using ramps)
        transform.position = new Vector3(transform.position.x, 0, transform.position.z);
    }

    void OnCollisionEnter(Collision collision) {
        //collision with walls/edge/obstacle bounce and cause spin
        if (collision.collider.CompareTag("Obstacle") || collision.collider.CompareTag("Environment")) {
            isSpinning = true;
            spinTimer = spinTime;
            Debug.Log("Player collided with obstacle");
        }
        //collision with red = lose health down to game over
        if (collision.collider.CompareTag("Enemy")) {
            Debug.Log("Player collided with enemy");
        }
    }

    void OnTriggerEnter(Collider other) {
        //collision with green = gain health up to max
        if (other.CompareTag("Pickup")) {
            Debug.Log("Activate Pickup");
            gameManager.UpdateLives(1);
            Destroy(other.gameObject);
        }
        //collision with projectile = lose health down to game over
        if (other.CompareTag("Enemy Projectile")) {
            Debug.Log("Player Hit");
            gameManager.UpdateLives(-1);
            Destroy(other.gameObject);
        }
    }

    private void EngineControl() {
        //rotate engine, force from engine direction for thrust
        if (Input.GetKey(KeyCode.LeftArrow)) {
            //Left arrow = CCW rotation of engine
            engine.transform.RotateAround(transform.position, transform.up, -rotatSpeed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.RightArrow)) {
            //Right arrow = CW rotation of engine
            engine.transform.RotateAround(transform.position, transform.up, rotatSpeed * Time.deltaTime);
        }
    }

    private void ThrustControl() {
        if (Input.GetKey(KeyCode.UpArrow)) {
            //Up arrow for thrust
            //Get thrust direction from engine rotation
            Vector3 thrust = new Vector3(-engine.transform.localPosition.x, 0, -engine.transform.localPosition.z);
            thrust.Normalize();
            thrust *= force;
            playerRb.AddRelativeForce(thrust);
        }
    }

    private void WeaponControl() {
        //rotate weapon using mouse position on screne
        //get mouse position
        Vector3 lookAtPos = Input.mousePosition;
        lookAtPos.z = 0f;
        lookAtPos = m_cam.ScreenToWorldPoint(lookAtPos);
        //get angle between player and mouse
        Vector3 lookDir = (lookAtPos - transform.position).normalized;
        float tarYRot = (Mathf.Atan(lookDir.x / lookDir.z)) * Mathf.Rad2Deg;
        //if target angle is below local x axis add 180 degrees
        if (lookDir.z < 0) {
            tarYRot += 180f;
        }
        //get current angle of balster
        Vector3 curDir = (blaster.transform.position - transform.position).normalized;
        float curYRot = (Mathf.Atan(curDir.x / curDir.z)) * Mathf.Rad2Deg;
        //if current angle is below local x axis add 180 degrees
        if (curDir.z < 0) {
            curYRot += 180f;
        }
        //rotate blaster around player to point at mouse
        blaster.transform.RotateAround(transform.position, transform.up, tarYRot - curYRot);

        //click(hold) to fire weapon
        //press and hold left click to fire at fire rate
        //OR every click fires once
        if (Input.GetKeyDown(KeyCode.Mouse0)) {
            //fire
            Instantiate(projectilePrefab, projectileSpawnPoint.transform.position, projectileSpawnPoint.transform.rotation);
            //start timer for fireRate
            fireTimer = 0f;
        }
        if (Input.GetKey(KeyCode.Mouse0)) {
            fireTimer += Time.deltaTime;
            //fire at 1/fireRate
            if (fireTimer >= 1 / fireRate) {
                Instantiate(projectilePrefab, projectileSpawnPoint.transform.position, projectileSpawnPoint.transform.rotation);
                fireTimer -= (1 / fireRate);
            }
        }
    }

    //control player spin based on recent collision
    private void SpinControl() {
        if (!isSpinning && !isRecovering) {
            //fix rotation such that up is always up for normal gameplay
            transform.eulerAngles = Vector3.zero;
        }
        else if (!isRecovering) {
            //decrement timer until spin is stopped
            spinTimer -= Time.deltaTime;
            transform.Rotate(new Vector3(-transform.eulerAngles.x * (Time.deltaTime / spinTime),
                -transform.eulerAngles.y * (Time.deltaTime / spinTime), -transform.eulerAngles.z * (Time.deltaTime / spinTime)));
            if (spinTimer <= 0) {
                //start recovery from spin
                isSpinning = false;
                isRecovering = true;
                spinTimer = recoveryTime;
            }
        }
        else {
            //spend recoveryTime to gradually reset rotations
            spinTimer -= Time.deltaTime;
            transform.Rotate(new Vector3(transform.eulerAngles.x * (recoveryTime / Time.deltaTime),
                transform.eulerAngles.y * (recoveryTime / Time.deltaTime), transform.eulerAngles.z * (recoveryTime / Time.deltaTime)));
            if (spinTimer <= 0) {
                //recovery finished, return to normal gameplay
                isRecovering = false;
                transform.eulerAngles = Vector3.zero;
            }
        }
    }
}
