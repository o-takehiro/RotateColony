using UnityEngine;
using Cysharp.Threading.Tasks;

/// <summary>
/// プレイヤーの射撃処理（左右別プール対応）
/// </summary>
public class PlayerShot : MonoBehaviour {
    private PlayerMove playerMove;

    [Header("Fire Points")]
    [SerializeField] private Transform leftFirePoint;
    [SerializeField] private Transform rightFirePoint;
    [SerializeField] private Transform shotTargetPoint;

    [Header("Projectile Settings")]
    [SerializeField] private int damage = 10;
    [SerializeField] private float maxSpreadAngle = 30f;

    [Header("Projectile Pools")]
    [SerializeField] private ProjectilePool leftProjectilePool;
    [SerializeField] private ProjectilePool rightProjectilePool;

    private readonly float FIRE_INTERVAL = 0.1f;
    private float leftFireTimer = 0f;
    private float rightFireTimer = 0f;

    public bool isShot = false;          // 射撃可能フラグ
    private const int _FIRE_SE_ID = 2;   // 発射音ID

    private void Start() {
        playerMove = GetComponent<PlayerMove>();
        isShot = false;
    }

    private async void Update() {
        if (playerMove == null || !playerMove.GetIsMoving()) return;

        if (isShot) {
            Fire();
            await SoundManager.instance.PlaySE(_FIRE_SE_ID);
        }
    }

    private void FireLeft() => FireFromPoint(leftFirePoint, leftProjectilePool);
    private void FireRight() => FireFromPoint(rightFirePoint, rightProjectilePool);

    private void FireBoth() {
        if (playerMove == null || !playerMove.GetIsMoving()) return;
        FireLeft();
        FireRight();
    }

    /// <summary>
    /// 指定FirePointから弾を発射
    /// </summary>
    private void FireFromPoint(Transform firePoint, ProjectilePool pool) {
        Projectile projScript = pool.GetProjectile();
        if (projScript == null) return;

        projScript.transform.position = firePoint.position;
        projScript.transform.rotation = Quaternion.identity;
        projScript.damage = damage;

        // 左右で弾道の方向を調整
        Vector3 forward = (shotTargetPoint.position - firePoint.position).normalized;
        Vector3 rightDir = Vector3.Cross(Vector3.up, forward);
        Vector3 offsetDir = firePoint == leftFirePoint ? -rightDir : rightDir;

        projScript.Initialize(firePoint.position, shotTargetPoint, maxSpreadAngle, offsetDir, pool);
    }

    /// <summary>
    /// 発射間隔の管理
    /// </summary>
    private void Fire() {
        leftFireTimer += Time.deltaTime;
        rightFireTimer += Time.deltaTime;

        if (leftFireTimer >= FIRE_INTERVAL) {
            FireLeft();
            leftFireTimer = 0f;
        }

        if (rightFireTimer >= FIRE_INTERVAL) {
            FireRight();
            rightFireTimer = 0f;
        }
    }
}
