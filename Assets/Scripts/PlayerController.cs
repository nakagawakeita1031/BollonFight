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

    //風船が何個あるか(配列)
    public GameObject[] ballons;

    //バルーンを生成する最大値
    public int maxBallonCount;

    //バルーンの生成位置の配列
    public Transform[] ballonTrans;

    //バルーンのプレファブ
    public GameObject ballonPrefab;

    //バルーンを生成する時間
    public float generateTime;

    //バルーンを生成中かどうか判定する。falseなら生成していない状態。trueは生成中の状態
    public bool isGenerating;

    //初めてバルーンを生成したかを判定するための変数(後ほど外部スクリプトでも利用するためpublicで宣言)
    public bool isFirstGenerateBallon;


    //画面外に出ないようにするための変数
    private float limitPosX = 9.5f;
    private float limitposY = 4.45f;

    [SerializeField]
    private StartChecker startChecker;

    public float knockbackPower;
 
    // Start is called before the first frame update
    void Start()
    {
        //必要なコンポーネントを取得して用意した変数に代入
        rb = GetComponent<Rigidbody2D>();

        //anim変数にPlayerのAnimatorを取得する
        anim = GetComponent<Animator>();

        scale = transform.localScale.x;

        //配列の初期化(バルーンの最大生成数だけ配列の要素数を用意)
        ballons = new GameObject[maxBallonCount];
    }

    private void Update()
    {
        //地面接地Phisics2D.Linecastメソッドを実行して、Ground Layerとキャラのコライダーとが
        //接地しているか確認し、接地しているならtrue、していないならfalseを戻す
        isGrounded = Physics2D.Linecast(transform.position + transform.up * 0.4f, transform.position - transform.up * 0.9f, groundLayer);

        //SceneビューにPhisics2D.LinecastメソッドのLineを表示する
        Debug.DrawLine(transform.position + transform.up * 0.4f, transform.position - transform.up * 0.9f, Color.red, 1.0f);

        //ballons変数の最初の要素の値が空でないならバルーンが１つ生成されるとこの要素に値が代入される＝バルーンが１つあるなら
        if (ballons[0] != null)
        {
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
        else
        {
            Debug.Log("バルーンがない。ジャンプ不可");
        }

        //Velocity.yの値が5.0ｆを超える場合(ジャンプを連続で押した場合)
        if (rb.velocity.y > 5.0f)
        {
            //Velocity.yの値に制限をかける(落下せずに上空での待機現象防止のため)
            rb.velocity = new Vector2(rb.velocity.x, 5.0f);
        }

        //地面に接地していて、バルーンが生成中ではない場合
        if (isGrounded == true && isGenerating == false)
        {
            //Qボタンを押したら
            if (Input.GetKeyDown(KeyCode.Q))
            {
                //バルーンを１つ作成する
                StartCoroutine(GenerateBallon());
            }
            
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

        //現在の位置情報が移動範囲の制限範囲を超えていないか確認する。超えていたら制限範囲内に収める
        float posX = Mathf.Clamp(transform.position.x, -limitPosX, limitPosX);
        float posY = Mathf.Clamp(transform.position.y, -limitposY, limitposY);

        //現在の位置を更新(制限範囲を超えた場合、ここで移動の範囲を制限する)
        transform.position = new Vector2(posX, posY);
    }

    /// <summary>
    /// バルーン生成
    /// </summary>
    /// <returns></returns>
    private IEnumerator GenerateBallon()
    {
        //すべての配列の要素にバルーンが存在している場合には、バルーンを生成しない
        if (ballons[1] != null)
        {
            yield break;
        }

        //生成中状態にする
        isGenerating = true;

        //isFirstGenerateBallon変数の値がfalse(初回バルーン未生成)
        if (isFirstGenerateBallon == false)
        {
            //初回バルーンを生成を行ったとし、trueに変更する。
            //(次回以降はバルーンを生成しても、if文の条件を満たさなくなり、この処理には入らない)
            isFirstGenerateBallon = true;

            Debug.Log("初回バルーン生成");

            //startChecker変数に代入されているStartCheckerスクリプトにアクセスして、SetInitialSpeedメソッドを実行する
            startChecker.SetInitialSpeed();
        }

        //1つめの配列の要素が空なら
        if (ballons[0] == null)
        {
            //1つめ目のバルーンを生成して、１番目の配列へ代入
            ballons[0] = Instantiate(ballonPrefab, ballonTrans[0]);

            ballons[0].GetComponent<Ballon>().SetUpBallon(this);
        }
        else
        {
            //2つ目のバルーンを生成して、２番目の配列へ代入
            ballons[1] = Instantiate(ballonPrefab, ballonTrans[1]);
            ballons[1].GetComponent<Ballon>().SetUpBallon(this);
        }

        //生成時間分待機
        yield return new WaitForSeconds(generateTime);

        //生成中状態終了。再度生成できるようにする
        isGenerating = false;
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        //接触したものがコライダーを持つTagがEnemyなら
        if (col.gameObject.tag == "Enemy")
        {
            //キャラと敵の位置から距離と方向を計算して、正規化処理を行い、direction変数へ代入
            Vector3 direction = (transform.position - col.transform.position).normalized;

            //敵の反対側にキャラを吹き飛ばす
            transform.position += direction * knockbackPower;
        }
    }
    /// <summary>
    /// バルーン破壊
    /// </summary>
    public void DestroyBallon()
    {
        //TODO 後ほど、バルーンが破壊される際に「割れた」ようにみえるアニメ演出を追加する

        if (ballons[1] != null)
        {
            Destroy(ballons[1]);
        }
        else if (ballons[0] != null)
        {
            Destroy(ballons[0]);
        }
    }
}
