using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class roleInfo
{
    private static roleInfo _instance;

    public string key_info = "BC_userinfo";

    public string uid;
    public string token;
    public string lastlogin;
    public string name;
    public string phone;
    public string wallet;
    public string area;
    public string region;
    public string qq;
    public string icon;
    public string sex;


    public string wallet_json;

    public ThinNeo.NEP6.NEP6Wallet nep6_wallet;

    public string m_wif;
    public byte[] prikey;
    public byte[] pubkey;
    public byte[] scripthash;
    public string address;

    public decimal gas;
    public decimal sgas;

    public static roleInfo getInstance()
    {
        if (_instance == null)
        {
            if (_instance == null)
            {
                _instance = new roleInfo();
            }
        }
        return _instance;
    }

    public void get_info()
    {
        string data = PlayerPrefs.GetString(key_info);

        if (data == "")
            return;

        var json = MyJson.Parse(data).AsDict();
        roleInfo.getInstance().uid = json["uid"].AsString();
        roleInfo.getInstance().token = json["token"].AsString();
        roleInfo.getInstance().phone = json["phone"].AsString();
        roleInfo.getInstance().phone = json["email"].AsString();

        roleInfo.getInstance().name = global.unicodeToStr(json["name"].AsString());
        roleInfo.getInstance().wallet = json["wallet"].AsString();
        roleInfo.getInstance().area = json["area"].AsString();
        roleInfo.getInstance().region = json["region"].AsString();
        roleInfo.getInstance().qq = json["qq"].AsString();
        roleInfo.getInstance().icon = json["icon"].AsString();
        roleInfo.getInstance().sex = json["sex"].AsString();
    }

    public void logined_cb()
    {
        NeoGameSDK_login_data one = new NeoGameSDK_login_data();
        one.uid = roleInfo.getInstance().uid;
        one.token = roleInfo.getInstance().token;
        one.lastlogin = roleInfo.getInstance().lastlogin;
        one.wallet = roleInfo.getInstance().wallet;
        //api的登入成功后回调函数
        NeoGameSDK_CS.api_cb_login(one);
    }

    public void get_storage()
    {
        string wif = PlayerPrefs.GetString("wif");

        roleInfo.getInstance().prikey = ThinNeo.Helper.GetPrivateKeyFromWIF(wif);
        roleInfo.getInstance().pubkey = ThinNeo.Helper.GetPublicKeyFromPrivateKey(roleInfo.getInstance().prikey);
        roleInfo.getInstance().address = ThinNeo.Helper.GetAddressFromPublicKey(roleInfo.getInstance().pubkey);
        roleInfo.getInstance().scripthash = ThinNeo.Helper.GetPublicKeyHashFromAddress(roleInfo.getInstance().address);
    }
    public void set_storage(string wif)
    {
        PlayerPrefs.SetString("uid", uid);
        PlayerPrefs.SetString("token",token);
        PlayerPrefs.SetString("wif", wif);

        roleInfo.getInstance().prikey = ThinNeo.Helper.GetPrivateKeyFromWIF(wif);
        roleInfo.getInstance().pubkey = ThinNeo.Helper.GetPublicKeyFromPrivateKey(roleInfo.getInstance().prikey);
        roleInfo.getInstance().address = ThinNeo.Helper.GetAddressFromPublicKey(roleInfo.getInstance().pubkey);
        roleInfo.getInstance().scripthash = ThinNeo.Helper.GetPublicKeyHashFromAddress(roleInfo.getInstance().address);
    }

    public void dispose_storage()
    {
        PlayerPrefs.DeleteKey("uid");
        PlayerPrefs.DeleteKey("token");
        PlayerPrefs.DeleteKey("wif");
    }
}
