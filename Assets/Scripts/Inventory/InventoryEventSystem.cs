using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.EventSystems;
using UnityEditor;

public class InventoryEventSystem : Singleton<InventoryEventSystem>
{
    public event EventHandler<InventoryEventArgs> OnInventoryChanged;

    public class InventoryEventArgs : EventArgs
    {
        public int ItemCount { get; set; }
    }

    public void InventoryChanged(int itemCount)
    {
        OnInventoryChanged?.Invoke(this, new InventoryEventArgs
        {
            ItemCount = itemCount
        });
    }
}
