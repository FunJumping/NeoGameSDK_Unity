using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;
using Common;

public class ui_walletPassword : basePanel
{
    InputField input_wallet_password;
    public ui_walletPassword(Transform go)
    {
        m_panel = go;

        input_wallet_password = m_panel.FindChild("wallet_password").GetComponent<InputField>();
        m_panel.FindChild("open").GetComponent<Button>().onClick.AddListener(onClick_wallet_password);
    }

    void onClick_wallet_password()
    {
        string password = input_wallet_password.text;
        ThinNeo.NEP6.NEP6Wallet one = new ThinNeo.NEP6.NEP6Wallet(roleInfo.getInstance().wallet_json, "");
        ThinNeo.NEP6.NEP6Account acc = one.accounts.Values.FirstOrDefault();
        if (acc == null || acc.nep2key == null)
        {
            testtool.showNotice("密码错误或者其他错误");
            return;
        }
        try
        {
            var prikey = acc.GetPrivate(one.scrypt, password);
            var wif = ThinNeo.Helper.GetWifFromPrivateKey(prikey);
            
            roleInfo.getInstance().set_storage(wif);

            testtool.panel_login.hide();
            hide();
            testtool.panel_main.show();
            testtool.panel_main.on_refresh_info();
            roleInfo.getInstance().logined_cb();
            sdk_http._instance.get_main_info();
        }
        catch
        {
            testtool.showNotice("密码错误或者其他错误");
        }
    }
}
