using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundScroller : MonoBehaviour
{
    [Header("背景画像のスクロール速度 = 強制スクロールの速度")]
    //浮動小数点型scrollSpeed変数に0.01を代入
    public float scrollSpeed = 0.01f;

    [Header("画像のスクロール終了地点")]
    //浮動小数点型stopPosition変数に-16を代入
    public float stopPosition = -16f;

    [Header("画像の再スタート地点")]
    //浮動小数点型restartPositionに5.8を代入
    public float restartPosition = 5.8f;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //画面の左方向にこのゲームオブジェクト(背景)の位置を移動する
        transform.Translate(-scrollSpeed, 0, 0);
        
        //このゲームオブジェクトの位置がstopPositionに到達したら
        //背景画像ループ処理
        if (transform.position.x < stopPosition)
        {
            //ゲームオブジェクトの位置を再スタート地点へ移動する
            transform.position = new Vector2(restartPosition, 0);
        }
    }
}
