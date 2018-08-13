using UnityEngine;
using System.Collections;

public class test : MonoBehaviour {

	void Start () {
        NeoGameSDK_CS.init(7, api_cb_login, api_cb_pay, api_cb_lgoinout);
        NeoGameSDK_CS.login();
	}

    void api_cb_login(NeoGameSDK_login_data one)
    {
        Debug.Log("uid:" + one.uid + " token:" + one.token + " wallet:" + one.wallet);

        //test_getAuctionSgas();
        //test_rechargeToken();
    }
    void api_cb_pay(NeoGameSDK_pay_data one)
    {
        
    }
    void api_cb_lgoinout()
    {

    }

    //市场合约余额测试
    void test_getAuctionSgas()
    {
        MyJson.JsonNode_Object paparms = new MyJson.JsonNode_Object();
        var array = new MyJson.JsonNode_Array();
        array.AddArrayValue("(addr)AYTcTTB8jpWtGgs8ukoUrQPm1zmEFxZHNk");
        paparms["sbParamJson"] = array;
        paparms["sbPushString"] = new MyJson.JsonNode_ValueString("balanceOf");
        paparms["nnc"] = new MyJson.JsonNode_ValueString("0x7753e79cfb98e63e2b7aa00a819e0cb86fdb1930");

        NeoGameSDK_CS.invokescrept((bool timeout, WWW www) =>{
            var json = MyJson.Parse(www.text);
            if (!json.AsDict().ContainsKey("result"))
            {
                Debug.Log("无余额");
            }
            else
            {
                var resultv = json.AsDict()["result"].AsList()[0].AsDict()["stack"].AsList()[0].AsDict();
                var rtype = resultv["type"].AsString();
                var rvalue = resultv["value"].AsString();
                if (rvalue != "")
                {
                    var n = new BigMath.Numerics.BigInteger(ThinNeo.Helper.HexString2Bytes(rvalue));
                    Debug.Log("余额：" + decimal.Parse((float.Parse(n.ToString()) / 100000000).ToString()));
                }
                else
                {
                    Debug.Log("无余额");
                }
            }

        }, paparms);
    }

    //往市场合约里的充值测试
    void test_rechargeToken()
    {
        MyJson.JsonNode_Array paparms = new MyJson.JsonNode_Array();

        MyJson.JsonNode_Object paparms_1 = new MyJson.JsonNode_Object();
        var array = new MyJson.JsonNode_Array();
        array.AddArrayValue("(addr)AYTcTTB8jpWtGgs8ukoUrQPm1zmEFxZHNk");
        array.AddArrayValue("(addr)ALAD4J1b7PnkV23GrEquBjo8wqUfc6MGQf");
        array.AddArrayValue("(int)" + 100000000);
        paparms_1["sbParamJson"] = array;
        paparms_1["sbPushString"] = new MyJson.JsonNode_ValueString("transfer");
        paparms_1["nnc"] = new MyJson.JsonNode_ValueString("0x2761020e5e6dfcd8d37fdd50ff98fa0f93bccf54");

        MyJson.JsonNode_Object paparms_2 = new MyJson.JsonNode_Object();
        paparms_2["sbPushString"] = new MyJson.JsonNode_ValueString("rechargeToken");
        paparms_2["nnc"] = new MyJson.JsonNode_ValueString("0x7753e79cfb98e63e2b7aa00a819e0cb86fdb1930");

        paparms.AddArrayValue(paparms_1);
        paparms.AddArrayValue(paparms_2);

        NeoGameSDK_CS.makeRawTransaction((bool timeout, WWW www) => { Debug.Log(www.text); }, paparms);
    }



    // Update is called once per frame
    void Update () {
	
	}
}
