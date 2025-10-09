/*
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace CardSystem
{
    public class Card : MonoBehaviour
    {
        private int amount;
        private void OnMouseDown()
        {
            if (ActionSystem.Instance.IsPerforming) return;
            DrawCardGA drawCardGA = new(amount);
            ActionSystem.Instance.Perform(drawCardGA);
            Destroy(gameObject);
        }
    }
}
*/