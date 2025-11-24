using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Enemy")]
public class EnemyData : ScriptableObject
{
    [field: SerializeField] public Sprite Image { get; private set; }
    [field: SerializeField] public Sprite Properity_1 { get; private set; }
    //[field: SerializeField] public bool InitProperity_1 { get; private set; }
    [field: SerializeField] public Sprite Properity_2 { get; private set; }
    //[field: SerializeField] public bool InitProperity_2 { get; private set; }
    [field: SerializeField] public int Health { get; private set; }
    [field: SerializeField] public int BaseATK { get; private set; }
    [field: SerializeField] public int AttackPower { get; private set; }
    [field: SerializeField] public List<CardData> Deck { get; private set; }
}
