using System;
using JetBrains.Annotations;
using UnityEngine;

public class Move : MonoBehaviour
{
    [Header("カメラ")] public GameObject CameraObject;
    [Header("プレイヤーの通常移動速度")] public float SetMoveSpeed = 2f;
    [Header("プレイヤーのダッシュ移動速度")] public float SetDashSpeed = 10f;
    [Header("プレイヤーの移動速度を保存")] public float MoveCurrSpeed;
    [Header("プレイヤーの座標軸移動速度")] public Vector3 MoveSpeedAxis;
    [Header("プレイヤーのRigidBody")] public Rigidbody playerRb;
    [Header("アニメーションに適用する移動速度")][SerializeField] public float TotalSpeedAxis;

    [Header("アニメーターコンポーネント")] public Animator animator;

    [Header("DEBUG:ANIMATION_SPEED_AXIS")] public float Animation_Speed_AxisX;
    public float Animation_Speed_AxisY;
    public float CameraCurrRotate_Y;
    [Header("DEBUG:INPUT_KEY_ANIMATION")] public float InputHorizontal;
    public float InputVertical;

    void Start()
    {

        // アニメーターコンポーネント取得
        animator = GetComponent<Animator>();
        //プレイヤーのRigidbodyを取得
        playerRb = GetComponent<Rigidbody>();
        //最初に代入
        MoveCurrSpeed = SetMoveSpeed;
    }
    public void Update()
    {

        //カメラの向いてる向きを取得
        CameraCurrRotate_Y = CameraObject.transform.eulerAngles.y;
        //モデルにカメラの向いてる向きをY座標軸のみ同期
        transform.rotation = Quaternion.Euler(transform.eulerAngles.x, CameraCurrRotate_Y, transform.eulerAngles.z);

        //カメラに対して前と右を取得
        Vector3 cameraForward = Vector3.Scale(CameraObject.transform.forward, new Vector3(1, 0, 1)).normalized;
        Vector3 cameraRight = Vector3.Scale(CameraObject.transform.right, new Vector3(1, 0, 1)).normalized;

        //moveVelocityを0で初期化する
        MoveSpeedAxis = Vector3.zero;

        //移動入力
        if (Input.GetKey(KeyCode.W))
        {
            InputReset();
            MoveSpeedAxis += MoveCurrSpeed * cameraForward;
            InputVertical = 1f; // 前進入力
        }
        if (Input.GetKey(KeyCode.A))
        {
            InputReset();
            MoveSpeedAxis -= MoveCurrSpeed * cameraRight;
            InputHorizontal = -1f; // 左移動入力
        }
        if (Input.GetKey(KeyCode.S))
        {
            InputReset();
            MoveSpeedAxis -= MoveCurrSpeed * cameraForward;
            InputVertical = -1f; // 後退入力
        }
        if (Input.GetKey(KeyCode.D))
        {
            InputReset();
            MoveSpeedAxis += MoveCurrSpeed * cameraRight;
            InputHorizontal = 1f; // 右移動入力
        }
        //Left/Right Shiftキーでダッシュ
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            MoveCurrSpeed = SetDashSpeed;
            animator.speed = 2.0f; // アニメーション速度を倍速に変更
        }
        else
        {
            MoveCurrSpeed = SetMoveSpeed;
            animator.speed = 1.0f; // アニメーション速度を通常に戻す
        }

        //移動メソッド
        ApplyForce();
    }
    /// <summary>
    /// 入力をリセットする
    /// </summary>
    public void InputReset()
    {
        InputHorizontal = 0f;
        InputVertical = 0f;
    }
    /// <summary>
    /// 移動方向に力を加える（重力対応）
    /// </summary>
    public void ApplyForce()
    {
        // 現在のY軸の速度を保存
        float CurrY = playerRb.linearVelocity.y;

        // X/Z軸の移動速度を適用
        playerRb.linearVelocity = new Vector3(MoveSpeedAxis.x, CurrY, MoveSpeedAxis.z);

        // アニメーション用のパラメータ計算
        if (animator != null && animator.runtimeAnimatorController != null)
        {
            float moveAmount = MoveSpeedAxis.magnitude;

            // 動きがある場合（閾値を小さくして感度を上げる）
            if (moveAmount > 0.01f)
            {
                //アニメーション速度を計算
                Animation_Speed_AxisX = InputHorizontal;
                Animation_Speed_AxisY = InputVertical;

                animator.SetBool("isWalking", true);
                animator.SetFloat("Speed_AxisX", Animation_Speed_AxisX);
                animator.SetFloat("Speed_AxisY", Animation_Speed_AxisY);
            }
        }
    }

}