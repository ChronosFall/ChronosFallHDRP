using System.Collections;
using UnityEngine;
using UnityEngine.UI;


public class tmp_OpenMenu : MonoBehaviour
{

    private bool isActive = false;
    [Header("ESCメニューの透明度、既定値120")] private int ESCMenuColor = 120;
    [Header("ESCメニューのオブジェクト")] public GameObject ESCMenu;

    void Update()
    {
        //Escメニューを開く
        if (Input.GetKeyDown(KeyCode.Escape)) StartCoroutine(OpenESCMenu()); // ← IEnumerator型をコルーチンとして実行
    }
    /// <summary>
    /// ESCメニューを開く処理をここに追加
    /// </summary>
    private IEnumerator OpenESCMenu()
    {

        Image escImage = ESCMenu.GetComponent<Image>();
        if (!isActive)
        {
            for (int colorA = 0; colorA <= ESCMenuColor; colorA++)
            {
                escImage.color = new Color32(140, 140, 140, (byte)colorA);
                yield return new WaitForSeconds(0.001f); // yield return new WaitForSeconds <- 1フレーム待つ
            }
            isActive = true;
        }
        else
        {
            for (int colorA = ESCMenuColor; colorA >= 0; colorA--)
            {
                escImage.color = new Color32(140, 140, 140, (byte)colorA);
                yield return new WaitForSeconds(0.001f);
            }
            isActive = false;
        }
    }
}