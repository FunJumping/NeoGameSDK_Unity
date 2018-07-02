using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class roleInfo
{
    private static roleInfo _instance;

    public string uid;
    public string token;
    public string lastlogin;
    public string name;
    public string phone;
    public string wallet;
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
        PlayerPrefs.SetString("phone", phone);
        PlayerPrefs.SetString("wif", wif);

        roleInfo.getInstance().prikey = ThinNeo.Helper.GetPrivateKeyFromWIF(wif);
        roleInfo.getInstance().pubkey = ThinNeo.Helper.GetPublicKeyFromPrivateKey(roleInfo.getInstance().prikey);
        roleInfo.getInstance().address = ThinNeo.Helper.GetAddressFromPublicKey(roleInfo.getInstance().pubkey);
        roleInfo.getInstance().scripthash = ThinNeo.Helper.GetPublicKeyHashFromAddress(roleInfo.getInstance().address);
    }

    public void dispose_storage()
    {
        PlayerPrefs.DeleteKey("uid");
        PlayerPrefs.DeleteKey("phone");
        PlayerPrefs.DeleteKey("wif");
    }
}
