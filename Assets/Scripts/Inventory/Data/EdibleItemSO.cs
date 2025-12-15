using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

namespace Inventory.Model
{
    [CreateAssetMenu(menuName = "Inventory/EdibleItem")]
    public class EdibleItemSO : ItemSO, IDestoryableItem, IItemAction
    {
        [SerializeField] private List<ModifierData> modifiersData = new();
        public string ActionName => "Consume";

        public AudioClip ActionSFX { get; private set; }

        public bool PerformAction(CombatantView character, List<ItemParameter> itemState = null)
        {
            if(character == null)
            {
                Debug.LogError("EdibleItemSO.PerformAction: character is null.");
                return false;
            }
            foreach (var data in modifiersData)
            {
                data.stateModifier.AffectCharacter(character, data.value);
            }
            return true;
        }
    }




    public interface IDestoryableItem
    {

    }

    public interface IItemAction
    {
        public string ActionName { get; }
        //public AudioClip ActionSFX { get; }
        bool PerformAction(CombatantView character, List<ItemParameter> itemState);

    }

    [Serializable]
    public class ModifierData
    {
        public CharacterStateModifierSO stateModifier;
        public int value;
    }
}