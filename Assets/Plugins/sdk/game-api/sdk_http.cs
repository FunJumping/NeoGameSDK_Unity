using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
public class sdk_http : MonoBehaviour
{
    static public sdk_http _instance = null;
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

    private bool check_www_result(WWW www)
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

    public void api_post_cmd(Action<bool, WWW> api_cb_cmd, string ids, string cmd, string main, string[] arry = null)
    {
    
        byte[] script = null;
        using (var sb = new ThinNeo.ScriptBuilder())
        {
            var array = new MyJson.JsonNode_Array();
            if (arry != null)
            {
                foreach (string str in arry)
                {
                    array.AddArrayValue(str);
                }
            }
            
            sb.EmitParamJson(array);//参数倒序入
            sb.EmitParamJson(new MyJson.JsonNode_ValueString("(str)" + main));//参数倒序入
            ThinNeo.Hash160 shash = new ThinNeo.Hash160(ids);
            sb.EmitAppCall(shash);//nep5脚本
            script = sb.ToArray();
        }

        if (cmd == "sendrawtransaction")
        {
            StartCoroutine(HTTP_nel_post_tan(script, api_cb_cmd));
        }
        else
        {
            WWWForm www_form = Helper.GetWWWFormPost(cmd, new MyJson.JsonNode_ValueString(ThinNeo.Helper.Bytes2HexString(script)));
            StartCoroutine(HTTP_nel_post(www_form, api_cb_cmd));
        }
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
        testtool.panel_main.on_refresh_info();
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
        testtool.panel_main.on_refresh_info();
    }

    decimal m_num;
    string m_paparms;
    string m_cur_txid;
    public void recharge_sgas(decimal num)
    {
        StartCoroutine(HTTP_nel_post_gas_sgas(num, on_recharge_gas));
    }

    public void transform_gas(string adrr, decimal num)
    {
        StartCoroutine(HTTP_nel_post_tan_gas(adrr, num, on_transaction_gas));
    }

    public void recharge_game_sgas(decimal num)
    {
        m_num = num;
        StartCoroutine(HTTP_nel_post_tan_sgas( num, on_transaction_game_gas));
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
                Debug.Log("txid=" + txid);

                api_tool._instance.addUserWalletLogs(roleInfo.getInstance().uid,roleInfo.getInstance().token, txid,
                    global.game_id.ToString(), m_num.ToString(), "3", m_paparms, 2, "0", null);
            }
            else
            {
                Debug.Log("交易失败");
            }
        }
        else
        {
            Debug.Log("交易失败");
        }
    }
   

    private void on_recharge_gas(bool timeout, WWW www)
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

        if(call_back != null)
            call_back(time_out, www);

        www.Dispose();
        www = null;
    }

    IEnumerator HTTP_nel_post_tan(byte[] script, Action<bool, WWW> call_back)
    {
        WWWForm www_getuxo_form = Helper.GetWWWFormPost("getutxo", new MyJson.JsonNode_ValueString(roleInfo.getInstance().address));
        WWW www = new WWW(global.api, www_getuxo_form);
        yield return www;

        Dictionary<string, List<Utxo>> _dir = get_utxo(www.text);

        if (_dir.ContainsKey(global.id_GAS) == false)
        {
            Debug.Log("no gas");
            yield return null;
        }

        ThinNeo.Transaction tran = null;
        {
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

        WWWForm www_form_sendraw = Helper.GetWWWFormPost("sendrawtransaction", new MyJson.JsonNode_ValueString(strtrandata));
        StartCoroutine(HTTP_nel_post(www_form_sendraw, call_back));
    }

    IEnumerator HTTP_nel_post_tan_gas(string toaddr, decimal num, Action<bool, WWW> call_back)
    {
        WWWForm www_getuxo_form = Helper.GetWWWFormPost("getutxo", new MyJson.JsonNode_ValueString(roleInfo.getInstance().address));
        WWW www = new WWW(global.api, www_getuxo_form);
        yield return www;

        Dictionary<string, List<Utxo>> _dir = get_utxo(www.text);

        if (_dir.ContainsKey(global.id_GAS) == false)
        {
            Debug.Log("no gas");
            yield return null;
        }

        ThinNeo.Transaction tran = null;
        {
            tran = Helper.makeTran(_dir[global.id_GAS], toaddr, new ThinNeo.Hash256(global.id_GAS), num);
        }

        //sign and broadcast
        var signdata = ThinNeo.Helper.Sign(tran.GetMessage(), roleInfo.getInstance().prikey);
        tran.AddWitness(signdata, roleInfo.getInstance().pubkey, roleInfo.getInstance().address);
        var trandata = tran.GetRawData();
        var strtrandata = ThinNeo.Helper.Bytes2HexString(trandata);

        WWWForm www_form_sendraw = Helper.GetWWWFormPost("sendrawtransaction", new MyJson.JsonNode_ValueString(strtrandata));
        StartCoroutine(HTTP_nel_post(www_form_sendraw, call_back));
    }

    IEnumerator HTTP_nel_post_tan_sgas(decimal num, Action<bool, WWW> call_back)
    {
        WWWForm www_getuxo_form = Helper.GetWWWFormPost("getutxo", new MyJson.JsonNode_ValueString(roleInfo.getInstance().address));
        WWW www = new WWW(global.api, www_getuxo_form);
        yield return www;

        Dictionary<string, List<Utxo>> _dir = get_utxo(www.text);

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

        WWWForm www_form_sendraw = Helper.GetWWWFormPost("sendrawtransaction", new MyJson.JsonNode_ValueString(strtrandata));
        StartCoroutine(HTTP_nel_post(www_form_sendraw, call_back));
    }

    IEnumerator HTTP_nel_post_gas_sgas(decimal num, Action<bool, WWW> call_back)
    {
        WWWForm www_getuxo_form = Helper.GetWWWFormPost("getutxo", new MyJson.JsonNode_ValueString(roleInfo.getInstance().address));
        WWW www = new WWW(global.api, www_getuxo_form);
        yield return www;

        Dictionary<string, List<Utxo>> _dir = get_utxo(www.text);

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
            tran = Helper.makeTran(_dir[global.id_GAS], targetaddr, new ThinNeo.Hash256(global.id_GAS), num);
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

        WWWForm www_form_sendraw = Helper.GetWWWFormPost("sendrawtransaction", new MyJson.JsonNode_ValueString(strtrandata));
        StartCoroutine(HTTP_nel_post(www_form_sendraw, call_back));
    }


    public Dictionary<string, List<Utxo>> get_utxo(string txt)
    {
        MyJson.JsonNode_Object response = (MyJson.JsonNode_Object)MyJson.Parse(txt);

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
        return _dir;
    }
}
