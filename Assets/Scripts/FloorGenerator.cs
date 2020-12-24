using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorGenerator : MonoBehaviour
{
    //売れファブにしたAerialFloor_Midゲームオブジェクトをインスペクターからアサインする
    [SerializeField]
    private GameObject aereaFloorPrefab;

    //プレファブのクローンを生成する位置の設定
    [SerializeField]
    private Transform generateTran;

    //１回生成するまでの待機時間。どのくらいの間隔で自動生成を行うか設定
    [Header("生成までの待機時間")]
    public float waitTime;

    //待機時間の計測用
    private float timer;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        //計測している時間がwaitTimeの値と同じか、超えたら
        if (timer >= waitTime)
        {
            //次回の計測用に、timerを0にする
            timer = 0;

            //クローン生成用のメソッドを呼び出す
            GenerateFloor();
        }
    }

    private void GenerateFloor()
    {
        //空中床のプレファブを元にクローンのゲームオブジェクトを生成
        GameObject obj = Instantiate(aereaFloorPrefab, generateTran);

        //ランダムな値を取得
        float randomPosY = Random.Range(-4.0f, 4.0f);

        //生成されたゲームオブジェクトのY軸にランダムな値を加算して、生成されるたびに高さの位置を変更する
        obj.transform.position = new Vector2(obj.transform.position.x, obj.transform.position.y + randomPosY);
    }

}
