using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartChecker : MonoBehaviour
{
    //MoveObjectスクリプトを取得した際に代入
    private MoveObject moveObject;

    // Start is called before the first frame update
    void Start()
    {
        //このスクリプトがアタッチされているゲームオブジェクトの持つ、MoveObjectスクリプトを
        //探して取得し、moveObject変数に代入
        moveObject = GetComponent<MoveObject>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// 空中床に移動速度を与える
    /// </summary>
    public void SetInitialSpeed()
    {
        //アサインしているゲームオブジェクトの持つMoveObjectスクリプトのmoveSpeed変数に
        //アクセスして、右辺の値を代入する
        moveObject.moveSpeed = 0.005f;
    }
}
