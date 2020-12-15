using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //キー入力用の文字列指定(InputManagerのHorizontalの入力を判定するための文字列)
    private string horizontal = "Horizontal";

    //コンポーネントの取得用
    private Rigidbody2D rb;

    //向きの設定に利用
    private float scale;

    //移動速度
    public float moveSpeed;

    // Start is called before the first frame update
    void Start()
    {
        //必要なコンポーネントを取得して用意した変数に代入
        rb = GetComponent<Rigidbody2D>();

        scale = transform.localScale.x;
    }


    // Update is called once per frame
    void FixedUpdate()
    {
        //移動
        Move();
        
    }

    ///<summary>
    ///移動
    ///<summary>
    private void Move()
    {
        //水平(横)方向への入力受付
        //InputManagerのHorizontalに登録されているキーの入力があるかどうか確認を行う
        float x = Input.GetAxis(horizontal);

        //xの値が0ではない場合 = キー入力がある場合
        if (x != 0)
        {
            //velocity(速度)に新しい値を代入して移動
            rb.velocity = new Vector2(x * moveSpeed, rb.velocity.y);

            //temp変数に現在のlocalScale値を代入
            Vector3 temp = transform.localScale;

            //現在のキー入力値xをtemp.xに代入
            temp.x = x;

            //向きが変わるときに少数になるとキャラが縮んで見えてしまうので整数値にする
            if (temp.x > 0)
            {
                //数字が0よりも大きければすべて1にする
                temp.x = scale;
            }
            else
            {
                //数字が0よりも小さければすべて-1にする
                temp.x = -scale;
            }
            //キャラの向きを移動方向に合わせる
            transform.localScale = temp;
        }
        else
        {
            //左右の入力がなかったら横移動の速度を0にしてピタッと止まるようにする
            rb.velocity = new Vector2(0, rb.velocity.y);
        }
    }
}
