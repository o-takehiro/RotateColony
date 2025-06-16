using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollisionHandler : MonoBehaviour
{
    private PlayerMove playerMove;

    private void Awake() {
        playerMove = GetComponent<PlayerMove>();
    }

    private void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.CompareTag("Obstacle")) {
            playerMove.StopMoving();
        }
    }
}
