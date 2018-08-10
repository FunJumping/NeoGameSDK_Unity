using UnityEngine;
using UnityEngine.UI;

public class ui_addressCode : basePanel
{
   
    public ui_addressCode(Transform go)
    {
        m_panel = go;

        m_panel.FindChild("return").GetComponent<Button>().onClick.AddListener(() =>
        {
            hide();
            testtool.panel_main.show();
        });
        m_panel.FindChild("copy").GetComponent<Button>().onClick.AddListener(onCopy);
    }

    public override void show()
    {
        m_panel.gameObject.SetActive(true);
        refreshInfo();
    }

    public void refreshInfo()
    {
        m_panel.FindChild("txt_address").GetComponent<Text>().text = roleInfo.getInstance().address;
    }

    public void onCopy()
    {
#if UNITY_ANDROID
        Debug.Log("这里是安卓设备^_^");
#endif

#if UNITY_IPHONE
        Debug.Log("这里是苹果设备>_<");
#endif

#if UNITY_STANDALONE_WIN
        TextEditor te = new TextEditor();
        te.content = new GUIContent(roleInfo.getInstance().address);
        te.SelectAll();
        te.Copy();
#endif
    }
}
