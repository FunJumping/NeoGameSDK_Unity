﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using UnityEngine;

public class NeoGameSDK_CS
{
    static GameObject go_login;
    static public Action<NeoGameSDK_login_data> api_cb_login;
    static public Action<NeoGameSDK_pay_data> api_cb_pay;
    static public Action api_cb_loginout;

    static public void init(int gameid, Action<NeoGameSDK_login_data> func_login, Action<NeoGameSDK_pay_data> func_pay, Action func_loginout)
    {
        //gameid 由黑猫后台配置，同时提供充值回调地址给后台。到时 sgas 充值到游戏中，后台确认后返回给游戏端 
        global.game_id = gameid;
        api_cb_login = func_login;
        api_cb_pay = func_pay;
        api_cb_loginout = func_loginout;

        GameObject ui_root_prefab = Resources.Load("Canvas_login") as GameObject;
        if (ui_root_prefab != null)
        {
            GameObject ui_root = GameObject.Instantiate(ui_root_prefab) as GameObject;
            go_login = ui_root.transform.FindChild("login").gameObject;
        }
    }
    static public void login()
    {
        if (PlayerPrefs.HasKey("uid") && PlayerPrefs.HasKey("token") && PlayerPrefs.GetString("uid") != "" && PlayerPrefs.GetString("token") != "")
        {
            api_tool._instance.isLogined(PlayerPrefs.GetString("uid"), PlayerPrefs.GetString("token"), (bool timeout, WWW www) =>
            {
                var r = MyJson.Parse(www.text).AsDict()["r"].AsInt();
                if (r == 1)
                    testtool.panel_login.on_user_login(timeout, www);
                else
                    testtool.panel_login.show();
            }, false);
        }
        else
        {
            testtool.panel_login.show();
        }
        go_login.SetActive(true);
    }

    static public void recharge(decimal num)
    {
        sdk_http._instance.recharge_game_sgas(num);
    }

    static public void invokescrept(Action<bool, WWW> func_cmd, MyJson.JsonNode_Object paparms)
    {
        sdk_http._instance.invokescrept(func_cmd, paparms);
    }

    static public void makeRawTransaction(Action<bool, WWW> func_cmd, MyJson.JsonNode_Array paparms)
    {
        sdk_http._instance.makeRawTransaction(func_cmd, paparms);
    }

    //后续添加在链上的操作
}

public class NeoGameSDK_login_data
{
    public string uid;
    public string token;
    public string lastlogin;
    public string wallet;
}

public class NeoGameSDK_pay_data
{
    public string uid;
    public string token;
    public string txid;
    public string cnts;
}

