using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
public class api_tool : MonoBehaviour
{
    static public api_tool _instance = null;
    //黑猫后台请求地址
    string m_lancfg_hostUrl = "http://10.1.8.132/new/nel/api_c/apic_user.php";
    //string m_lancfg_hostUrl = "http://182.254.139.130/apic/";
    void Start()
    {
        _instance = this;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void isLogined(string uid, string token, Action<bool, WWW> call_back)
    {
        WWWForm www_form = new WWWForm();
        www_form.AddField("cmd", "user.isLogined");
        www_form.AddField("uid", uid);
        www_form.AddField("token", token);
        StartCoroutine(HTTP_post("/apic_user.php", www_form, call_back));
    }

    public void validPhone(string phone, Action<bool, WWW> call_back)
    {
        WWWForm www_form = new WWWForm();
        www_form.AddField("cmd", "user_phone.valid_register");
        www_form.AddField("phone", phone);
        StartCoroutine(HTTP_post("/apic_user.php", www_form, call_back));
    }
    public void validEmail(string email, Action<bool, WWW> call_back)
    {
        WWWForm www_form = new WWWForm();
        www_form.AddField("cmd", "user_email.valid_register");
        www_form.AddField("email", email);
        StartCoroutine(HTTP_post("/apic_user.php", www_form, call_back));
    }

    public void validUid(string uid, Action<bool, WWW> call_back)
    {
        WWWForm www_form = new WWWForm();
        www_form.AddField("cmd", "user.valid_register");
        www_form.AddField("uid", uid);
        StartCoroutine(HTTP_post("/apic_user.php", www_form, call_back));
    }

    public void getEmailCode(string email, string lang, Action<bool, WWW> call_back)
    {
        WWWForm www_form = new WWWForm();
        www_form.AddField("cmd", "user_email.get_code");
        www_form.AddField("email", email);
        www_form.AddField("lang", lang);
        StartCoroutine(HTTP_post("/apic_user.php", www_form, call_back));
    }

    public void getPhoneCode(string phone, Action<bool, WWW> call_back)
    {
        WWWForm www_form = new WWWForm();
        www_form.AddField("cmd", "user_phone.get_code");
        www_form.AddField("phone", phone);
        StartCoroutine(HTTP_post("/apic_user.php", www_form, call_back));
    }

    public void registerByPhone(string phone, string code, string passward, string region, string uid, Action<bool, WWW> call_back)
    {
        WWWForm www_form = new WWWForm();
        www_form.AddField("cmd", "user_phone.register_pass");
        www_form.AddField("phone", phone);
        www_form.AddField("code", code);
        www_form.AddField("pwd", passward);
        www_form.AddField("region", region);
        www_form.AddField("uid", uid);

        StartCoroutine(HTTP_post("/apic_user.php", www_form, call_back));
    }

    public void registerByEmail(string email, string code, string passward, string region, string uid, Action<bool, WWW> call_back)
    {
        WWWForm www_form = new WWWForm();
        www_form.AddField("cmd", "user_email.register_pass");
        www_form.AddField("email", email);
        www_form.AddField("code", code);
        www_form.AddField("pwd", passward);
        www_form.AddField("region", region);
        www_form.AddField("uid", uid);

        StartCoroutine(HTTP_post("/apic_user.php", www_form, call_back));
    }

    public void userLoginPass(string uid, string pwd, Action<bool, WWW> call_back)
    {
        WWWForm www_form = new WWWForm();
        www_form.AddField("cmd", "user.login");
        www_form.AddField("uid", uid);
        www_form.AddField("pwd", pwd);
        StartCoroutine(HTTP_post("/apic_user.php", www_form, call_back));
    }
    public void phoneLoginPass(string phone, string pwd, Action<bool, WWW> call_back)
    {
        WWWForm www_form = new WWWForm();
        www_form.AddField("cmd", "user_phone.login_pass");
        www_form.AddField("phone", phone);
        www_form.AddField("pwd", pwd);
        StartCoroutine(HTTP_post("/apic_user.php", www_form, call_back));
    }
    public void emailLoginPass(string email, string pwd, Action<bool, WWW> call_back)
    {
        WWWForm www_form = new WWWForm();
        www_form.AddField("cmd", "user_email.login_pass");
        www_form.AddField("email", email);
        www_form.AddField("pwd", pwd);
        StartCoroutine(HTTP_post("/apic_user.php", www_form, call_back));
    }

    public void bindWallet(string uid, string token, string wallet_json, Action<bool, WWW> call_back)
    {
        WWWForm www_form = new WWWForm();
        www_form.AddField("cmd", "user.bind_wallet");
        www_form.AddField("uid", uid);
        www_form.AddField("token", token);
        www_form.AddField("wallet", wallet_json);
        StartCoroutine(HTTP_post("/apic_user.php", www_form, call_back));
    }

    public void getWalletFile(string uid, string token, Action<bool, WWW> call_back)
    {
        WWWForm www_form = new WWWForm();
        www_form.AddField("cmd", "user.get_wallet_file");
        www_form.AddField("uid", uid);
        www_form.AddField("token", token);
        StartCoroutine(HTTP_post("/apic_user.php", www_form, call_back));
    }

    public void addUserWalletLogs(string uid, string token, string txid, string g_id, string cnts, string type, string pparams,int net_type,string trust, Action<bool, WWW> call_back)
    {
        WWWForm www_form = new WWWForm();
        www_form.AddField("cmd", "user_wallet.logss");
        www_form.AddField("uid", uid);
        www_form.AddField("token", token);
        www_form.AddField("txid", txid);
        www_form.AddField("g_id", g_id);
        www_form.AddField("cnts", cnts);
        www_form.AddField("type", type);
        www_form.AddField("params", pparams);
        www_form.AddField("net_type", net_type);
        www_form.AddField("trust", trust);
        StartCoroutine(HTTP_post("/apic_user.php", www_form, call_back));
    }

    public void getWalletListss(string uid, string token, int page, int num, int net_type, Action<bool, WWW> call_back)
    {
        WWWForm www_form = new WWWForm();
        www_form.AddField("cmd", "user_wallet.listss");
        www_form.AddField("uid", uid);
        www_form.AddField("token", token);
        www_form.AddField("page", page);
        www_form.AddField("num", num);
        www_form.AddField("net_type", net_type);

        StartCoroutine(HTTP_post("/apic_user.php", www_form, call_back));
    }

    public void walletNotify(string uid, string token, string txid, int net_type, Action<bool, WWW> call_back)
    {
        WWWForm www_form = new WWWForm();
        www_form.AddField("cmd", "user_wallet.notify");
        www_form.AddField("uid", uid);
        www_form.AddField("token", token);
        www_form.AddField("txid", txid);
        www_form.AddField("net_type", net_type);

        StartCoroutine(HTTP_post("/apic_user.php", www_form, call_back));
    }

    public void getAppWalletNotifys(string uid, string token, string g_id, int net_type, Action<bool, WWW> call_back)
    {
        WWWForm www_form = new WWWForm();
        www_form.AddField("cmd", "user_wallet.get_notify");
        www_form.AddField("uid", uid);
        www_form.AddField("token", token);
        www_form.AddField("g_id", g_id);
        www_form.AddField("net_type", net_type);

        StartCoroutine(HTTP_post("/apic_user.php", www_form, call_back));
    }

    public void getPlatWalletNotifys(string uid, string token, int net_type, Action<bool, WWW> call_back)
    {
        WWWForm www_form = new WWWForm();
        www_form.AddField("cmd", "user_wallet.get_notify_plat");
        www_form.AddField("uid", uid);
        www_form.AddField("token", token);
        www_form.AddField("net_type", net_type);

        StartCoroutine(HTTP_post("/apic_user.php", www_form, call_back));
    }

    public void walletNotifyExt(string uid, string token, string txid, string ext, int net_type, Action<bool, WWW> call_back)
    {
        WWWForm www_form = new WWWForm();
        www_form.AddField("cmd", "user_wallet.notify_ext");
        www_form.AddField("uid", uid);
        www_form.AddField("token", token);
        www_form.AddField("txid", txid);
        www_form.AddField("ext", ext);
        www_form.AddField("net_type", net_type);

        StartCoroutine(HTTP_post("/apic_user.php", www_form, call_back));
    }


    public void wallet_load(string file)
    {
        StartCoroutine(WaitLoadFile(file, on_wallet_load));
    }
    private void on_wallet_load(WWW www)
    {
        Debug.Log(www.text);
        roleInfo.getInstance().wallet_json = www.text;
    }
    IEnumerator WaitLoadFile(string fileName, Action<WWW> callback_wallet_load)
    {
        WWW www = new WWW("file://" + fileName);
        Debug.Log(www.url);
        yield return www;
        
        callback_wallet_load(www);
    }


    public bool check_www_result(WWW www)
    {
        Debug.Log(www.text);
        var r = MyJson.Parse(www.text).AsDict()["r"].AsInt();
        if (r == 0)
        {
            testtool.on_error_notice(MyJson.Parse(www.text).AsDict()["errCode"].AsInt());
            return false;
        }
        else if (r == 1)
            return true;
        else
            return false;
    }

    IEnumerator HTTP_post(string url, WWWForm www_form, Action<bool, WWW> call_back) //请求黑猫后台
    {
        WWW www = new WWW(m_lancfg_hostUrl + url, www_form);

        float timer = 0;
        bool time_out = false;
        while (!www.isDone)
        {
            if (timer > 30f) { time_out = true; break; }
            timer += Time.deltaTime;
            yield return null;
        }

        if (check_www_result(www) && call_back!= null)
            call_back(time_out, www);

        www.Dispose();
        www = null;
    }

}
