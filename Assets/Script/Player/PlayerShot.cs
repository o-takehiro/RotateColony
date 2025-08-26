using UnityEngine;
using Cysharp.Threading.Tasks;

/// <summary>
/// プレイヤーの射撃処理（プール対応版）
/// </summary>
public class PlayerShot : MonoBehaviour {
    private PlayerMove playerMove; // プレイヤー移動スクリプト参照

    [Header("Fire Points")]
    [SerializeField] private Transform leftFirePoint;
    [SerializeField] private Transform rightFirePoint;
    [SerializeField] private Transform shotTargetPoint;

    [Header("Projectile Settings")]
    [SerializeField] private int damage = 10;
    [SerializeField] private float maxSpreadAngle = 30f;

    [Header("Pooling")]
    [SerializeField] private ProjectilePool projectilePool; // 弾のプール

    [Header("Fire Settings")]
    private readonly float FIRE_INTERVAL = 0.1f; // 発射間隔
    private float leftFireTimer = 0f;
    private float rightFireTimer = 0f;

    public bool isShot = false; // 射撃可能フラグ
    private const int _FIRE_SE_ID = 2; // 発射音ID

    private void Start() {
        playerMove = GetComponent<PlayerMove>();
        isShot = false;
    }

    private async void Update() {
        // 移動開始していなければ撃てない
        if (playerMove == null || !playerMove.GetIsMoving()) return;

        if (isShot) {
            Fire();
            await SoundManager.instance.PlaySE(_FIRE_SE_ID);
        }
    }

    /// <summary>
    /// 左側から発射
    /// </summary>
    public void FireLeft() {
        FireFromPoint(leftFirePoint);
    }

    /// <summary>
    /// 右側から発射
    /// </summary>
    public void FireRight() {
        FireFromPoint(rightFirePoint);
    }

    /// <summary>
    /// 左右同時発射
    /// </summary>
    public void FireBoth() {
        if (playerMove == null || !playerMove.GetIsMoving()) return;

        FireLeft();
        FireRight();
    }

    /// <summary>
    /// プールから弾を取得して初期化・発射
    /// </summary>
    private void FireFromPoint(Transform firePoint) {
        GameObject proj = projectilePool.GetProjectile();
        proj.transform.position = firePoint.position;
        proj.transform.rotation = Quaternion.identity;

        Projectile projScript = proj.GetComponent<Projectile>();
        projScript.damage = damage;

        // 左右で方向を変える
        Vector3 forward = (shotTargetPoint.position - firePoint.position).normalized;
        Vector3 rightDir = Vector3.Cross(Vector3.up, forward);
        Vector3 offsetDir = firePoint == leftFirePoint ? -rightDir : rightDir;

        projScript.Initialize(firePoint.position, shotTargetPoint, maxSpreadAngle, offsetDir);
    }

    /// <summary>
    /// 発射処理（インターバル管理）
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
