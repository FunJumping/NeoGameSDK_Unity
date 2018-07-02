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
    Text txt_notcie;
    Text txt_name;
    Text txt_address;
    Text txt_gas;
    Text txt_sgas;
    InputField input_phone;
    InputField input_code;
    InputField input_wallet_password;
    InputField input_wallet_password2;
    InputField input_recharge_num;
    Transform panel_wallet;
    Transform panel_phone;
    Transform panel_notice;
    Transform panel_password;
    Transform panel_main;
    Button btn_main;
    int recharge_type = 0;  //0 gas转sgas     1 sgas转入游戏内的sgas
    // Use this for initialization
    void Start()
    {
        panel_phone = this.transform.FindChild("phone");
        panel_wallet = this.transform.FindChild("wallet");
        panel_notice = this.transform.FindChild("notice");
        panel_password = this.transform.FindChild("wallet_password");
        panel_main = this.transform.FindChild("main");
        input_phone = panel_phone.FindChild("phone_num").GetComponent<InputField>();
        input_code = panel_phone.FindChild("phone_code").GetComponent<InputField>();
        input_wallet_password = panel_wallet.FindChild("wallet_open/wallet_password").GetComponent<InputField>();
        input_wallet_password2 = panel_password.FindChild("wallet_password").GetComponent<InputField>();
        input_recharge_num = panel_main.FindChild("rechargenum/num").GetComponent<InputField>();
        input_recharge_num.text = "0";
        txt_name = panel_main.FindChild("name").GetComponent<Text>();
        txt_address = panel_main.FindChild("address").GetComponent<Text>();
        txt_gas = panel_main.FindChild("gas").GetComponent<Text>();
        txt_sgas = panel_main.FindChild("sgas").GetComponent<Text>();
        btn_main = this.transform.FindChild("btn_main").GetComponent<Button>();

        if (PlayerPrefs.HasKey("phone"))
            input_phone.text = PlayerPrefs.GetString("phone");

        //提示ui
        txt_notcie = panel_notice.FindChild("txt").GetComponent<Text>();
        panel_notice.FindChild("bg").GetComponent<Button>().onClick.AddListener(() =>
        {
            panel_notice.gameObject.SetActive(false);
        });

        //手机号登入ui
        panel_phone.FindChild("login").GetComponent<Button>().onClick.AddListener(onCLick_phone_login);
        panel_phone.FindChild("get_num").GetComponent<Button>().onClick.AddListener(onCLick_phone_code);

        //钱包导入ui
        panel_wallet.FindChild("creat").GetComponent<Button>().onClick.AddListener(onCLick_wallet_creat);
        panel_wallet.FindChild("open").GetComponent<Button>().onClick.AddListener(onCLick_wallet_open);
        panel_wallet.FindChild("wallet_open/wallet_file").GetComponent<Button>().onClick.AddListener(onCLick_wallet_open2);
        panel_wallet.FindChild("wallet_open/open").GetComponent<Button>().onClick.AddListener(onCLick_wallet_open3);
        
        panel_wallet.FindChild("wallet_open/close").GetComponent<Button>().onClick.AddListener(() =>
        {
            panel_wallet.FindChild("wallet_open").gameObject.SetActive(false);
        });

        //钱包密码登入ui
        panel_password.FindChild("open").GetComponent<Button>().onClick.AddListener(onClick_wallet_password);

        //主界面ui
        panel_main.FindChild("recharge").GetComponent<Button>().onClick.AddListener(() =>
        {
            recharge_type = 0;
            panel_main.FindChild("rechargenum").gameObject.SetActive(true);
        });
        panel_main.FindChild("game_recharge").GetComponent<Button>().onClick.AddListener(() =>
        {
            recharge_type = 1;
            panel_main.FindChild("rechargenum").gameObject.SetActive(true);
        });
        panel_main.FindChild("rechargenum/ok").GetComponent<Button>().onClick.AddListener(onClick_Recharge);
        panel_main.FindChild("refresh").GetComponent<Button>().onClick.AddListener(onClick_Refresh);
        panel_main.FindChild("loginout").GetComponent<Button>().onClick.AddListener(onLoginOut);

        btn_main.onClick.AddListener(onMainPanelCtr);

        sdk_http._instance.init(on_error_notice, on_refresh_info);
    }

    //显示提示
     public void showNotice(string str)
    {
        panel_notice.gameObject.SetActive(true);
        txt_notcie.text = str;
    }

    //导入钱包的状态栏变化
    void showOpenWallet(string files = "")
    {
        input_wallet_password.text = "";
        if (files == "")
        {
            panel_wallet.FindChild("wallet_open/wallet_file/Placeholder").gameObject.SetActive(true);
            panel_wallet.FindChild("wallet_open/wallet_file/Text").gameObject.SetActive(false);
        }
        else
        {
            panel_wallet.FindChild("wallet_open/wallet_file/Placeholder").gameObject.SetActive(false);
            panel_wallet.FindChild("wallet_open/wallet_file/Text").gameObject.SetActive(true);
            panel_wallet.FindChild("wallet_open/wallet_file/Text").GetComponent<Text>().text = files;
        }
    }


    //手机号登入
    void onCLick_phone_login()
    {
        if (input_phone.text.Length != 11)
        {
            showNotice("登入失败");
        }
        else
        {
            sdk_http._instance.phone_login(input_phone.text, input_code.text, on_phone_login);
        }
    }

    //获取验证码
    void onCLick_phone_code()
    {
       
    }

    void onCLick_wallet_creat()
    {
        //创建钱包，这个后续加
    }

    void onCLick_wallet_open()
    {
        showOpenWallet("");
        panel_wallet.FindChild("wallet_open").gameObject.SetActive(true);
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
                showNotice("导入文件错误");
                showOpenWallet("");
            }
            else
            {
                showOpenWallet(ofn.fileTitle);
                sdk_http._instance.wallet_load(ofn.file);
                Debug.Log("Selected file with full path: {0}" + ofn.file);
            }
        }

#endif
    }

    void onCLick_wallet_open3()
    {
        if (roleInfo.getInstance().wallet_json == "")
        {
            showNotice("未导入钱包");
            return;
        }
           

        //默认第一个
        ThinNeo.NEP6.NEP6Wallet one = new ThinNeo.NEP6.NEP6Wallet(roleInfo.getInstance().wallet_json, "");
        ThinNeo.NEP6.NEP6Account acc = one.accounts.Values.FirstOrDefault();
        if (acc == null || acc.nep2key == null)
        {
            showNotice("密码错误或者其他错误");
            return;
        }
        try
        {
            var prikey = acc.GetPrivate(one.scrypt, input_wallet_password.text);
            var wif = ThinNeo.Helper.GetWifFromPrivateKey(prikey);
            sdk_http._instance.wallet_bind(wif, roleInfo.getInstance().wallet_json, on_get_wallet_file);
        }
        catch
        {
            showNotice("密码错误或者其他错误");
        }

    }
    void onClick_wallet_password()
    {
        string password = input_wallet_password2.text;
        ThinNeo.NEP6.NEP6Wallet one = new ThinNeo.NEP6.NEP6Wallet(roleInfo.getInstance().wallet_json, "");
        ThinNeo.NEP6.NEP6Account acc = one.accounts.Values.FirstOrDefault();
        if (acc == null || acc.nep2key == null)
        {
            showNotice("密码错误或者其他错误");
            return;
        }
        try
        {
            var prikey = acc.GetPrivate(one.scrypt, password);
            var wif = ThinNeo.Helper.GetWifFromPrivateKey(prikey);
            panel_password.gameObject.SetActive(false);
            roleInfo.getInstance().set_storage(wif);
            sdk_http._instance.get_main_info();

            on_get_wallet_file(false);
        }
        catch
        {
            showNotice("密码错误或者其他错误");
        }
    }

    void onClick_Recharge()
    {
        panel_main.FindChild("rechargenum").gameObject.SetActive(false);
        if (input_recharge_num.text != "0" && input_recharge_num.text != "")
        {
            if(recharge_type == 0)
                sdk_http._instance.recharge_sgas(decimal.Parse(input_recharge_num.text));
            else
                sdk_http._instance.recharge_game_sgas(decimal.Parse(input_recharge_num.text));
        }
    }

    void onClick_Refresh()
    {
        sdk_http._instance.get_main_info();
    }

    void onLoginOut()
    {
        panel_main.gameObject.SetActive(false);
        btn_main.gameObject.SetActive(false);
        panel_phone.gameObject.SetActive(true);
        roleInfo.getInstance().dispose_storage();

        NeoGameSDK_CS.api_cb_loginout();
    }

    bool mainPanel_show = false;
    void onMainPanelCtr()
    {
        if (mainPanel_show)
        {
            panel_main.gameObject.SetActive(false);
            mainPanel_show = false;
        }
        else
        {
            panel_main.gameObject.SetActive(true);
            mainPanel_show = true;
        }

    }


    // Update is called once per frame
    void Update()
    {

    }

    //手机号登入回调
    private void on_phone_login(bool login_wallet)
    {
        if (login_wallet)
            panel_wallet.gameObject.SetActive(true);
        else
            sdk_http._instance.get_wallet_file(on_get_wallet_file);
    }

    //登入后获取钱包信息回调
    private void on_get_wallet_file(bool need_password)
    {
        if (need_password)
            panel_password.gameObject.SetActive(true);
        else
        {
            panel_wallet.gameObject.SetActive(false);
            panel_phone.gameObject.SetActive(false);
            btn_main.gameObject.SetActive(true);
            //panel_main.gameObject.SetActive(true);

            on_refresh_info();

            NeoGameSDK_login_data one = new NeoGameSDK_login_data();
            one.uid = roleInfo.getInstance().uid;
            one.token = roleInfo.getInstance().token;
            one.lastlogin = roleInfo.getInstance().lastlogin;
            one.wallet = roleInfo.getInstance().wallet;
            //api的登入成功后回调函数
            NeoGameSDK_CS.api_cb_login(one);
        }
    }
    private void on_refresh_info()
    {
        txt_name.text = roleInfo.getInstance().name;
        txt_gas.text = "gas:" + roleInfo.getInstance().gas.ToString();
        txt_sgas.text = "sgas:" + roleInfo.getInstance().sgas.ToString();
        string txt1 = roleInfo.getInstance().address;
        string txt2 = roleInfo.getInstance().address;
        txt_address.text = txt1.Substring(0, 4) + "XXX" + txt2.Substring(txt2.Length - 5);
    }

    private void on_error_notice(int code)
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

        showNotice(str);
    }
}
