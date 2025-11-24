using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EnemyCardView : MonoBehaviour
{
    [SerializeField] private TMP_Text title;
    [SerializeField] private TMP_Text description;
    [SerializeField] private TMP_Text mana;
    [SerializeField] private SpriteRenderer imageSR;
    [SerializeField] private GameObject wrapper;
    [SerializeField] private LayerMask dropLayer;

    public Card Card { get; private set; }


    public void Setup(Card card)
    {
        if (card == null)
        {
            Debug.LogError("≥¢ ‘…Ë÷√ø’ø®≈∆ ”Õº£°");
            return;
        }
        Card = card;
        if (title != null) title.text = card.Title ?? "";
        if (description != null) description.text = card.Description ?? "";
        if (mana != null) mana.text = card.Mana.ToString() ?? "";
        if (imageSR != null) imageSR.sprite = card.Image;
        else
        {
            Debug.LogError("ImageŒ™ø’");
            return;
        }
    }

    private void OnMouseEnter()
    {
        if (!Interactions.Instance.PlayerCanHover()) return;
        wrapper.SetActive(false);
        Vector3 pos = new(transform.position.x, -2, 0);
        CardViewHoverSystem.Instance.Show(Card, pos);
    }

    private void OnMouseExit()
    {
        if (!Interactions.Instance.PlayerCanHover()) return;
        CardViewHoverSystem.Instance.Hide();
        wrapper.SetActive(true);
    }

}
