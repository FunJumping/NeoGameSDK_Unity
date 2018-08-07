using UnityEngine;
using UnityEngine.UI;

public class ui_main : basePanel
{
    Text txt_name;
    Text txt_address;
    Text txt_gas;
    Text txt_sgas;
    public ui_main(Transform go)
    {
        m_panel = go;

        txt_name = m_panel.FindChild("panel_card/name").GetComponent<Text>();
        txt_address = m_panel.FindChild("panel_card/address").GetComponent<Text>();
        txt_gas = m_panel.FindChild("panel_money/gas").GetComponent<Text>();
        txt_sgas = m_panel.FindChild("panel_money/sgas").GetComponent<Text>();
        

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
        sdk_http._instance.recharge_sgas(decimal.Parse(input_recharge_num.text));
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
}
