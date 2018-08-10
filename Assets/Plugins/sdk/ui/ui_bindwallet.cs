using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;
using Common;

public class ui_bindwallet : basePanel
{
    InputField input_wallet_password;

    public ui_bindwallet(Transform go)
    {
        m_panel = go;

        input_wallet_password = m_panel.FindChild("wallet_open/wallet_password").GetComponent<InputField>();

        m_panel.FindChild("creat").GetComponent<Button>().onClick.AddListener(onCLick_wallet_creat);
        m_panel.FindChild("wallet_create/open").GetComponent<Button>().onClick.AddListener(onCLick_wallet_creat2);

        m_panel.FindChild("open").GetComponent<Button>().onClick.AddListener(onCLick_wallet_open);
        m_panel.FindChild("wallet_open/wallet_file").GetComponent<Button>().onClick.AddListener(onCLick_wallet_open2);
        m_panel.FindChild("wallet_open/open").GetComponent<Button>().onClick.AddListener(onCLick_wallet_open3);

        m_panel.FindChild("wallet_open/close").GetComponent<Button>().onClick.AddListener(() =>
        {
            m_panel.FindChild("wallet_open").gameObject.SetActive(false);
        });
        m_panel.FindChild("wallet_create/close").GetComponent<Button>().onClick.AddListener(() =>
        {
            m_panel.FindChild("wallet_create").gameObject.SetActive(false);
        });
    }

    //导入钱包的状态栏变化
    void showOpenWallet(string files = "")
    {
        input_wallet_password.text = "";
        if (files == "")
        {
            m_panel.FindChild("wallet_open/wallet_file/Placeholder").gameObject.SetActive(true);
            m_panel.FindChild("wallet_open/wallet_file/Text").gameObject.SetActive(false);
        }
        else
        {
            m_panel.FindChild("wallet_open/wallet_file/Placeholder").gameObject.SetActive(false);
            m_panel.FindChild("wallet_open/wallet_file/Text").gameObject.SetActive(true);
            m_panel.FindChild("wallet_open/wallet_file/Text").GetComponent<Text>().text = files;
        }
    }

    void onCLick_wallet_creat()
    {
        m_panel.FindChild("wallet_create").gameObject.SetActive(true);
    }

    void onCLick_wallet_creat2()
    {
        string passward1 = m_panel.FindChild("wallet_create/wallet_password1").GetComponent<InputField>().text;
        string passward2 = m_panel.FindChild("wallet_create/wallet_password2").GetComponent<InputField>().text;
        if (passward1 == "")
        {
            testtool.showNotice("请输入密码");
            return;
        }
        if (passward2 == "")
        {
            testtool.showNotice("请确认密码");
            return;
        }
        if(passward1 != passward2)
        {
            testtool.showNotice("密码不一致");
            return;
        }

        try
        {
            //创建一个钱包
            ThinNeo.NEP6.NEP6Wallet wallet = new ThinNeo.NEP6.NEP6Wallet("");
            byte[] privateKey = new byte[32];
            RandomNumberGenerator rng = RandomNumberGenerator.Create();
            rng.GetBytes(privateKey);
            wallet.CreateAccount(privateKey, passward1);
            MyJson.JsonNode_Object wallet_json = new MyJson.JsonNode_Object();
            var n = new MyJson.JsonNode_ValueNumber();
            n.SetNull();
            wallet_json["name"] = n;
            wallet_json["version"] = new MyJson.JsonNode_ValueString("1.0");
            wallet_json["scrypt"] = wallet.scrypt.ToJson();
            wallet_json["accounts"] = new MyJson.JsonNode_Array();
            foreach (var ac in wallet.accounts.Values)
            {
                var jnot = ac.ToJson();
                wallet_json["accounts"].AsList().Add(jnot);
            }
            wallet_json["extra"] = n;

            var wif = ThinNeo.Helper.GetWifFromPrivateKey(privateKey);
            roleInfo.getInstance().set_storage(wif);

            m_panel.FindChild("wallet_create").gameObject.SetActive(false);
            api_tool._instance.bindWallet(roleInfo.getInstance().uid, roleInfo.getInstance().token, wallet_json.ToString(), on_wallet_bind);
        }
        catch
        {
            testtool.showNotice("创建钱包错误");
        }
    }

    void onCLick_wallet_open()
    {
        showOpenWallet("");
        m_panel.FindChild("wallet_open").gameObject.SetActive(true);
    }

    //打开导入的钱包文件
    void onCLick_wallet_open2()
    {
#if UNITY_ANDROID
        Debug.Log("这里是安卓设备^_^");
#endif

#if UNITY_IPHONE
        Debug.Log("这里是苹果设备>_<");
#endif

#if UNITY_STANDALONE_WIN
        OpenFileName ofn = new OpenFileName();
        ofn.structSize = Marshal.SizeOf(ofn);
        ofn.filter = "All Files\0*.*\0\0";
        ofn.file = new string(new char[256]);
        ofn.maxFile = ofn.file.Length;
        ofn.fileTitle = new string(new char[64]);
        ofn.maxFileTitle = ofn.fileTitle.Length;
        ofn.initialDir = UnityEngine.Application.dataPath;//默认路径
        ofn.title = "Open Project";
        ofn.defExt = "json";//显示文件的类型
                            //注意 一下项目不一定要全选 但是0x00000008项不要缺少
        ofn.flags = 0x00080000 | 0x00001000 | 0x00000800 | 0x00000200 | 0x00000008;//OFN_EXPLORER|OFN_FILEMUSTEXIST|OFN_PATHMUSTEXIST| OFN_ALLOWMULTISELECT|OFN_NOCHANGEDIR
        if (DllTest.GetOpenFileName(ofn))
        {
            if (ofn.fileTitle.Substring(ofn.fileTitle.Length - 5) != ".json")
            {
                testtool.showNotice("导入文件错误");
                showOpenWallet("");
            }
            else
            {
                showOpenWallet(ofn.fileTitle);
                api_tool._instance.wallet_load(ofn.file);
                Debug.Log("Selected file with full path: {0}" + ofn.file);
            }
        }

#endif
    }


    void onCLick_wallet_open3()
    {
        if (roleInfo.getInstance().wallet_json == "")
        {
            testtool.showNotice("未导入钱包");
            return;
        }


        //默认第一个
        ThinNeo.NEP6.NEP6Wallet one = new ThinNeo.NEP6.NEP6Wallet(roleInfo.getInstance().wallet_json, "");
        ThinNeo.NEP6.NEP6Account acc = one.accounts.Values.FirstOrDefault();
        if (acc == null || acc.nep2key == null)
        {
            testtool.showNotice("密码错误或者其他错误");
            return;
        }
        try
        {
            var prikey = acc.GetPrivate(one.scrypt, input_wallet_password.text);
            var wif = ThinNeo.Helper.GetWifFromPrivateKey(prikey);
            roleInfo.getInstance().set_storage(wif);

            m_panel.FindChild("wallet_open").gameObject.SetActive(false);
            api_tool._instance.bindWallet(roleInfo.getInstance().uid, roleInfo.getInstance().token, roleInfo.getInstance().wallet_json, on_wallet_bind);
        }
        catch
        {
            testtool.showNotice("密码错误或者其他错误");
        }

    }

    private void on_wallet_bind(bool timeout, WWW www)
    {
        hide();
        api_tool._instance.isLogined(roleInfo.getInstance().uid, roleInfo.getInstance().token,(bool timeout1, WWW www1) => {
            testtool.panel_login.on_user_login(timeout1, www1);
        });
    }
}
