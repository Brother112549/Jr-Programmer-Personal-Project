using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class IgnoreNonPlayerCollision : MonoBehaviour {
    //player binding walls ignore collisions with everything else
    void OnCollisionEnter(Collision collision) {
        if (!collision.gameObject.CompareTag("Player")) {
            Physics.IgnoreCollision(collision.collider, GetComponent<Collider>());
        }
    }
}
