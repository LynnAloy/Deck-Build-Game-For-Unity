using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetHeroSystem : MonoBehaviour
{
    [SerializeField] private HeroView heroView;
    [SerializeField] private HeroData heroData;

    public void ResetHero()
    {
        heroView.Setup(heroData);
    }
}
