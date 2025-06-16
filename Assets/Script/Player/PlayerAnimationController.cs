using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// プレイヤーのアニメーションを制御
/// </summary>
[RequireComponent(typeof(Animator))]
public class PlayerAnimationController : MonoBehaviour {
    private Animator animator;

    // Animatorパラーメタ名をハッシュ化
    private static readonly int TransformTrigger = Animator.StringToHash("Transform");

    private void Awake() {
        animator = GetComponent<Animator>();
    }

    /// <summary>
    /// 変形アニメーションを一度だけ再生
    /// </summary>
    public void PlayTransformAnimation() {
        animator.SetTrigger(TransformTrigger);
    }
}
