using System.Collections.Generic;
using UnityEngine;

public abstract class UIListBase<T> : MonoBehaviour where T : MonoBehaviour
{
    [SerializeField] protected Transform listParent;
    [SerializeField] protected T itemPrefab;
    protected List<T> items = new List<T>();

    public T AddItem()
    {
        var item = Instantiate(itemPrefab, listParent);
        items.Add(item);
        return item;
    }

    public void ClearList()
    {
        foreach (var obj in items)
        {
            Destroy(obj.gameObject);
        }
        items.Clear();
    }

    public virtual void HideDirect()
    {
        gameObject.SetActive(false);
    }

    public virtual void Opened(object[] param)
    {
        gameObject.SetActive(true);
    }

    public abstract void SetList();
}
