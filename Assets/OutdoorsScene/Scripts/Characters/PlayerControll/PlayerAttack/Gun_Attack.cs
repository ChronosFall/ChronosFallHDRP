using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Gun_Attack : MonoBehaviour
{
    //宣言等
    [Header("DEBUG:TEXTUI")]
    public GameObject DebugTextUI = null;

    public void Start()
    {
        // 初期化処理
        // DebugTextUIが設定されているかチェック
        if (DebugTextUI == null)
        {
            Debug.LogWarning("DebugTextUI is not assigned in the inspector!");
        }
    }

    void Update()
    {
        // 攻撃処理
        if (Input.GetMouseButtonDown(0)) // 左クリックで攻撃
        {
            Attack();
        }
    }

    private void Attack()
    {
        // 攻撃のロジックをここに実装
        Debug.Log("Gun attack executed");

        // DebugTextUIがnullでないかチェックしてからアクセス
        if (DebugTextUI != null)
        {
            // TextMeshProUGUIコンポーネントを取得してテキストを更新
            if (DebugTextUI.GetComponent<TMPro.TextMeshProUGUI>() == null) Debug.LogError("コンポーネントがNULLです。TextMeshProUGUIをアタッチしてください。");
            TMPro.TextMeshProUGUI tmpText = DebugTextUI.GetComponent<TMPro.TextMeshProUGUI>();
            tmpText.text = "Gun Attack: Gun attack executed";
        }
        else
        {
            Debug.LogWarning("DebugTextUI is null! Please assign it in the inspector.");
        }
    }
}