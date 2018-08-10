using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;
using Common;
public class basePanel
{
    public Transform m_panel;

    public virtual void show()
    {
        m_panel.gameObject.SetActive(true);
    }
    public virtual void hide()
    {
        m_panel.gameObject.SetActive(false);
    }
}
