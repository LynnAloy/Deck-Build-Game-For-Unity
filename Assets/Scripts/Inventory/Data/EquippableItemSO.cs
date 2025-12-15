using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Inventory.Model

{
    [CreateAssetMenu(menuName = "Inventory/EquippableItem")]
    public class EquippableItemSO : ItemSO, IDestoryableItem, IItemAction
    {
        public string ActionName => "Equip";
        public AudioClip ActionSFX { get; private set; }
        public bool PerformAction(CombatantView character, List<ItemParameter> itemState = null)
        {
            if (character == null)
            {
                Debug.LogError("EquippableItemSO.PerformAction: character is null.");
                return false;
            }
            // Implement the logic to equip the item to the character
            // For example, you might want to change the character's appearance or stats
            return true;
        }

    }
}