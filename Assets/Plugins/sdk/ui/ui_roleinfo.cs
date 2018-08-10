using UnityEngine;
using UnityEngine.UI;

public class ui_roleinfo : basePanel
{
   
    public ui_roleinfo(Transform go)
    {
        m_panel = go;

        m_panel.FindChild("return").GetComponent<Button>().onClick.AddListener(() =>
        {
            hide();
            testtool.panel_main.show();
        });

        m_panel.FindChild("loginout").GetComponent<Button>().onClick.AddListener(onLoginOut);
    }

    public override void show()
    {
        m_panel.gameObject.SetActive(true);
        refreshInfo();
    }

    public void refreshInfo()
    {
        m_panel.FindChild("name").GetComponent<Text>().text = roleInfo.getInstance().name;
        m_panel.FindChild("sex").GetComponent<Text>().text = int.Parse(roleInfo.getInstance().sex) == 0 ?"男":"女";
        m_panel.FindChild("area").GetComponent<Text>().text = roleInfo.getInstance().region;
        m_panel.FindChild("account").GetComponent<Text>().text = roleInfo.getInstance().uid;
    }

    public void onLoginOut()
    {
        hide();
        testtool.panel_login.show();
        roleInfo.getInstance().dispose_storage();
        NeoGameSDK_CS.api_cb_loginout();
    }
}
