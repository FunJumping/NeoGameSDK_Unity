using UnityEngine;
using UnityEngine.UI;

public class ui_login : basePanel
{
    int tab = 0;
    public ui_login(Transform go)
    {
        m_panel = go;

        m_panel.FindChild("tab_phone").GetComponent<Button>().onClick.AddListener(on_tab_phone);
        m_panel.FindChild("tab_uid").GetComponent<Button>().onClick.AddListener(on_tab_user);
        m_panel.FindChild("tab_email").GetComponent<Button>().onClick.AddListener(on_tab_email);

        m_panel.FindChild("login").GetComponent<Button>().onClick.AddListener(on_login);
        m_panel.FindChild("register").GetComponent<Button>().onClick.AddListener(on_register);

        on_tab_user();
    }

    void on_register()
    {
        hide();
        testtool.panel_register.show();
    }

    public string getphone_group()
    {
        Dropdown drop = m_panel.FindChild("panel_phone/Dropdown").GetComponent<Dropdown>();
        string txt = "";
        Debug.Log(drop.itemText.text);
        switch (drop.itemText.text)
        {
            case "中国+86": txt = "@86"; break;
        }
        return txt;
    }

    void on_login()
    {
        string txt_passward;
        if (tab == 0)
        {
            string txt_name = m_panel.FindChild("panel_user/username").GetComponent<InputField>().text;
            txt_passward = m_panel.FindChild("panel_user/passward").GetComponent<InputField>().text;
            if (txt_name == "") { testtool.showNotice("请输入用户名");  return; }
            if (txt_passward == "") { testtool.showNotice("请输入密码"); return; }

            api_tool._instance.userLoginPass(txt_name, txt_passward, on_user_login);
        }
        else if (tab == 1)
        {
            string txt_phnoe = m_panel.FindChild("panel_phone/phonenum").GetComponent<InputField>().text;
            txt_passward = m_panel.FindChild("panel_phone/passward").GetComponent<InputField>().text;
            if (txt_phnoe == "") { testtool.showNotice("请输入手机号"); return; }
            if (txt_passward == "") { testtool.showNotice("请输入密码"); return; }
            txt_phnoe += getphone_group();

            api_tool._instance.phoneLoginPass(txt_phnoe, txt_passward, on_user_login);
        }
        else
        {
            string txt_email = m_panel.FindChild("panel_email/email").GetComponent<InputField>().text;
            txt_passward = m_panel.FindChild("panel_email/passward").GetComponent<InputField>().text;
            if (txt_email == "") { testtool.showNotice("请输入邮箱"); return; }
            if (txt_passward == "") { testtool.showNotice("请输入密码"); return; }
            string expression = @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";
            if (!System.Text.RegularExpressions.Regex.IsMatch(txt_email, expression))
            {
                testtool.showNotice("邮箱格式错误");
            }
            else
            {
                api_tool._instance.emailLoginPass(txt_email, txt_passward, on_user_login);
            }
        }
    }

    void on_tab_phone()
    {
        tab = 1;
        m_panel.FindChild("tab_phone").GetComponent<Button>().interactable = false;
        m_panel.FindChild("tab_email").GetComponent<Button>().interactable = true;
        m_panel.FindChild("tab_uid").GetComponent<Button>().interactable = true;
        m_panel.FindChild("panel_phone").gameObject.SetActive(true);
        m_panel.FindChild("panel_email").gameObject.SetActive(false);
        m_panel.FindChild("panel_user").gameObject.SetActive(false);
    }

    void on_tab_email()
    {
        tab = 2;
        m_panel.FindChild("tab_phone").GetComponent<Button>().interactable = true;
        m_panel.FindChild("tab_uid").GetComponent<Button>().interactable = true;
        m_panel.FindChild("tab_email").GetComponent<Button>().interactable = false;
        m_panel.FindChild("panel_phone").gameObject.SetActive(false);
        m_panel.FindChild("panel_email").gameObject.SetActive(true);
        m_panel.FindChild("panel_user").gameObject.SetActive(false);
    }

    void on_tab_user()
    {
        tab = 0;
        m_panel.FindChild("tab_phone").GetComponent<Button>().interactable = true;
        m_panel.FindChild("tab_uid").GetComponent<Button>().interactable = false;
        m_panel.FindChild("tab_email").GetComponent<Button>().interactable = true;
        m_panel.FindChild("panel_phone").gameObject.SetActive(false);
        m_panel.FindChild("panel_email").gameObject.SetActive(false);
        m_panel.FindChild("panel_user").gameObject.SetActive(true);
    }

    public void on_user_login(bool timeout, WWW www)
    {
        string data = MyJson.Parse(www.text).AsDict()["data"].ToString();
        PlayerPrefs.SetString(roleInfo.getInstance().key_info, data);
        roleInfo.getInstance().get_info();

        if (roleInfo.getInstance().wallet == "")
        {
            hide();
            testtool.panel_bindwallet.show();
        }
        else
        {
            api_tool._instance.getWalletFile(roleInfo.getInstance().uid, roleInfo.getInstance().token, on_getWalletFile);
        }
    }
    public void on_getWalletFile(bool timeout, WWW www)
    {
        roleInfo.getInstance().wallet_json = MyJson.Parse(www.text).AsDict()["data"].AsString();

        check_logined();
    }

    public void check_logined()
    {
        if (!PlayerPrefs.HasKey("wif") ||
            PlayerPrefs.GetString("wif") == "" ||
            PlayerPrefs.GetString("uid") != roleInfo.getInstance().uid)
        {//这里重新登入钱包
            testtool.panel_wallet_password.show();
        }
        else
        {
            roleInfo.getInstance().get_storage();
            hide();
            testtool.panel_main.show();
            testtool.panel_main.on_refresh_WalletListss();
            testtool.panel_main.on_refresh_plat_NotifyList();
            roleInfo.getInstance().logined_cb();
            sdk_http._instance.get_main_info();
        }
    }
}
