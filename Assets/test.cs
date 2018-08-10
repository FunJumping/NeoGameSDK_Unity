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

        //string[] array = { "(addr)ALAD4J1b7PnkV23GrEquBjo8wqUfc6MGQf" };
        //NeoGameSDK_CS.postMessagge(api_cb_cmd, "0xa251ae31ee3eca6fc521492b6cc7b2230d4b1cc3", "sendrawtransaction", "setAuctionAddr", array);
        //NeoGameSDK_CS.postMessagge(api_cb_cmd, "0xa251ae31ee3eca6fc521492b6cc7b2230d4b1cc3", "invokescript", "getAuctionAddr", null);


        //string[] array = { "(hex)0x2761020e5e6dfcd8d37fdd50ff98fa0f93bccf54" };
        //NeoGameSDK_CS.postMessagge(api_cb_cmd, "0x7753e79cfb98e63e2b7aa00a819e0cb86fdb1930", "sendrawtransaction", "_setSgas", array);


        //string[] array = { "(addr)ASwnAMZuyMGGE6ZkHhoHEyxRM2jPNeJsKE" };
        //NeoGameSDK_CS.postMessagge(api_cb_cmd, "0x7753e79cfb98e63e2b7aa00a819e0cb86fdb1930", "invokescript", "balanceOf", array);

        //string[] array = { "(addr)ASwnAMZuyMGGE6ZkHhoHEyxRM2jPNeJsKE", "(addr)ALAD4J1b7PnkV23GrEquBjo8wqUfc6MGQf", "(int)" + 100000000 };
        //NeoGameSDK_CS.postMessagge(api_cb_cmd, "0x2761020e5e6dfcd8d37fdd50ff98fa0f93bccf54", "sendrawtransaction", "transfer", array);

        //string[] array = { "(addr)" + roleInfo.getInstance().address, "(hex256)0x64af64cd56ce89e966b712ef1438779f0ce86b6d83406ae4fc5da3e33dbbd925" };
        //NeoGameSDK_CS.postMessagge(api_cb2, "0x7753e79cfb98e63e2b7aa00a819e0cb86fdb1930", "sendrawtransaction", "rechargeToken", array);


        //string[] array = { "(addr)ASwnAMZuyMGGE6ZkHhoHEyxRM2jPNeJsKE", "(addr)ASwnAMZuyMGGE6ZkHhoHEyxRM2jPNeJsKE",  "(int)1"};
        //NeoGameSDK_CS.postMessagge(api_cb_cmd, "0xa251ae31ee3eca6fc521492b6cc7b2230d4b1cc3", "sendrawtransaction", "transferFrom_app", array);

        //string[] array = { "(addr)ATqAGuPPgshgmiUWfMkyCQCAYnU4WhqtKG", "(int)2", "(int)1000000000000", "(int)10" };
        //NeoGameSDK_CS.postMessagge(api_cb_cmd, "0x7753e79cfb98e63e2b7aa00a819e0cb86fdb1930", "sendrawtransaction", "createSaleAuction", array);

        //string[] array = { "(addr)ATqAGuPPgshgmiUWfMkyCQCAYnU4WhqtKG", "(int)2"};
        //NeoGameSDK_CS.postMessagge(api_cb_cmd, "0x7753e79cfb98e63e2b7aa00a819e0cb86fdb1930", "sendrawtransaction", "cancelAuction", array);
    }
    void api_cb_pay(NeoGameSDK_pay_data one)
    {
        
    }

    void api_cb_lgoinout()
    {
        
    }

    void api_cb_cmd(bool timeout, WWW www)
    {
        Debug.Log(www.text);
    }
    void api_cb2(bool timeout, WWW www)
    {
        Debug.Log(www.text);
    }

    // Update is called once per frame
    void Update () {
	
	}
}
