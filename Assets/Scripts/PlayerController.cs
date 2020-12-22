using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //キー入力用の文字列指定(InputManagerのHorizontalの入力を判定するための文字列)
    private string horizontal = "Horizontal";

    //キー入力用の文字指定
    private string jump = "Jump";

    //コンポーネントの取得用
    private Rigidbody2D rb;

    //Animator型のanim変数
    private Animator anim;

    //向きの設定に利用
    private float scale;

    //移動速度
    public float moveSpeed;

    //ジャンプ・浮遊力
    public float jumpPower;

    //地面の元となるものをアサインする為の変数
    [SerializeField,Header("Linecast用地面判定レイヤー")]
    private LayerMask groundLayer;

    //地面に設置しているかどうかを判定するための変数
    public bool isGrounded;

    // Start is called before the first frame update
    void Start()
    {
        //必要なコンポーネントを取得して用意した変数に代入
        rb = GetComponent<Rigidbody2D>();

        //anim変数にPlayerのAnimatorを取得する
        anim = GetComponent<Animator>();

        scale = transform.localScale.x;
    }

    private void Update()
    {
        //地面接地Phisics2D.Linecastメソッドを実行して、Ground Layerとキャラのコライダーとが
        //接地しているか確認し、接地しているならtrue、していないならfalseを戻す
        isGrounded = Physics2D.Linecast(transform.position + transform.up * 0.4f, transform.position - transform.up * 0.9f, groundLayer);

        //SceneビューにPhisics2D.LinecastメソッドのLineを表示する
        Debug.DrawLine(transform.position + transform.up * 0.4f, transform.position - transform.up * 0.9f, Color.red, 1.0f);

        //ジャンプ
        //InputManagerのjumpの項目に登録されているキー入力を判定する
        //jumpボタン(space)を押した場合
        if (Input.GetButtonDown(jump))
        {
            //Jumpメソッドを実行
            Jump();
        }

        //接地していない(空中にいる)間で、落下中の場合
        if (isGrounded == false && rb.velocity.y < 0.15f)
        {
            //落下アニメを繰り返す
            anim.SetTrigger("Fall");
        }
    }

    /// <summary>
    /// ジャンプと空中浮遊
    /// </summary>
    private void Jump()
    {
        //キャラの位置を上方向へ移動させる(ジャンプ・浮遊)
        //Rigidbody2DのAddForce関数でy方向*jumpPower移動させる
        rb.AddForce(transform.up * jumpPower);

        //jump(Up + Mid)アニメーションを再生する
        //AnimatorのTriggerのJumpとあるアニメーションを再生する
        anim.SetTrigger("Jump");
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

            //待機状態のアニメの再生を止めて、走るアニメの再生への遷移を行う
            //Idleアニメーションをfalseにして待機アニメーションを停止する
            anim.SetBool("Idle", false);

            //Runアニメーションに対して、0.5fの値を情報として渡す
            //遷移条件greater0.1なので、0.1以上の値を渡すと条件が成立してRunアニメーションが再生される
            anim.SetFloat("Run", 0.5f);
        }
        else
        {
            //左右の入力がなかったら横移動の速度を0にしてピタッと止まるようにする
            rb.velocity = new Vector2(0, rb.velocity.y);

            //走るアニメの再生を止めて、待機状態のアニメの再生への遷移を行う
            //Runアニメーションに対して0.0fの値を情報として渡す。
            //遷移条件がless0.1なので、0.1以下の値を渡すと条件が成立してRunアニメーションが停止される
            anim.SetFloat("Run", 0.0f);

            //Idleアニメーションをtrueにして、待機アニメーションを再生する
            anim.SetBool("Idle", true);
        }
    }
}
