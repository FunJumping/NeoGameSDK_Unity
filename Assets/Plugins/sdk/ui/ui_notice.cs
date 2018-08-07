using UnityEngine;
using UnityEngine.UI;

public class ui_notice : basePanel
{

    Text txt_notcie;
    public ui_notice(Transform go)
    {
        m_panel = go;

        //提示ui
        txt_notcie = m_panel.FindChild("txt").GetComponent<Text>();
        m_panel.FindChild("bg").GetComponent<Button>().onClick.AddListener(() =>
        {
            hide();
        });
    }

    public void showNotice(string str)
    {
        m_panel.gameObject.SetActive(true);
        txt_notcie.text = str;
    }

}
