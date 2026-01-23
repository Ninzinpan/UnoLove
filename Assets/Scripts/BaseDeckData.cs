using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewBaseDeck", menuName = "UnoLovePrototype/BaseDeckData")]
public class BaseDeckData : ScriptableObject
{
    [Header("Base Deck Configuration")]
    [SerializeField]
    private List<CardData> baseDeckCards = new List<CardData>();

    public List<CardData> BaseDeckCards => baseDeckCards;
    }