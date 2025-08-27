using System;
using JetBrains.Annotations;
using NUnit.Framework.Internal;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    [Header("カメラ")] public Camera Cam;
    [Header("感度")] public float Sensitivity = 1.0f;
    [Header("上下回転制限")] public float MaxLookAngleX = 55f;
    [Header("プレイヤーモデル")] public GameObject PlayerModel;
    [Header("ピボット用ゲームオブジェクト")] public GameObject CameraPivot;

    private float CurrentX = 0f; // 現在の上下回転角度
    private float CurrentY = 0f; // 現在の左右回転角度

    void Start()
    {
        Cam = GetComponent<Camera>();

        // カーソルをロックする（FPS風の操作にする場合）
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        //debug
        if (MaxLookAngleX <= 0f)
        {
            Debug.LogWarning("MaxLookAngleが0以下に設定されているため、デフォルト値を使用します。");
            MaxLookAngleX = 80f; // デフォルト値を設定
        }

        // マウスの移動量を取得
        float rotateX = 0f - Input.GetAxis("Mouse Y") * Sensitivity;
        float rotateY = Input.GetAxis("Mouse X") * Sensitivity;

        // 累積回転角度を更新
        CurrentX += rotateX;
        CurrentY += rotateY;

        // 回転の制限を適用
        CurrentX = Mathf.Clamp(CurrentX, -MaxLookAngleX, MaxLookAngleX);

        // オイラー角で直接設定することでZ軸の回転を防ぐ
        CameraPivot.transform.rotation = Quaternion.Euler(CurrentX, CurrentY, 0f);

        // ESCキーでカーソルロックを解除
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
        }
    }
}