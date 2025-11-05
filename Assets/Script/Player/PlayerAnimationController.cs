
/*
 *  @file   PlayerAnimationCOntroller.cs
 *  @author oorui
 */

using UnityEngine;

/// <summary>
/// プレイヤーのアニメーションを制御
/// </summary>
[RequireComponent(typeof(Animator))]
public class PlayerAnimationController : MonoBehaviour {
    private Animator animator;      // Animatorコンポーネント

    // Animatorパラーメタ名をハッシュ化
    private static readonly int TransformTrigger = Animator.StringToHash("Transform");

    /// <summary>
    /// 初期化
    /// </summary>
    private void Awake() {
        animator = GetComponent<Animator>();
    }

    /// <summary>
    /// 変形アニメーションを一度だけ再生
    /// </summary>
    public void PlayTransformAnimation() {
        // アニメーション再生
        animator.SetTrigger(TransformTrigger);
    }
}
