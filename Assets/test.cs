using UnityEngine;
using System.Collections;

public class test : MonoBehaviour {

	void Start () {
        NeoGameSDK_CS.init(5, api_cb_login, api_cb_pay, api_cb_lgoinout);
        NeoGameSDK_CS.login();
	}

    void api_cb_login(NeoGameSDK_login_data one)
    {
        Debug.Log("uid:" + one.uid + " token:" + one.token + " wallet" + one.wallet);
    }
    void api_cb_pay(NeoGameSDK_pay_data one)
    {
        
    }

    void api_cb_lgoinout()
    {

    }

    // Update is called once per frame
    void Update () {
	
	}
}
