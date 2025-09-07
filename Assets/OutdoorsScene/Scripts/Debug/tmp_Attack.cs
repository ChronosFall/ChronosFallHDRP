using UnityEngine;

public class tmp_Attack : MonoBehaviour
{
    public Animator animator;
    void Start()
    {
        // アニメーターコンポーネント取得
        animator = GetComponent<Animator>();
    }
    void Update()
    {
        //攻撃
        //左クリック(値:0)するたびに攻撃
        if (Input.GetMouseButtonDown(0)) Attack();
    }
    /// <summary>
    /// 左クリックするたびに呼び出し
    /// </summary>
    private void Attack()
    {
        GameObject[] allModels = GameObject.FindGameObjectsWithTag("Models");
        foreach (GameObject model in allModels)
        {
            float AttackDistance = Vector3.Distance(transform.position, model.transform.position);

            //直線距離で3m以内に敵がいるかどうか
            if (AttackDistance <= 3f)
            {
                //50-攻撃距離*10の範囲でダメージを与える
                int MaxAttackDamage = (int)(50 - AttackDistance * 10);
                int Damage = UnityEngine.Random.Range(1, MaxAttackDamage);
                Debug.Log("敵に" + Damage + "/" + MaxAttackDamage + "ダメージを与えた");
                //敵のHPを減らす
                //model.GetComponent<EnemyWalkModel>().HP -= Damage;
            }
            //アニメーション再生
            animator.SetTrigger("AttackTrigger");
            /*
            * アニメーションパラメーター一覧
            * 1:剣を振る
            */
            int Attack_Type = 1;
            animator.SetInteger("Attack_Type", Attack_Type);
            switch (Attack_Type)
            {
                case 1:
                    animator.SetInteger("Attack_SwordID", Random.Range(0, 1));
                    break;
                // 他の攻撃タイプがあればここに追加
                default:
                    Debug.LogError("未知の攻撃タイプ");
                    break;
            }
        }
    }
}