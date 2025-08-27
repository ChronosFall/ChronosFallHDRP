using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Player_Health : MonoBehaviour
{

    [Header("初期HP")]public int HP = 100;
    [Header("HPバーのスライダー")]public Slider HPbarSlider;
    [Header("HP数の表示")]public TextMeshProUGUI HPText;

    void Start()
    {
        //HPバーを取得
        HPbarSlider = GameObject.Find("HPbar").GetComponent<Slider>();
        //初期設定HPを最大に変更
        HPbarSlider.maxValue = HP;
    }
    void Update()
    {
        //UIのHPバーにHPを反映
        HPbarSlider.value = HP;
        //テキストにHPを反映
        HPText.text = "HP " + HP.ToString() + " / " + HPbarSlider.maxValue.ToString();
        //もしHPが0以下になったら
        if (HP <= 0)
        {
            //試験的にラグドールを実装予定
            //https://github.com/medakoro0321/ChronosFall/issues/14 [#14]
            //プレイヤーを消す
            Destroy(gameObject);
            //HP表記が0以下になってほしくないのでリセット
            HP = 0;
        }
        if (transform.position.y < -10f)
        {
            //プレイヤーを消す
            Destroy(gameObject);
        }
    }
}