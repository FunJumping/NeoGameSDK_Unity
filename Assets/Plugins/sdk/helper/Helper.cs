using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using UnityEngine;


public class Utxo
{
    //txid[n] 是utxo的属性
    public ThinNeo.Hash256 txid;
    public int n;

    //asset资产、addr 属于谁，value数额，这都是查出来的
    public string addr;
    public string asset;
    public decimal value;
    public Utxo(string _addr, ThinNeo.Hash256 _txid, string _asset, decimal _value, int _n)
    {
        this.addr = _addr;
        this.txid = _txid;
        this.asset = _asset;
        this.value = _value;
        this.n = _n;
    }
}
public class Helper : MonoBehaviour
{
    public static WWWForm GetWWWFormPost(string method, params MyJson.IJsonNode[] _params)
    {
        WWWForm www_form = new WWWForm();
        www_form.AddField("jsonrpc", "2.0");
        www_form.AddField("id", 1);
        www_form.AddField("method", method);

        StringBuilder sb = new StringBuilder();
        sb.Append("[");
        for (var i = 0; i < _params.Length; i++)
        {
            _params[i].ConvertToString(sb);
            if (i != _params.Length - 1)
                sb.Append(",");
        }
        sb.Append("]");

        www_form.AddField("params", sb.ToString());
        return www_form;
    }

    public static ThinNeo.Transaction makeTran(List<Utxo> utxos, string targetaddr, ThinNeo.Hash256 assetid, decimal sendcount)
    {
        var tran = new ThinNeo.Transaction();
        tran.type = ThinNeo.TransactionType.ContractTransaction;
        tran.version = 0;//0 or 1
        tran.extdata = null;

        tran.attributes = new ThinNeo.Attribute[0];
        var scraddr = "";
        utxos.Sort((a, b) =>
        {
            if (a.value > b.value)
                return 1;
            else if (a.value < b.value)
                return -1;
            else
                return 0;
        });
        decimal count = decimal.Zero;
        List<ThinNeo.TransactionInput> list_inputs = new List<ThinNeo.TransactionInput>();
        for (var i = 0; i < utxos.Count; i++)
        {
            ThinNeo.TransactionInput input = new ThinNeo.TransactionInput();
            input.hash = utxos[i].txid;
            input.index = (ushort)utxos[i].n;
            list_inputs.Add(input);
            count += utxos[i].value;
            scraddr = utxos[i].addr;
            if (count >= sendcount)
            {
                break;
            }
        }
        tran.inputs = list_inputs.ToArray();
        if (count >= sendcount)//输入大于等于输出
        {
            List<ThinNeo.TransactionOutput> list_outputs = new List<ThinNeo.TransactionOutput>();
            //输出
            if (sendcount > decimal.Zero && targetaddr != null)
            {
                ThinNeo.TransactionOutput output = new ThinNeo.TransactionOutput();
                output.assetId = assetid;
                output.value = sendcount;
                output.toAddress = ThinNeo.Helper.GetPublicKeyHashFromAddress(targetaddr);
                list_outputs.Add(output);
            }

            //找零
            var change = count - sendcount;
            if (change > decimal.Zero)
            {
                ThinNeo.TransactionOutput outputchange = new ThinNeo.TransactionOutput();
                outputchange.toAddress = ThinNeo.Helper.GetPublicKeyHashFromAddress(scraddr);
                outputchange.value = change;
                outputchange.assetId = assetid;
                list_outputs.Add(outputchange);

            }
            tran.outputs = list_outputs.ToArray();
        }
        else
        {
            throw new Exception("no enough money.");
        }
        return tran;
    }
}
    