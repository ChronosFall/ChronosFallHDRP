using System.Linq.Expressions;
using UnityEngine;


public class Interaction : MonoBehaviour
{
    [Header("インタラクション反応距離[m]")] public float searchRadius = 1f;
    void Update()
    {
        //インタラクション
        if (Input.GetKeyDown(KeyCode.E)) PerformInteraction();
    }

    /// <summary>
    /// インタラクション（50cm以内の最も近いオブジェクトに干渉）
    /// </summary>
    private void PerformInteraction()
    {
        //searchRadiusの範囲内のコライダーを取得
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, searchRadius);

        //最も近いオブジェクトを探す
        Collider closestObject = null;
        //最も近い距離を初期化(ようわからん)
        float closestDistance = Mathf.Infinity;

        //forEachでコライダーを検索していく
        foreach (var hitCollider in hitColliders)
        {
            //タグ:Interactableがついているかどうか
            if (hitCollider.CompareTag("Interactable"))
            {
                //距離計算
                float distance = Vector3.Distance(transform.position, hitCollider.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    //最も近いコライダーを保存
                    closestObject = hitCollider;
                }
            }
        }

        if (closestObject != null)
        {
            // InterfaceScriptを実装しているコンポーネントを探す
            IInterface interactable = closestObject.GetComponent<IInterface>();
            if (interactable != null)
            {
                interactable.Interact();
            }
            else
            {
                Debug.LogError($"{closestObject.gameObject.name} は IInteractable を実装していません！");
            }
        }
    }
}