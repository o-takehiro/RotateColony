using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : PlayerBase {
    private float playerSpeed = 3f;

    private void Update() {
        Move();   
    }

    /// <summary>
    /// ƒvƒŒƒCƒ„[‚ÌˆÚ“®ˆ—
    /// </summary>
    public override void Move() {
        Vector3 forward = transform.forward;
        transform.Translate(forward * playerSpeed * Time.deltaTime, Space.World);

    }



}
