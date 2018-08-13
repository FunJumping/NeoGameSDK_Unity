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

    public void invokescrept(Action<bool, WWW> api_cb_cmd, MyJson.JsonNode_Object paparms)
    {
        byte[] script = null;
        using (var sb = new ThinNeo.ScriptBuilder())
        {
            sb.EmitParamJson(paparms["sbParamJson"]);//参数倒序入
            sb.EmitPushString(paparms["sbPushString"].ToString());//参数倒序入
            ThinNeo.Hash160 shash = new ThinNeo.Hash160(paparms["nnc"].ToString());
            sb.EmitAppCall(shash);//nep5脚本
            script = sb.ToArray();
        }

        WWWForm www_form = Helper.GetWWWFormPost("invokescript", new MyJson.JsonNode_ValueString(ThinNeo.Helper.Bytes2HexString(script)));
        StartCoroutine(HTTP_nel_post(www_form, api_cb_cmd));
    }

    public void makeRawTransaction(Action<bool, WWW> api_cb_cmd, MyJson.JsonNode_Array pararms)
    {
        if (pararms.Count > 2)
        {
            Debug.Log("不支持大于两部的操作");
            return;
        }

        byte[] script = null;
        using (var sb = new ThinNeo.ScriptBuilder())
        {
            Debug.Log(pararms.AsList()[0].ToString());
            sb.EmitParamJson(pararms.AsList()[0].AsDict()["sbParamJson"]);//参数倒序入
            sb.EmitPushString(pararms.AsList()[0].AsDict()["sbPushString"].ToString());//参数倒序入
            ThinNeo.Hash160 shash = new ThinNeo.Hash160(pararms.AsList()[0].AsDict()["nnc"].ToString());
            sb.EmitAppCall(shash);//nep5脚本

            if (pararms.Count > 1)
            {
                //这个方法是为了在同一笔交易中转账并充值
                //当然你也可以分为两笔交易
                //插入下述两条语句，能得到txid
                sb.EmitSysCall("System.ExecutionEngine.GetScriptContainer");
                sb.EmitSysCall("Neo.Transaction.GetHash");

                sb.EmitPushBytes(roleInfo.getInstance().scripthash);
                sb.EmitPushNumber(2);
                sb.Emit(ThinNeo.VM.OpCode.PACK);

                sb.EmitPushString(pararms.AsList()[1].AsDict()["sbPushString"].ToString());//参数倒序入
                ThinNeo.Hash160 shash_2 = new ThinNeo.Hash160(pararms.AsList()[1].AsDict()["nnc"].ToString());
                sb.EmitAppCall(shash_2);//nep5脚本
            }

            script = sb.ToArray();
        }

        StartCoroutine(HTTP_nel_post_tan(script, api_cb_cmd));
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

    public void transform_gas_sgas(decimal num)
    {
        StartCoroutine(HTTP_nel_post_gas_sgas(num));
    }

    public void transform_gas(string adrr, decimal num)
    {
        StartCoroutine(HTTP_nel_post_tan_gas(adrr, num));
    }

    public void transform_sgas_gas(decimal num)
    {
        StartCoroutine(HTTP_nel_post_sgas_gas(num));
    }

    public void transform_sgas_gas2(MyJson.IJsonNode json_complete)
    {
        StartCoroutine(HTTP_nel_post_sgas_gas2(json_complete));
    }

    public void recharge_game_sgas(decimal num)
    {
        StartCoroutine(HTTP_nel_post_tan_sgas(num));
    }

    public void getrawtransaction(string txid, Action<bool, WWW> call_back)
    {
        WWWForm www_form = Helper.GetWWWFormPost("getrawtransaction", new MyJson.JsonNode_ValueString(txid));
        StartCoroutine(HTTP_nel_post_timeCircle(www_form, call_back));
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

    IEnumerator HTTP_nel_post_timeCircle(WWWForm www_form, Action<bool, WWW> call_back)
    {
        float timer = 11;

        while (true)
        {
            if (timer > 10f)
            {
                WWW www = new WWW(global.api, www_form);
                yield return www;
                Debug.Log(www.text);
                bool time_out = false;
                var json = MyJson.Parse(www.text).AsDict();

                if (json.ContainsKey("result"))
                {
                    if (call_back != null)
                        call_back(time_out, www);
                    break;
                }
                else
                {
                    timer = 0;
                }
            }
            timer += Time.deltaTime;
            yield return null;
        }
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

    IEnumerator HTTP_nel_post_tan_gas(string toaddr, decimal num)
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

        string paparms;

        //拼接发给后台做验证的json字符串
        MyJson.JsonNode_Object account = new MyJson.JsonNode_Object();
        account["sbPushString"] = new MyJson.JsonNode_ValueString("transfer");
        account["cnts"] = new MyJson.JsonNode_ValueString(num.ToString());

        paparms = account.ToString();
        Debug.Log(paparms);

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
        WWW sendraw = new WWW(global.api, www_form_sendraw);
        yield return sendraw;

        var json = MyJson.Parse(sendraw.text).AsDict();

        if (json.ContainsKey("result"))
        {
            var resultv = json["result"].AsList()[0].AsDict();
            var txid = resultv["txid"].AsString();
            if (txid.Length > 0)
            {
                //Nep55_1.lastNep5Tran = tran.GetHash();
            }
            Debug.Log("txid=" + txid);

            api_tool._instance.addUserWalletLogs(roleInfo.getInstance().uid, roleInfo.getInstance().token, txid,
                  "0", num.ToString(), "6", paparms, global.netType, "0",
                  (bool timeout1, WWW www1) => { testtool.panel_main.on_refresh_WalletListss(); });
        }
        else
        {
            Debug.Log("交易失败");
            //testtool.showNotice("交易失败");
        }
    }

    IEnumerator HTTP_nel_post_tan_sgas(decimal num)
    {
        WWWForm www_getuxo_form = Helper.GetWWWFormPost("getutxo", new MyJson.JsonNode_ValueString(roleInfo.getInstance().address));
        WWW www = new WWW(global.api, www_getuxo_form);
        yield return www;

        Dictionary<string, List<Utxo>> _dir = get_utxo(www.text);

        if (_dir.ContainsKey(global.id_GAS) == false)
        {
            Debug.Log("no gas");
        }

        string paparms;

        ThinNeo.Transaction tran = null;
        {

            byte[] script = null;
            using (var sb = new ThinNeo.ScriptBuilder())
            {
                var array = new MyJson.JsonNode_Array();
                array.AddArrayValue("(addr)" + roleInfo.getInstance().address);//from
                array.AddArrayValue("(addr)" + global.cp_adress);//to
                array.AddArrayValue("(int)" + (num * 100000000));//value

                sb.EmitParamJson(array);//参数倒序入
                sb.EmitParamJson(new MyJson.JsonNode_ValueString("(str)transfer"));//参数倒序入
                ThinNeo.Hash160 shash = new ThinNeo.Hash160(global.id_sgas);
                sb.EmitAppCall(shash);//nep5脚本

                //拼接发给后台做验证的json字符串
                MyJson.JsonNode_Object account = new MyJson.JsonNode_Object();
                account["sbParamJson"] = array;
                account["sbPushString"] = new MyJson.JsonNode_ValueString("transfer");
                account["nnc"] = new MyJson.JsonNode_ValueString(global.id_sgas);
                account["cnts"] = new MyJson.JsonNode_ValueString(num.ToString());
                paparms = account.ToString();


                script = sb.ToArray();
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
        WWW sendraw = new WWW(global.api, www_form_sendraw);
        yield return sendraw;

        var json = MyJson.Parse(sendraw.text).AsDict();

        if (json.ContainsKey("result"))
        {
            var resultv = json["result"].AsList()[0].AsDict();
            var txid = resultv["txid"].AsString();
            if (txid.Length > 0)
            {
                Debug.Log("txid=" + txid);

                api_tool._instance.addUserWalletLogs(roleInfo.getInstance().uid, roleInfo.getInstance().token, txid,
                    global.game_id.ToString(), num.ToString(), "3", paparms, global.netType, "0",
                    (bool timeout1, WWW www1) => { testtool.panel_main.on_refresh_WalletListss(); });
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

    IEnumerator HTTP_nel_post_gas_sgas(decimal num)
    {
        WWWForm www_getuxo_form = Helper.GetWWWFormPost("getutxo", new MyJson.JsonNode_ValueString(roleInfo.getInstance().address));
        WWW www = new WWW(global.api, www_getuxo_form);
        yield return www;

        Dictionary<string, List<Utxo>> _dir = get_utxo(www.text);

        if (_dir.ContainsKey(global.id_GAS) == false)
        {
            Debug.Log("no gas");
        }

        string paparms;

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

                //拼接发给后台做验证的json字符串
                MyJson.JsonNode_Object account = new MyJson.JsonNode_Object();
                account["sbParamJson"] = array;
                account["sbPushString"] = new MyJson.JsonNode_ValueString("mintTokens");
                account["nnc"] = new MyJson.JsonNode_ValueString(global.id_sgas);
                account["cnts"] = new MyJson.JsonNode_ValueString(num.ToString());
                paparms = account.ToString();
                Debug.Log(paparms);
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
        WWW sendraw = new WWW(global.api, www_form_sendraw);
        yield return sendraw;

        var json = MyJson.Parse(sendraw.text).AsDict();

        if (json.ContainsKey("result"))
        {
            var resultv = json["result"].AsList()[0].AsDict();
            var txid = resultv["txid"].AsString();
            if (txid.Length > 0)
            {
                Debug.Log("txid=" + txid);

                api_tool._instance.addUserWalletLogs(roleInfo.getInstance().uid, roleInfo.getInstance().token, txid,
                    "0", num.ToString(), "1", paparms, global.netType, "0",
                     (bool timeout1, WWW www1) => { testtool.panel_main.on_refresh_WalletListss(); });
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


    IEnumerator HTTP_nel_post_sgas_gas(decimal num)
    {
        var nep55_shash = new ThinNeo.Hash160(global.id_sgas);
        string nep55_address = ThinNeo.Helper.GetAddressFromScriptHash(nep55_shash);

        WWWForm www_getuxo_form = Helper.GetWWWFormPost("getutxo", new MyJson.JsonNode_ValueString(nep55_address));
        WWW www = new WWW(global.api, www_getuxo_form);
        yield return www;

        Dictionary<string, List<Utxo>> _dir = get_utxo(www.text);

        if (_dir.ContainsKey(global.id_GAS) == false)
        {
            Debug.Log("no gas");
        }

        List<Utxo> newlist = new List<Utxo>(_dir[global.id_GAS]);
        for (var i = newlist.Count - 1; i >= 0; i--)
        {
            Debug.Log(newlist[i].txid.ToString());
            
            byte[] script = null;
            using (var sb = new ThinNeo.ScriptBuilder())
            {
                var array = new MyJson.JsonNode_Array();
                array.AddArrayValue("(hex256)" + newlist[i].txid.ToString());
                sb.EmitParamJson(array);//参数倒序入
                sb.EmitParamJson(new MyJson.JsonNode_ValueString("(str)getRefundTarget"));//参数倒序入
                var shash = new ThinNeo.Hash160(global.id_sgas);
                sb.EmitAppCall(shash);//nep5脚本
                script = sb.ToArray();
            }
            if (newlist[i].n > 0)
                continue;

            WWWForm www_getTarget_form = Helper.GetWWWFormPost("invokescript", new MyJson.JsonNode_ValueString(ThinNeo.Helper.Bytes2HexString(script)));
            WWW www_form = new WWW(global.api, www_getTarget_form);

            yield return www_form;
            var jsonCU = MyJson.Parse(www_form.text);
            var stack = jsonCU.AsDict()["result"].AsList()[0].AsDict()["stack"].AsList()[0].AsDict();
            var value = stack["value"].AsString();
            if (value.Length > 0)//已经标记的UTXO，不能使用
            {
                newlist.RemoveAt(i);
            }
        }

        string paparms;

        ThinNeo.Transaction tran = null;
        {
            byte[] script = null;
            using (var sb = new ThinNeo.ScriptBuilder())
            {
                var array = new MyJson.JsonNode_Array();
                array.AddArrayValue("(bytes)" + ThinNeo.Helper.Bytes2HexString(roleInfo.getInstance().scripthash));
                sb.EmitParamJson(array);//参数倒序入
                sb.EmitParamJson(new MyJson.JsonNode_ValueString("(str)refund"));//参数倒序入
                var shash = new ThinNeo.Hash160(global.id_sgas);
                sb.EmitAppCall(shash);//nep5脚本
                script = sb.ToArray();


                //拼接发给后台做验证的json字符串
                MyJson.JsonNode_Object account = new MyJson.JsonNode_Object();
                account["sbParamJson"] = array;
                account["sbPushString"] = new MyJson.JsonNode_ValueString("refund");
                account["nnc"] = new MyJson.JsonNode_ValueString(global.id_sgas);
                account["cnts"] = new MyJson.JsonNode_ValueString(num.ToString());
                paparms = account.ToString();

            }

            //生成交易
            tran = Helper.makeTran(newlist, nep55_address, new ThinNeo.Hash256(global.id_GAS), num);
            tran.type = ThinNeo.TransactionType.InvocationTransaction;
            var idata = new ThinNeo.InvokeTransData();
            tran.extdata = idata;
            idata.script = script;

            //附加鉴证
            tran.attributes = new ThinNeo.Attribute[1];
            tran.attributes[0] = new ThinNeo.Attribute();
            tran.attributes[0].usage = ThinNeo.TransactionAttributeUsage.Script;
            tran.attributes[0].data = roleInfo.getInstance().scripthash;
        }

        //sign and broadcast
        {//做智能合约的签名
            byte[] n55contract = null;
            {
                WWWForm www_getState = Helper.GetWWWFormPost("getcontractstate", new MyJson.JsonNode_ValueString(global.id_sgas));
                WWW www_state = new WWW(global.api, www_getState);

                yield return www_state;

                var _json = MyJson.Parse(www_state.text).AsDict();
                var _resultv = _json["result"].AsList()[0].AsDict();
                n55contract = ThinNeo.Helper.HexString2Bytes(_resultv["script"].AsString());
            }
            byte[] iscript = null;
            using (var sb = new ThinNeo.ScriptBuilder())
            {
                sb.EmitPushString("whatever");
                sb.EmitPushNumber(250);
                iscript = sb.ToArray();
            }
            tran.AddWitnessScript(n55contract, iscript);
        }
        {//做提款人的签名
            var signdata = ThinNeo.Helper.Sign(tran.GetMessage(), roleInfo.getInstance().prikey);
            tran.AddWitness(signdata, roleInfo.getInstance().pubkey, roleInfo.getInstance().address);
        }
        var trandata = tran.GetRawData();
        var strtrandata = ThinNeo.Helper.Bytes2HexString(trandata);

        ThinNeo.Transaction testde = new ThinNeo.Transaction();
        testde.Deserialize(new System.IO.MemoryStream(trandata));

        WWWForm www_transaction = Helper.GetWWWFormPost("sendrawtransaction", new MyJson.JsonNode_ValueString(strtrandata));
        WWW www_tran = new WWW(global.api, www_transaction);

        yield return www_tran;


        Debug.Log(www_tran.text);

        var json = MyJson.Parse(www_tran.text).AsDict();

        ThinNeo.Hash256 lasttxid = null;
        if (json.ContainsKey("result"))
        {
            bool bSucc = false;
            if (json["result"].type == MyJson.jsontype.Value_Number)
            {
                bSucc = json["result"].AsBool();
                Debug.Log("cli=" + json["result"].ToString());
            }
            else
            {
                var resultv = json["result"].AsList()[0].AsDict();
                var txid = resultv["txid"].AsString();
                bSucc = txid.Length > 0;
            }
            if (bSucc)
            {
                lasttxid = tran.GetHash();
                Debug.Log("你可以从这个UTXO拿走GAS了 txid=" + lasttxid.ToString() + "[0]");

                api_tool._instance.addUserWalletLogs(roleInfo.getInstance().uid, roleInfo.getInstance().token, lasttxid.ToString(),
                   "0", num.ToString(), "2", paparms, global.netType, "0",
                    (bool timeout1, WWW www1) => { testtool.panel_main.on_refresh_WalletListss(); testtool.panel_main.on_refresh_plat_NotifyList(); });
            }
            else
            {
                lasttxid = null;
            }
        }

       
    }

    IEnumerator HTTP_nel_post_sgas_gas2(MyJson.IJsonNode json_complete)
    {
        string txid = json_complete.AsDict()["txid"].ToString();

        var nep55_shash = new ThinNeo.Hash160(global.id_sgas);
        string nep55_address = ThinNeo.Helper.GetAddressFromScriptHash(nep55_shash);

        WWWForm www_getuxo_form_2 = Helper.GetWWWFormPost("getutxo", new MyJson.JsonNode_ValueString(nep55_address));
        WWW www_utxo_2 = new WWW(global.api, www_getuxo_form_2);
        yield return www_utxo_2;

        Dictionary<string, List<Utxo>> _dir_2 = get_utxo(www_utxo_2.text);

        if (_dir_2.ContainsKey(global.id_GAS) == false)
        {
            Debug.Log("no gas");
        }

        List<Utxo> newlist_2 = new List<Utxo>();
        foreach (var utxo in _dir_2[global.id_GAS])
        {
            if (utxo.n == 0 && utxo.txid.ToString().Equals(txid))
                newlist_2.Add(utxo);
        }
        if (newlist_2.Count == 0)
        {
            Debug.Log("找不到要使用的UTXO");
            yield return null;
        }

        {//检查utxo
            byte[] script = null;
            using (var sb = new ThinNeo.ScriptBuilder())
            {
                var array = new MyJson.JsonNode_Array();
                array.AddArrayValue("(hex256)" + newlist_2[0].txid.ToString());
                sb.EmitParamJson(array);//参数倒序入
                sb.EmitParamJson(new MyJson.JsonNode_ValueString("(str)getRefundTarget"));//参数倒序入
                var shash = new ThinNeo.Hash160(global.id_sgas);
                sb.EmitAppCall(shash);//nep5脚本
                script = sb.ToArray();
            }

            WWWForm www_getTarget_form = Helper.GetWWWFormPost("invokescript", new MyJson.JsonNode_ValueString(ThinNeo.Helper.Bytes2HexString(script)));
            WWW www_form = new WWW(global.api, www_getTarget_form);
            yield return www_form;

            var jsonCU = MyJson.Parse(www_form.text);
            var stack = jsonCU.AsDict()["result"].AsList()[0].AsDict()["stack"].AsList()[0].AsDict();
            var value = stack["value"].AsString();
            if (value.Length == 0)//未标记的UTXO，不能使用
            {
                Debug.Log("这个utxo没有标记");
                yield return null;
            }
            
            var hash = new ThinNeo.Hash160(ThinNeo.Helper.HexString2Bytes(value));
            if (hash.ToString() != ThinNeo.Helper.GetPublicKeyHashFromAddress(roleInfo.getInstance().address).ToString())
            {
                Debug.Log("这个utxo不是标记给你用的");
                Debug.Log(hash.ToString());
                Debug.Log(ThinNeo.Helper.Bytes2HexString(roleInfo.getInstance().scripthash));
                yield return null;
            }
        }


        ThinNeo.Transaction tran_2 = Helper.makeTran(newlist_2, roleInfo.getInstance().address, new ThinNeo.Hash256(global.id_GAS), newlist_2[0].value);
        tran_2.type = ThinNeo.TransactionType.ContractTransaction;
        tran_2.version = 0;

        //sign and broadcast
        {//做智能合约的签名
            byte[] n55contract = null;
            {
                WWWForm www_getState = Helper.GetWWWFormPost("getcontractstate", new MyJson.JsonNode_ValueString(global.id_sgas));
                WWW www_state = new WWW(global.api, www_getState);
                yield return www_state;

                var _json = MyJson.Parse(www_state.text).AsDict();
                var _resultv = _json["result"].AsList()[0].AsDict();
                n55contract = ThinNeo.Helper.HexString2Bytes(_resultv["script"].AsString());
            }
            byte[] iscript = null;
            using (var sb = new ThinNeo.ScriptBuilder())
            {
                sb.EmitPushNumber(0);
                sb.EmitPushNumber(0);
                iscript = sb.ToArray();
            }
            tran_2.AddWitnessScript(n55contract, iscript);
        }


        var trandata_2 = tran_2.GetRawData();
        var strtrandata_2 = ThinNeo.Helper.Bytes2HexString(trandata_2);

        ThinNeo.Transaction testde_2 = new ThinNeo.Transaction();
        testde_2.Deserialize(new System.IO.MemoryStream(trandata_2));

        WWWForm www_transaction_2 = Helper.GetWWWFormPost("sendrawtransaction", new MyJson.JsonNode_ValueString(strtrandata_2));
        WWW www_tran_2 = new WWW(global.api, www_transaction_2);

        yield return www_tran_2;

        Debug.Log(www_tran_2.text);

        var json_2 = MyJson.Parse(www_tran_2.text).AsDict();

        if (json_2.ContainsKey("result"))
        {
            var resultv = json_2["result"].AsList()[0].AsDict();
            var ext = resultv["txid"].AsString();
            if (txid.Length > 0)
            {
                Debug.Log("txid=" + ext);
                testtool.panel_main.do_plat_notify(json_complete, ext);
                api_tool._instance.walletNotifyExt(roleInfo.getInstance().uid, roleInfo.getInstance().token, txid, ext, global.netType, null);
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
