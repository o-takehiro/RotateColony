using UnityEngine;

public class PlayerShot : MonoBehaviour {
    [SerializeField] private Transform leftFirePoint;
    [SerializeField] private Transform rightFirePoint;
    [SerializeField] private GameObject projectilePrefab;

    [SerializeField] private float maxSpreadAngle = 30f;
    [SerializeField] private int damage = 10;
    [SerializeField] private float flightDuration = 1f;

    private void Update() {
        if (Input.GetKey(KeyCode.A)) {
            FireLeft();
        }

        if (Input.GetKey(KeyCode.D)) {
            FireRight();
        }


    }

    /// <summary>
    /// ¶‘¤‚©‚ç”­Ë
    /// </summary>
    public void FireLeft() {
        FireFromPoint(leftFirePoint);
    }

    /// <summary>
    /// ‰E‘¤‚©‚ç”­Ë
    /// </summary>
    public void FireRight() {
        FireFromPoint(rightFirePoint);
    }

    /// <summary>
    /// ’e‚Ì’…’e“_‚ğ’T‚µA’e‚ğ¶¬‚·‚é
    /// </summary>
    /// <param name="firePoint"></param>
    private void FireFromPoint(Transform firePoint) {
        // ³–Ê•ûŒü‚ÉRaycast‚µ‚ÄObstacle‚ğ’T‚·
        if (Physics.Raycast(firePoint.position, firePoint.forward, out RaycastHit hit, 100f)) {
            if (hit.collider.CompareTag("Obstacle")) {
                Vector3 targetPos = hit.point;

                // ’e¶¬
                GameObject proj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
                Projectile projScript = proj.GetComponent<Projectile>();
                projScript.damage = damage;
                projScript.flightDuration = flightDuration;

                // Å‘åŠgUŠp“x‚ğ“n‚µ‚Ä‰Šú‰»
                projScript.Initialize(firePoint.position, targetPos, maxSpreadAngle);
            }
        }
    }
}