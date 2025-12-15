using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Inventory/HealthModifierSO")]
public class CharacterStateHealthModifierSO : CharacterStateModifierSO
{
    public override void AffectCharacter(CombatantView character, int val)
    {
        if(character == null)
        {
            Debug.LogError("CharacterStateHealthModifierSO: character is null.");
            return;
        }
        character.Heal(val);
    }
}
