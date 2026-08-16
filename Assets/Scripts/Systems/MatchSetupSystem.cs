using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchSetupSystem : MonoBehaviour
{
    [SerializeField] private HeroData heroData;
    [SerializeField] private List<EnemyData> enemyDatas;
    [SerializeField] private PerkData perkData;

    public bool IsInitialized { get; private set; }

    private IEnumerator Start()
    {
        Debug.Log("[MatchSetup] Start entered.");

        if (HotUpdateBootstrap.Instance != null)
        {
            Debug.Log(
                $"[MatchSetup] Waiting for hot update. " +
                $"Ready={HotUpdateBootstrap.Instance.IsReady}"
            );

            yield return new WaitUntil(
                () => HotUpdateBootstrap.Instance == null ||
                      HotUpdateBootstrap.Instance.IsReady
            );
        }

        Debug.Log("[MatchSetup] Hot update ready. Initializing match.");

        HeroSystem.Instance.Setup(heroData);
        Debug.Log("[MatchSetup] Hero initialized.");

        CardSystem.Instance.Setup(heroData.Deck);
        Debug.Log("[MatchSetup] Player deck initialized.");

        EnemySystem.Instance.Setup(enemyDatas);
        Debug.Log("[MatchSetup] Enemies initialized.");

        Debug.Log(
            $"[MatchSetup] Starting initial draw. " +
            $"ActionSystem.IsPerforming={ActionSystem.Instance.IsPerforming}"
        );

        DrawCardGA drawCardGA = new(5);
        ActionSystem.Instance.Perform(drawCardGA);

        PerkSystem.Instance.AddPerk(new Perk(perkData));
        IsInitialized = true;
        Debug.Log("[MatchSetup] Setup finished.");
    }
}
