using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
public class sdk_http : MonoBehaviour
{
    static public sdk_http _instance = null;
    //黑猫后台请求地址
    string m_lancfg_hostUrl = "http://182.254.139.130/apic/";
    void Start()
    {
        _instance = this;
    }

    // Update is called once per frame
    void Update()
    {

    }

    Action<bool> ck_phone_login;
    Action<bool> ck_wallet_bind;
    Action<bool> ck_get_wallet_file;
    Action<int> ck_error_notice;
    Action ck_refresh_info;

    public void init(Action<int> callback_error_notice, Action callback_refresh_info)
    {
        ck_error_notice = callback_error_notice;
        ck_refresh_info = callback_refresh_info;
    }

    private bool check_www_result(WWW www)
    {
        Debug.Log(www.text);
        var r = MyJson.Parse(www.text).AsDict()["r"].AsInt();
        if (r == 0)
        {
            ck_error_notice(MyJson.Parse(www.text).AsDict()["errCode"].AsInt());
            return false;
        }
        else if (r == 1)
            return true;
        else
            return false;
    }

    public void phone_login(string phone, string code, Action<bool> callback_phone_login)
    {
        ck_phone_login = callback_phone_login;

        WWWForm www_form = new WWWForm();
        www_form.AddField("cmd", "user_phone.login");
        www_form.AddField("phone", phone);
        www_form.AddField("code", code);
        StartCoroutine(HTTP_post("/apic_user.php", www_form, on_phone_login));
    }

    public void wallet_load(string file)
    {
        StartCoroutine(WaitLoadFile(file, on_wallet_load));
    }

    public void wallet_bind(string wif, string json, Action<bool> callback_wallet_bind)
    {
        roleInfo.getInstance().m_wif = wif;
        roleInfo.getInstance().set_storage(wif);
        ck_wallet_bind = callback_wallet_bind;

        WWWForm www_form = new WWWForm();
        www_form.AddField("cmd", "user.bind_wallet");
        www_form.AddField("uid", roleInfo.getInstance().uid);
        www_form.AddField("token", roleInfo.getInstance().token);
        www_form.AddField("wallet", roleInfo.getInstance().wallet_json);
        StartCoroutine(HTTP_post("/apic_user.php", www_form, on_wallet_bind));
    }

    public void get_wallet_file(Action<bool> callback_get_wallet_file)
    {
        ck_get_wallet_file = callback_get_wallet_file;

        WWWForm www_form = new WWWForm();
        www_form.AddField("cmd", "user.get_wallet_file");
        www_form.AddField("uid", roleInfo.getInstance().uid);
        www_form.AddField("token", roleInfo.getInstance().token);
        StartCoroutine(HTTP_post("/apic_user.php", www_form, on_get_wallet_file));
    }

    public void pay_sgas(string txid)
    {
        WWWForm www_form = new WWWForm();
        www_form.AddField("cmd", "user_wallet.logss");
        www_form.AddField("uid", roleInfo.getInstance().uid);
        www_form.AddField("token", roleInfo.getInstance().token);
        www_form.AddField("txid", txid);
        //www_form.AddField("from", roleInfo.getInstance().address);
        //www_form.AddField("to", global.cp_adress);
        www_form.AddField("g_id", global.game_id);
        www_form.AddField("cnts", m_num.ToString());
        www_form.AddField("type", 3);
        www_form.AddField("params", m_paparms);
        Debug.Log(www_form.ToString());
        StartCoroutine(HTTP_post("/apic_user.php", www_form, on_pay_sgas));
    }

    public void get_main_info()
    {
        get_allasset();
    }

    public void get_allasset()
    {
        WWWForm www_form = Helper.GetWWWFormPost("getallasset", new MyJson.JsonNode_ValueString(roleInfo.getInstance().address));
        StartCoroutine(HTTP_nel_post(www_form, on_get_allasset));
    }

    public void get_balance()
    {
        WWWForm www_form = Helper.GetWWWFormPost("getbalance", new MyJson.JsonNode_ValueString(roleInfo.getInstance().address));
        StartCoroutine(HTTP_nel_post(www_form, on_get_balance));
    }

    public void get_sgas_balance()
    {
        {//查sgas balance
            string script = null;
            using (var sb = new ThinNeo.ScriptBuilder())
            {
                var array = new MyJson.JsonNode_Array();
                array.AddArrayValue("(bytes)" + ThinNeo.Helper.Bytes2HexString(roleInfo.getInstance().scripthash));
                sb.EmitParamJson(array);//参数倒序入
                sb.EmitParamJson(new MyJson.JsonNode_ValueString("(str)balanceOf"));//参数倒序入
                ThinNeo.Hash160 shash = new ThinNeo.Hash160(global.id_sgas);
                sb.EmitAppCall(shash);//nep5脚本
                var data = sb.ToArray();
                script = ThinNeo.Helper.Bytes2HexString(data);

            }

            WWWForm www_form = Helper.GetWWWFormPost("invokescript", new MyJson.JsonNode_ValueString(script));
            StartCoroutine(HTTP_nel_post(www_form, on_get_sgas_balance));
        }
    }
    public void get_allnep5assetofaddres()
    {
        WWWForm www_form = Helper.GetWWWFormPost("getallnep5assetofaddress", new MyJson.JsonNode_ValueString(roleInfo.getInstance().address), new MyJson.JsonNode_ValueNumber(1));
        StartCoroutine(HTTP_nel_post(www_form, on_get_allnep5assetofaddress));
    }

    private void on_get_allasset(bool timeout, WWW www)
    {
        Debug.Log(www.text);
        var json = MyJson.Parse(www.text).AsDict()["result"].AsList();
        get_balance();
    }

    private void on_get_sgas_balance(bool timeout, WWW www)
    {
        Debug.Log(www.text);
        var json = MyJson.Parse(www.text);

        if (!json.AsDict().ContainsKey("result"))
        {
            roleInfo.getInstance().sgas = 0;
        }
        else
        {
            var resultv = json.AsDict()["result"].AsList()[0].AsDict()["stack"].AsList()[0].AsDict();
            var rtype = resultv["type"].AsString();
            var rvalue = resultv["value"].AsString();
            if (rvalue != "")
            {
                var n = new BigMath.Numerics.BigInteger(ThinNeo.Helper.HexString2Bytes(rvalue));
                roleInfo.getInstance().sgas = decimal.Parse((float.Parse(n.ToString()) / 100000000).ToString());
            }
            else
            {
                roleInfo.getInstance().sgas = 0;
            }
        }

        //数据获取结束后刷到主界面上
        ck_refresh_info();
    }

    private void on_get_allnep5assetofaddress(bool timeout, WWW www)
    {
        Debug.Log(www.text);
        var json = MyJson.Parse(www.text);

        if (!json.AsDict().ContainsKey("result"))
        {
            roleInfo.getInstance().sgas = 0;
        }
        else
        {
            var json2 = json.AsDict()["result"].AsList();
            foreach (var item in json2)
            {
                //这个合约地址写死
                //global.id_sgas = item.AsDict()["assetid"].ToString();
                roleInfo.getInstance().sgas = decimal.Parse(item.AsDict()["balance"].ToString());
            }
        }


        //数据获取结束后刷到主界面上
        ck_refresh_info();
    }

    decimal m_num;
    string m_paparms;
    string m_cur_txid;
    public void recharge_sgas(decimal num)
    {
        m_num = num;
        WWWForm www_form = Helper.GetWWWFormPost("getutxo", new MyJson.JsonNode_ValueString(roleInfo.getInstance().address));
        StartCoroutine(HTTP_nel_post(www_form, on_getutxo));
    }

    public void recharge_game_sgas(decimal num)
    {
        m_num = num;
        WWWForm www_form = Helper.GetWWWFormPost("getutxo", new MyJson.JsonNode_ValueString(roleInfo.getInstance().address));
        StartCoroutine(HTTP_nel_post(www_form, on_getutxo_1));
    }

    public void transaction_sgas(string strtrandata)
    {

        WWWForm www_form = Helper.GetWWWFormPost("sendrawtransaction", new MyJson.JsonNode_ValueString(strtrandata));
        StartCoroutine(HTTP_nel_post(www_form, on_transaction_gas));
    }

    public void transaction_game_sgas(string strtrandata)
    {
        WWWForm www_form = Helper.GetWWWFormPost("sendrawtransaction", new MyJson.JsonNode_ValueString(strtrandata));
        StartCoroutine(HTTP_nel_post(www_form, on_transaction_game_gas));
    }

    private void on_getutxo_1(bool timeout, WWW www)
    {
        Debug.Log(www.text);
        MyJson.JsonNode_Object response = (MyJson.JsonNode_Object)MyJson.Parse(www.text);

        if (!response.AsDict().ContainsKey("result"))
        {
            Debug.Log("交易失败");
            return;
        }
        MyJson.JsonNode_Array resJA = (MyJson.JsonNode_Array)response["result"];
        Dictionary<string, List<Utxo>> _dir = new Dictionary<string, List<Utxo>>();
        foreach (MyJson.JsonNode_Object j in resJA)
        {
            Utxo utxo = new Utxo(j["addr"].ToString(), new ThinNeo.Hash256(j["txid"].ToString()), j["asset"].ToString(), decimal.Parse(j["value"].ToString()), int.Parse(j["n"].ToString()));
            if (_dir.ContainsKey(j["asset"].ToString()))
            {
                _dir[j["asset"].ToString()].Add(utxo);
            }
            else
            {
                List<Utxo> l = new List<Utxo>();
                l.Add(utxo);
                _dir[j["asset"].ToString()] = l;
            }
        }

        if (_dir.ContainsKey(global.id_GAS) == false)
        {
            Debug.Log("no gas");
        }
        ThinNeo.Transaction tran = null;
        {

            byte[] script = null;
            using (var sb = new ThinNeo.ScriptBuilder())
            {
                var array = new MyJson.JsonNode_Array();
                array.AddArrayValue("(addr)" + roleInfo.getInstance().address);//from
                array.AddArrayValue("(addr)" + global.cp_adress);//to
                array.AddArrayValue("(int)" + (m_num * 100000000));//value

                sb.EmitParamJson(array);//参数倒序入
                sb.EmitParamJson(new MyJson.JsonNode_ValueString("(str)transfer"));//参数倒序入
                ThinNeo.Hash160 shash = new ThinNeo.Hash160(global.id_sgas);
                sb.EmitAppCall(shash);//nep5脚本

                //拼接发给后台做验证的json字符串
                MyJson.JsonNode_Object account = new MyJson.JsonNode_Object();
                account["sbParamJson"] = array;
                account["sbPushString"] = new MyJson.JsonNode_ValueString("(str)transfer");
                account["nnc"] = new MyJson.JsonNode_ValueString(global.id_sgas);

                script = sb.ToArray();
               
                m_paparms = account.ToString();
                Debug.Log(m_paparms);
            }

            tran = Helper.makeTran(_dir[global.id_GAS], roleInfo.getInstance().address, new ThinNeo.Hash256(global.id_GAS), 0);
            tran.type = ThinNeo.TransactionType.InvocationTransaction;
            var idata = new ThinNeo.InvokeTransData();
            tran.extdata = idata;
            idata.script = script;
        
        }

        //sign and broadcast
        var signdata = ThinNeo.Helper.Sign(tran.GetMessage(), roleInfo.getInstance().prikey);
        tran.AddWitness(signdata, roleInfo.getInstance().pubkey, roleInfo.getInstance().address);
        var trandata = tran.GetRawData();
        var strtrandata = ThinNeo.Helper.Bytes2HexString(trandata);
        transaction_game_sgas(strtrandata);
    }

    private void on_transaction_game_gas(bool timeout, WWW www)
    {
        var json = MyJson.Parse(www.text).AsDict();

        if (json.ContainsKey("result"))
        {
            var resultv = json["result"].AsList()[0].AsDict();
            var txid = resultv["txid"].AsString();
            if (txid.Length > 0)
            {
                //Nep55_1.lastNep5Tran = tran.GetHash();

                Debug.Log("txid=" + txid);

                m_cur_txid = txid;
                pay_sgas(txid);
            }
            else
            {
                Debug.Log("交易失败");
            }
        }
        else
        {
            Debug.Log("交易失败");
            //testtool.showNotice("交易失败");
        }
    }
    private void on_getutxo(bool timeout, WWW www)
    {
        Debug.Log(www.text);
        MyJson.JsonNode_Object response = (MyJson.JsonNode_Object)MyJson.Parse(www.text);

        if (!response.AsDict().ContainsKey("result"))
        {
            Debug.Log("交易失败");
            //testtool.showNotice("交易失败");
            return;
        }

        MyJson.JsonNode_Array resJA = (MyJson.JsonNode_Array)response["result"];
        Dictionary<string, List<Utxo>> _dir = new Dictionary<string, List<Utxo>>();
        foreach (MyJson.JsonNode_Object j in resJA)
        {
            Utxo utxo = new Utxo(j["addr"].ToString(), new ThinNeo.Hash256(j["txid"].ToString()), j["asset"].ToString(), decimal.Parse(j["value"].ToString()), int.Parse(j["n"].ToString()));
            if (_dir.ContainsKey(j["asset"].ToString()))
            {
                _dir[j["asset"].ToString()].Add(utxo);
            }
            else
            {
                List<Utxo> l = new List<Utxo>();
                l.Add(utxo);
                _dir[j["asset"].ToString()] = l;
            }
        }

        if (_dir.ContainsKey(global.id_GAS) == false)
        {
            Debug.Log("no gas");
        }
        ThinNeo.Transaction tran = null;
        {
            byte[] script = null;
            using (var sb = new ThinNeo.ScriptBuilder())
            {
                var array = new MyJson.JsonNode_Array();
                sb.EmitParamJson(array);//参数倒序入
                sb.EmitParamJson(new MyJson.JsonNode_ValueString("(str)mintTokens"));//参数倒序入
                ThinNeo.Hash160 shash = new ThinNeo.Hash160(global.id_sgas);
                sb.EmitAppCall(shash);//nep5脚本
                script = sb.ToArray();
            }
            var nep5scripthash = new ThinNeo.Hash160(global.id_sgas);
            var targetaddr = ThinNeo.Helper.GetAddressFromScriptHash(nep5scripthash);
            Debug.Log("contract address=" + targetaddr);//往合约地址转账

            //生成交易
            tran = Helper.makeTran(_dir[global.id_GAS], targetaddr, new ThinNeo.Hash256(global.id_GAS), m_num);
            tran.type = ThinNeo.TransactionType.InvocationTransaction;
            var idata = new ThinNeo.InvokeTransData();
            tran.extdata = idata;
            idata.script = script;
        }

        //sign and broadcast
        var signdata = ThinNeo.Helper.Sign(tran.GetMessage(), roleInfo.getInstance().prikey);
        tran.AddWitness(signdata, roleInfo.getInstance().pubkey, roleInfo.getInstance().address);
        var trandata = tran.GetRawData();
        var strtrandata = ThinNeo.Helper.Bytes2HexString(trandata);

        transaction_sgas(strtrandata);
    }

    private void on_transaction_gas(bool timeout, WWW www)
    {
        var json = MyJson.Parse(www.text).AsDict();

        if (json.ContainsKey("result"))
        {
            var resultv = json["result"].AsList()[0].AsDict();
            var txid = resultv["txid"].AsString();
            if (txid.Length > 0)
            {
                //Nep55_1.lastNep5Tran = tran.GetHash();
            }
            Debug.Log("txid=" + txid);
        }
        else
        {
            Debug.Log("交易失败");
            //testtool.showNotice("交易失败");
        }
    }

    private void on_get_balance(bool timeout, WWW www)
    {
        Debug.Log(www.text);
        var json = MyJson.Parse(www.text);
        if (!json.AsDict().ContainsKey("result"))
        {
            roleInfo.getInstance().gas = 0;
        }
        else
        {
            var json2 = json.AsDict()["result"].AsList();
            foreach (var item in json2)
            {
                if (item.AsDict()["asset"].AsString() == global.id_GAS)
                {
                    roleInfo.getInstance().gas = decimal.Parse(item.AsDict()["balance"].ToString());
                }
            }
        }

        get_sgas_balance();

        //之前通过这个接口来查sgas余额
        //get_allnep5assetofaddres();
    }

    private void on_get_wallet_file(bool timeout, WWW www)
    {
        if (!check_www_result(www))
            return;
        roleInfo.getInstance().wallet_json = MyJson.Parse(www.text).AsDict()["data"].AsString();

        if (!PlayerPrefs.HasKey("wif") || 
            PlayerPrefs.GetString("wif") == "" ||
            PlayerPrefs.GetString("uid") != roleInfo.getInstance().uid)
        {//这里重新登入钱包
            ck_get_wallet_file(true);
        }
        else
        {
            roleInfo.getInstance().get_storage();
            get_main_info();

            ck_get_wallet_file(false);
        }
    }

    private void on_pay_sgas(bool timeout, WWW www)
    {
        if (!check_www_result(www))
            return;

        NeoGameSDK_pay_data one = new NeoGameSDK_pay_data();
        one.uid = roleInfo.getInstance().uid;
        one.token = roleInfo.getInstance().token;
        one.txid = m_cur_txid;
        one.cnts = m_num.ToString();
        //api的登入成功后回调函数
        NeoGameSDK_CS.api_cb_pay(one);
    }

    private void on_wallet_bind(bool timeout, WWW www)
    {
        if (!check_www_result(www))
            return;
        get_wallet_file(ck_wallet_bind);
    }

    private void on_phone_login(bool timeout, WWW www)
    {
        if (!check_www_result(www))
            return;

        var json = MyJson.Parse(www.text).AsDict()["data"].AsDict();
       
        roleInfo.getInstance().uid = json["uid"].AsString();
        roleInfo.getInstance().token = json["token"].AsString();
        roleInfo.getInstance().phone = json["phone"].AsString();
        roleInfo.getInstance().name = json["name"].AsString();
        roleInfo.getInstance().wallet = json["wallet"].AsString();
        roleInfo.getInstance().wallet_json = "";

        if (roleInfo.getInstance().wallet == "")
        {
            ck_phone_login(true);
        }
        else
        {
            ck_phone_login(false);
        }
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

        call_back(time_out, www);

        www.Dispose();
        www = null;
    }

    IEnumerator HTTP_nel_post(WWWForm www_form, Action<bool, WWW> call_back) //请求nel后台
    {
        WWW www = new WWW(global.api, www_form);

        float timer = 0;
        bool time_out = false;
        while (!www.isDone)
        {
            if (timer > 30f) { time_out = true; break; }
            timer += Time.deltaTime;
            yield return null;
        }

        call_back(time_out, www);

        www.Dispose();
        www = null;
    }
}
