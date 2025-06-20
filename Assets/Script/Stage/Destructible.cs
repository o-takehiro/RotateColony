using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Destructible : MonoBehaviour {
    private int durability = 200;

    // ”íƒ_ƒ
    public void TakeDamage(int amount) {
        durability -= amount;
        if (durability <= 0) {
            // ‘Ï‹v’l‚ª‚È‚­‚È‚èŸ‘æíœ
            Destroy(gameObject);
        }
    }
}
