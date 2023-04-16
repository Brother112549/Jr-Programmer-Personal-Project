using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class EnemyController : MonoBehaviour {

    [SerializeField, Tooltip("Speed at which enemy revolves")] private float angleSpeed = 50f;
    [SerializeField, Tooltip("Speed at which enemy climbs/descends spiral")] private float radialSpeed = 0.5f;
    [SerializeField, Range(0f, 7f), Tooltip("Radius where spiral becomes circular orbit")] private float radius = 3f;
    [SerializeField] private bool spiralIsCW = false;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform projectileSpawnPoint;
    [SerializeField, Range(0.375f, 5f), Tooltip("Projectiles fired per second")] private float fireRate = 1f;

    private GameManager gameManager;
    private GameObject player;
    private float fireTimer = 0f;

    // Start is called before the first frame update
    void Start() {
        player = GameObject.FindWithTag("Player");
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update() {
        //look toward player
        LookAtPlayer();

        //fire at fireRate
        Fire();

        //move in spiral down to set radius circle around origin
        SpiralMotion();
    }

    void LateUpdate() {
        //keep orientation correct
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
    }

    void OnTriggerEnter(Collider other) {
        Debug.Log(other.tag);
        if (other.CompareTag("Player Projectile")) {
            Debug.Log("Enemy Hit");
            gameManager.UpdateScore(1);
            Destroy(other.gameObject);
            Destroy(gameObject);
        }
    }

    private void LookAtPlayer() {
        //get angle to look at and current angle
        Vector3 lookDir = (player.transform.position - transform.position).normalized;
        float curYRot = transform.eulerAngles.y;
        //angle in degrees from +z axis => arctan(x/z)
        float tarYRot = (Mathf.Atan(lookDir.x / lookDir.z)) * Mathf.Rad2Deg;
        //if angle is below local x axis add 180 degrees
        if (lookDir.z < 0) {
            tarYRot += 180f;
        }

        //get amount to rotate
        float rotDif = tarYRot - curYRot;

        //rotate relative to world space
        transform.Rotate(0, rotDif, 0, Space.World);
    }

    private void Fire() {
        //time between shots = 1/fireRate
        fireTimer += Time.deltaTime;
        if (fireTimer >= (1 / fireRate)) {
            fireTimer = 0f;
            Instantiate(projectilePrefab, projectileSpawnPoint.position, projectileSpawnPoint.rotation);
        }
    }

    private void SpiralMotion() {
        //get current angle from +x axis and radius from origin 
        float angle = Mathf.Atan(transform.position.z / transform.position.x) * Mathf.Rad2Deg;
        //if angle is left of z axis add 180 degrees
        if (transform.position.x < 0) {
            angle += 180;
        }
        float curRadius = Mathf.Sqrt(Mathf.Pow(transform.position.x, 2) + Mathf.Pow(transform.position.z, 2));

        //increment angle and radius
        if (!spiralIsCW) {
            angle += angleSpeed * Time.deltaTime;
        }
        else {
            angle -= angleSpeed * Time.deltaTime;
        }
        if (curRadius < radius) {
            curRadius += radialSpeed * Time.deltaTime;
        }
        else {
            curRadius -= radialSpeed * Time.deltaTime;
        }

        //convert polar coords back to euclidean
        float newX = curRadius * Mathf.Cos(angle * Mathf.Deg2Rad);
        float newZ = curRadius * Mathf.Sin(angle * Mathf.Deg2Rad);

        //preform transform
        transform.position = new Vector3(newX, 0, newZ);
    }
}
