using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
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
    static public ui_addressCode panel_address_code;
    static public ui_findPassword panel_findPassword;

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
        panel_address_code = new ui_addressCode(this.transform.FindChild("main_ui/adressCode"));
        panel_findPassword = new ui_findPassword(this.transform.FindChild("main_ui/findPassword"));

        btn_main = this.transform.FindChild("btn_main").GetComponent<Button>();
        btn_main.onClick.AddListener(onMainPanelCtr);
        panel_main_ui.transform.FindChild("btn_bg").GetComponent<Button>().onClick.AddListener(onMainPanelCtr);
        EventTriggerListener.Get(btn_main.gameObject).onDrag = OnDrag;
        EventTriggerListener.Get(btn_main.gameObject).onDragOut = OnDragOut;
    }

    bool mainPanel_show = true;
    public void onMainPanelCtr()
    {
        if (m_moving)
            return;
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

    bool m_moving = false;
    void OnDrag(GameObject go, Vector2 delta)
    {
        m_moving = true;
        Debug.Log(delta.x + " " + delta.y);
        btn_main.transform.localPosition += new Vector3(delta.x, delta.y, 0);
    }
    void OnDragOut(GameObject go = null)
    {
        m_moving = false;
    }

    // Update is called once per frame
    void Update()
    {
        panel_main.Update();
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
            case 8100004: str = "头像文件格式不支持！"; break;
            case 8100005: str = "头像文件上传失败！"; break;
            case 8100006: str = "头像文件大小不能超过100kb！"; break;
            case 8100007: str = "联系人已存在！"; break;

            case 100600: str = "验证码X分钟内不能重复发送"; break;
            case 100601: str = "获取短信码失败"; break;
            case 100602: str = "短信码错误"; break;
            case 100603: str = "新手机号已经绑定其他账号了"; break;
            case 100604: str = "注册手机用户失败"; break;
            case 100605: str = "手机已注册！"; break;
            case 100606: str = "手机未注册，请先注册"; break;
            case 100607: str = "用户名或手机错误！"; break;
            case 100608: str = "手机或密码错误！"; break;

            case 100700: str = " 账号/密码错误"; break;
            case 100701: str = "登录态失效，重新登录"; break;
            case 100702: str = "用户信息获取失败"; break;
            case 100703: str = "生成游客账号失败"; break;
            case 100704: str = "生成游客账号太快"; break;
            case 100705: str = "旧密码错误"; break;
            case 100706: str = "账号已经绑定手机了"; break;
            case 100707: str = "用户名已存在！"; break;
            case 100708: str = "用户名格式不正确！"; break;
            case 100709: str = "地区代码错误！"; break;

            case 100801: str = "获取邮箱验证码失败！"; break;
            case 100802: str = "邮箱验证码错误！"; break;
            case 100805: str = "邮箱已注册！"; break;
            case 100806: str = "邮箱未注册，请先注册"; break;
            case 100807: str = "用户名或邮箱错误！"; break;
            case 100808: str = "邮箱或密码错误！"; break;

            default: str = "未知错误，错误码：" + code; break;
        }

        panel_notice.showNotice(str);
    }
    static public void showNotice(string str)
    {
        panel_notice.showNotice(str);
    }
}
