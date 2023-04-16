using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MoveForward : MonoBehaviour {

    [SerializeField] protected float speed = 10.0f;

    private int xBound = 20;
    private int zBound = 11;

    // Update is called once per frame
    void Update() {
        Move();

        //destroy gameObject if outside bounds
        if (transform.position.x > xBound || transform.position.x < -xBound || transform.position.z > zBound || transform.position.z < -zBound) {
            Destroy(gameObject);
        }
    }

    protected virtual void Move() {
        //move in forward direction on xz plane
        Vector3 moveDirection = new Vector3(transform.forward.x, 0, transform.forward.z);
        transform.Translate(moveDirection.normalized * speed * Time.deltaTime, Space.World);
    }

    void OnTriggerEnter(Collider other) {
        if (!(other.CompareTag("Enemy Projectile") || other.CompareTag("Player Projectile"))) {
            //ensures that other is a projectile for simpler code later in obstacle v projectile
            return;
        }
        //projectile v projectile
        if ((other.CompareTag("Enemy Projectile") && gameObject.CompareTag("Player Projectile"))
            || (gameObject.CompareTag("Enemy Projectile") && other.CompareTag("Player Projectile"))) {
            //destroy both colliding projectiles
            Destroy(gameObject);
            Destroy(other.gameObject);
        }
        //obstacle v projectile
        if (gameObject.CompareTag("Obstacle")) {
            //destroy projectile when it hits an obstacle
            Destroy(other.gameObject);
        }
    }
}
