using UnityEngine;
using UnityEngine.Scripting;

public class PlayerShot : MonoBehaviour {
    // PlayerMoveクラスを参照
    private PlayerMove playerMove;


    [SerializeField] private Transform leftFirePoint;
    [SerializeField] private Transform rightFirePoint;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform shotTargetPoint;

    [SerializeField] private float maxSpreadAngle = 30f;
    [SerializeField] private int damage = 10;
    // [SerializeField] private float flightDuration = 1f;

    // 弾の発射間隔
    private readonly float FIRE_INTERVAL = 0.1f;
    private float leftFireTimer = 0f;
    private float rightFireTimer = 0f;

    // 射撃可能フラグ
    public bool isShot = false;
    // 使用するSEのID
    private const int _FIRE_SE_ID = 2;

    /// <summary>
    /// 初期化処理
    /// </summary>
    private void Start() {
        playerMove = GetComponent<PlayerMove>();
        isShot = false;
    }

    /// <summary>
    /// 更新処理
    /// </summary>
    private async void Update() {
        // プレイヤーが移動開始していなければ撃てない
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
    /// 弾の同時発射
    /// </summary>
    public void FireBoth() {
        if (playerMove == null || !playerMove.GetIsMoving()) return;

        FireLeft();
        FireRight();
    }

    /// <summary>
    /// 弾の着弾点を探し、弾を生成する
    /// </summary>
    /// <param name="firePoint"></param>
    private void FireFromPoint(Transform firePoint) {
        GameObject proj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        Projectile projScript = proj.GetComponent<Projectile>();
        projScript.damage = damage;

        // 左右で方向を変える（FirePointの向きを使って決定）
        Vector3 forward = (shotTargetPoint.position - firePoint.position).normalized;
        Vector3 rightDir = Vector3.Cross(Vector3.up, forward); // 「右方向」

        // 左なら右方向を反転
        Vector3 offsetDir = firePoint == leftFirePoint ? -rightDir : rightDir;


        // 弾を生成
        projScript.Initialize(firePoint.position, shotTargetPoint, maxSpreadAngle, offsetDir);

    }

    /// <summary>
    /// 発射処理
    /// </summary>
    private void Fire() {
        // 経過時間を足す
        leftFireTimer += Time.deltaTime;
        rightFireTimer += Time.deltaTime;

        // 0.3秒ごとに弾を発射 x 2
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