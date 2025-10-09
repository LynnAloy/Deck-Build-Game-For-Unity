using System.Collections;
using System.Collections.Generic;
using Unity.Properties;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Hero")]

public class HeroData : ScriptableObject
{
    [field: SerializeField] public Sprite Image { get; private set; }
    [field: SerializeField] public int Health { get; private set; }
    [field: SerializeField] public int BaseATK { get; private set; }
    [field: SerializeField] public List<CardData> Deck { get; private set; }

}
