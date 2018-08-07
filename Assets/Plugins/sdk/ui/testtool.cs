using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;
using Common;
public class testtool : MonoBehaviour
{
    static public ui_register panel_register;
    static public ui_login panel_login;
    static public ui_bindwallet panel_bindwallet;
    static public ui_walletPassword panel_wallet_password;
    static public ui_main panel_main;
    static public ui_notice panel_notice;
    static public ui_roleinfo panel_roleinfo;

    static Text txt_notcie;

    GameObject panel_main_ui;
    Button btn_main;
  
    // Use this for initialization
    void Start()
    {
        panel_main_ui = this.transform.FindChild("main_ui").gameObject;
        panel_login = new ui_login(this.transform.FindChild("main_ui/login"));
        panel_register = new ui_register(this.transform.FindChild("main_ui/register"));
        panel_bindwallet = new ui_bindwallet(this.transform.FindChild("main_ui/bindwallet"));
        panel_wallet_password = new ui_walletPassword(this.transform.FindChild("main_ui/wallet_password"));
        panel_main = new ui_main(this.transform.FindChild("main_ui/main"));
        panel_notice = new ui_notice(this.transform.FindChild("main_ui/notice"));
        panel_roleinfo = new ui_roleinfo(this.transform.FindChild("main_ui/roleinfo"));

        btn_main = this.transform.FindChild("btn_main").GetComponent<Button>();
        btn_main.onClick.AddListener(onMainPanelCtr);
    }

    bool mainPanel_show = true;
    void onMainPanelCtr()
    {
        if (mainPanel_show)
        {
            panel_main_ui.SetActive(false);
            mainPanel_show = false;
        }
        else
        {
            panel_main_ui.SetActive(true);
            mainPanel_show = true;
        }

    }


    // Update is called once per frame
    void Update()
    {

    }

    static public void on_error_notice(int code)
    {
        string str = "";
        switch (code)
        {
            case 8100000: str = "已绑定钱包"; break;
            case 8100001: str = "钱包文件内容有误"; break;
            case 8100002: str = "钱包绑定失败"; break;
            case 8100003: str = "该钱包已经绑定了"; break;

            case 100600: str = "验证码X分钟内不能重复发送"; break;
            case 100601: str = "获取短信码失败"; break;
            case 100602: str = "短信码错误"; break;
            case 100603: str = "新手机号已经绑定其他账号了"; break;
            case 100604: str = "注册手机用户失败"; break;

            case 100700: str = " 账号/密码错误"; break;
            case 100701: str = "登录态失效，重新登录"; break;
            case 100702: str = "用户信息获取失败"; break;
            case 100703: str = "生成游客账号失败"; break;
            case 100704: str = "生成游客账号太快"; break;
            case 100705: str = "旧密码错误"; break;
            case 100706: str = "账号已经绑定手机了"; break;
            default: str = "错误"; break;
        }

        panel_notice.showNotice(str);
    }
    static public void showNotice(string str)
    {
        panel_notice.showNotice(str);
    }
}
