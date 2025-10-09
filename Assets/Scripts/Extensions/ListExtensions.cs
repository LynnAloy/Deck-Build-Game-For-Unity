using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CardGame.Extensions
{
    public static class ListExtensions
    {
        public static T Draw<T>(this List<T> list)
        {
            if (list.Count == 0 || list == null)
            {
                Debug.LogWarning("尝试从空列表中抽取元素！");
                return default;
            }
            int r = Random.Range(0, list.Count);
            T t = list[r];
            list.RemoveAt(r);
            return t;
        }
    }
}
