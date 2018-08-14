using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ui_main : basePanel
{
    Text txt_name;
    Text txt_address;
    Text txt_gas;
    Text txt_sgas;

    GameObject itemListView;
    GridLayoutGroup item_Parent;
    Dictionary<string, GameObject> m_itemList = new Dictionary<string, GameObject>();

    MyJson.JsonNode_Array m_json = new MyJson.JsonNode_Array();
    Dictionary<string, int> m_platNotifyTxids = new Dictionary<string, int>();
    
    public ui_main(Transform go)
    {
        m_panel = go;

        txt_name = m_panel.FindChild("panel_card/name").GetComponent<Text>();
        txt_address = m_panel.FindChild("panel_card/address").GetComponent<Text>();
        txt_gas = m_panel.FindChild("panel_money/gas").GetComponent<Text>();
        txt_sgas = m_panel.FindChild("panel_money/sgas").GetComponent<Text>();


        itemListView = m_panel.FindChild("item_scroll/scroll_view/contain").gameObject; 
        item_Parent = itemListView.GetComponent<GridLayoutGroup>();


        //主界面ui
        m_panel.FindChild("panel_money/btn_gas_sgas").GetComponent<Button>().onClick.AddListener(() =>
        {
            m_panel.FindChild("panel_gas_sgas").gameObject.SetActive(true);
        });
        m_panel.FindChild("panel_money/btn_sgas_gas").GetComponent<Button>().onClick.AddListener(() =>
        {
            m_panel.FindChild("panel_sgas_gas").gameObject.SetActive(true);
        });
        m_panel.FindChild("panel_card/transfer_to").GetComponent<Button>().onClick.AddListener(() =>
        {
            m_panel.FindChild("panel_gas_gas/money").GetComponent<Text>().text = roleInfo.getInstance().gas.ToString();
            m_panel.FindChild("panel_gas_gas").gameObject.SetActive(true);
        });
        m_panel.FindChild("panel_card/transfer_from").GetComponent<Button>().onClick.AddListener(() =>
        {
            hide();
            testtool.panel_address_code.show();
        });
        m_panel.FindChild("panel_gas_sgas/close").GetComponent<Button>().onClick.AddListener(() =>
        {
            m_panel.FindChild("panel_gas_sgas").gameObject.SetActive(false);
        });
        m_panel.FindChild("panel_sgas_gas/close").GetComponent<Button>().onClick.AddListener(() =>
        {
            m_panel.FindChild("panel_sgas_gas").gameObject.SetActive(false);
        });
        m_panel.FindChild("panel_gas_gas/close").GetComponent<Button>().onClick.AddListener(() =>
        {
            m_panel.FindChild("panel_gas_gas").gameObject.SetActive(false);
        });
        m_panel.FindChild("role").GetComponent<Button>().onClick.AddListener(() =>
        {
            hide();
            testtool.panel_roleinfo.show();
        });
       
        m_panel.FindChild("panel_gas_gas/ok").GetComponent<Button>().onClick.AddListener(onClick_gas_gas);
        m_panel.FindChild("panel_gas_sgas/ok").GetComponent<Button>().onClick.AddListener(onClick_gas_sgas);
        m_panel.FindChild("panel_sgas_gas/ok").GetComponent<Button>().onClick.AddListener(onClick_sgas_gas);
        m_panel.FindChild("panel_card/refresh").GetComponent<Button>().onClick.AddListener(onClick_Refresh);
    }

    void onClick_gas_gas()
    {
        m_panel.FindChild("panel_gas_gas").gameObject.SetActive(false);
        InputField input_recharge_address = m_panel.FindChild("panel_gas_gas/address").GetComponent<InputField>();
        InputField input_recharge_num = m_panel.FindChild("panel_gas_gas/num").GetComponent<InputField>();
        if (input_recharge_address.text == "" /*|| input_recharge_address.text.Length !=*/)
        {
            testtool.showNotice("地址输入有误");
            return;
        }
        if (input_recharge_num.text == "" || input_recharge_num.text == "0")
        {
            testtool.showNotice("数量输入有误");
            return;
        }
        sdk_http._instance.transform_gas(input_recharge_address.text, decimal.Parse(input_recharge_num.text));
    }
    void onClick_gas_sgas()
    {
        m_panel.FindChild("panel_gas_sgas").gameObject.SetActive(false);
        InputField input_recharge_num = m_panel.FindChild("panel_gas_sgas/num").GetComponent<InputField>();
        if (input_recharge_num.text == "0" || input_recharge_num.text == "")
        {
            testtool.showNotice("数量输入有误");
            return;
        }
        sdk_http._instance.transform_gas_sgas(decimal.Parse(input_recharge_num.text));
    }
    void onClick_sgas_gas()
    {
        m_panel.FindChild("panel_sgas_gas").gameObject.SetActive(false);
        InputField input_recharge_num = m_panel.FindChild("panel_sgas_gas/num").GetComponent<InputField>();
        if (input_recharge_num.text == "0" || input_recharge_num.text == "")
        {
            testtool.showNotice("数量输入有误");
            return;
        }
        sdk_http._instance.transform_sgas_gas(decimal.Parse(input_recharge_num.text));
    }
    void onClick_Refresh()
    {
        sdk_http._instance.get_main_info();
    }

    public void on_refresh_info()
    {
        txt_name.text = roleInfo.getInstance().name;
        txt_gas.text =  roleInfo.getInstance().gas.ToString();
        txt_sgas.text = roleInfo.getInstance().sgas.ToString();
        string txt1 = roleInfo.getInstance().address;
        string txt2 = roleInfo.getInstance().address;
        txt_address.text = txt1.Substring(0, 4) + "****" + txt2.Substring(txt2.Length - 5);
    }

    public void refresh_list_ui(MyJson.JsonNode_Array json)
    {
        if (m_json.ToString() == json.ToString())
        {
            return;
        }

        m_json = json;
        int index = 0;
        foreach (var item in m_json)
        {
            GameObject itemclone;
            if (m_itemList.ContainsKey(item.AsDict()["id"].ToString()))
            {
                itemclone = m_itemList[item.AsDict()["id"].ToString()];
            }
            else
            {
                GameObject go = m_panel.FindChild("item_scroll/scroll_view/icon").gameObject;
                itemclone = ((GameObject)GameObject.Instantiate(go));
                m_itemList[item.AsDict()["id"].ToString()] = itemclone;

                itemclone.SetActive(true);
                itemclone.transform.SetParent(item_Parent.transform, false);
                itemclone.transform.SetSiblingIndex(index);
            }

            string txt_do = "";
            if (item.AsDict()["params"].ToString().Substring(0, 1) == "[")
            {
                txt_do = MyJson.Parse(item.AsDict()["params"].ToString()).AsList()[0].AsDict()["sbPushString"].ToString();
            }
            else
            {
                txt_do = MyJson.Parse(item.AsDict()["params"].ToString()).AsDict()["sbPushString"].ToString();
            }

            switch (item.AsDict()["type"].ToString())
            {
                case "1":
                    itemclone.transform.FindChild("sgas").gameObject.SetActive(true);
                    break;
                case "2":
                    itemclone.transform.FindChild("sgas").gameObject.SetActive(true);
                    break;
                case "5":
                    itemclone.transform.FindChild("tran").gameObject.SetActive(true);
                    break;
                case "6":
                    itemclone.transform.FindChild("gas").gameObject.SetActive(true);

                    break;

            }

            string txt_cnt = "";
            if (item.AsDict()["cnts"].ToString() != "")
            {
                if(item.AsDict()["type"].ToString() == "1"  || (item.AsDict()["type"].ToString() == "5" && item.AsDict()["type_detail"].ToString() == "2"))
                    txt_cnt = "+" + item.AsDict()["cnts"].ToString();
                else
                    txt_cnt = "-" + item.AsDict()["cnts"].ToString();
            }

            string txt_time = "";
            switch (item.AsDict()["state"].ToString())
            {
                case "0":
                    txt_time = "等待...";
                    break;
                case "1":
                    txt_time = "成功";
                    if (item.AsDict()["type"].ToString() == "2" && item.AsDict()["client_notify"].ToString() == "0")
                        txt_time = "等待...";
                    break;
                case "2":
                    txt_time = "失败";
                    break;
                default:

                    break;
            }
           
            itemclone.transform.FindChild("text_do").GetComponent<Text>().text = txt_do;
            itemclone.transform.FindChild("text_num").GetComponent<Text>().text = txt_cnt;
            itemclone.transform.FindChild("text_time").GetComponent<Text>().text = txt_time;

            index++;
        }

        RectTransform con = item_Parent.gameObject.GetComponent<RectTransform>();
        float childSizeY = item_Parent.cellSize.y + item_Parent.spacing.y;
        Vector2 newSize = new Vector2(con.sizeDelta.x, childSizeY * m_json.Count);
        con.sizeDelta = newSize;
    }

    void check_list(MyJson.JsonNode_Array json)
    {
        b_list_need_refresh = false;
        foreach (var item in json)
        {
            if (item.AsDict()["state"].ToString() == "0")
            {
                b_list_need_refresh = true;
            }
            else if (item.AsDict()["type"].ToString() == "2" && item.AsDict()["state"].ToString() == "1")
            {
                if (item.AsDict()["client_notify"].ToString() == "0")
                    b_list_need_refresh = true;
            }
        }

        if (!b_list_need_refresh)
            onClick_Refresh();
    }

    void check_plat_notify(MyJson.IJsonNode json)
    {
        b_notify_need_refresh = false;

        var json_pending = json.AsDict()["pending"].AsList();
        if (json_pending.Count > 0)
        {//有pending数据，开启定时器
            b_notify_need_refresh = true;
        }

        var json_complete = json.AsDict()["complete"].AsList();
        if (json_complete.Count > 0)
        {
            foreach (var item in json_complete)
            {
                string txid = item.AsDict()["txid"].ToString();
                string ext = item.AsDict()["ext"].ToString();
                if (!m_platNotifyTxids.ContainsKey(txid))
                {
                    //notify
                    do_plat_notify(item, ext);

                    m_platNotifyTxids[txid] = 1;
                }
            }
        }
    }

    public void do_plat_notify(MyJson.IJsonNode json_complete, string ext = "")
    {
        string txid = json_complete.AsDict()["txid"].ToString();
        switch (json_complete.AsDict()["type"].ToString())
        {
            case "2": //sgas->gas退款
                if (json_complete.AsDict()["state"].ToString() == "1")
                {
                    if (ext == "")
                    {//sgas->utxo确认
                        sdk_http._instance.transform_sgas_gas2(json_complete);
                    }
                    else
                    {//utxo->gas提交成功，循环获取结果
                        doPlatNotifyRefundRes(txid, ext);
                    }
                }
                else
                {
                    api_tool._instance.walletNotify(roleInfo.getInstance().uid, roleInfo.getInstance().token, txid, global.netType, null);
                }
                break;
        }
    }

    public void doPlatNotifyRefundRes(string txid, string ext)
    {
        sdk_http._instance.getrawtransaction(ext , (bool timeout, WWW www) => { 
            api_tool._instance.walletNotify(roleInfo.getInstance().uid, roleInfo.getInstance().token, txid, global.netType, null);
        });
    }


    public void on_refresh_WalletListss()
    {
        api_tool._instance.getWalletListss(roleInfo.getInstance().uid, roleInfo.getInstance().token, 1, 5, global.netType,
            (bool timeout, WWW www) => {
                var json = MyJson.Parse(www.text).AsDict()["data"].AsList();
                check_list(json);
                refresh_list_ui(json);
            });
    }

    public void on_refresh_plat_NotifyList()
    {
        api_tool._instance.getPlatWalletNotifys(roleInfo.getInstance().uid, roleInfo.getInstance().token, global.netType,
           (bool timeout, WWW www) => {
               var json = MyJson.Parse(www.text).AsDict()["data"];
               check_plat_notify(json);
           });
    }

    bool b_list_need_refresh = false;
    bool b_notify_need_refresh = false;

    float update_list_time = 0;
    float updata_notify_time = 0;
    float refresh_list_tiem = 8;
    float refresh_notify_tiem = 8;
    public void Update()
    {
        if (b_list_need_refresh)
        {
            update_list_time += Time.deltaTime;
            if (update_list_time > refresh_list_tiem)
            {
                on_refresh_WalletListss();
                update_list_time = 0;
            }
        }

        if (b_notify_need_refresh)
        {
            updata_notify_time += Time.deltaTime;
            if (updata_notify_time > refresh_notify_tiem)
            {
                on_refresh_plat_NotifyList();
                updata_notify_time = 0;
            }
        }
    }
}
