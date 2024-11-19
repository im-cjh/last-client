using System;
using UnityEngine;
using UnityEngine.Events;

public class UIBase : MonoBehaviour
{
    public enum eUIPosition
    {
        Default,
        Center,
        Top,
        Bottom
    }

    public eUIPosition uiPosition;
    public UIOptions uiOptions = new UIOptions();
    public UnityAction<object[]> opened;
    public UnityAction<object[]> closed;

    protected virtual void Awake()
    {
        opened = Opened;
        closed = Closed;
    }

    public virtual void Opened(object[] param)
    {
        if (uiOptions.isActiveOnLoad)
        {
            gameObject.SetActive(true);
        }
    }

    public virtual void Closed(object[] param = null)
    {
        gameObject.SetActive(false);

    }
}

[System.Serializable]
public class UIOptions
{
    public bool isActiveOnLoad = true;
    public bool isDestroyOnHide = true;
}
