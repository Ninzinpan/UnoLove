using UnityEngine;
using System.Collections.Generic;

class  DuelistManager : MonoBehaviour
{
    [SerializeField]
    private DeckManager deckManager;

    [SerializeField]
    private HandManager handManager;

    [SerializeField]
    private BaseDeckData baseDeckData;

    public List<CardData> StartingDeck = new List<CardData>();

    private void Awake()
    {
        StartingDeck = new List<CardData>(baseDeckData.BaseDeckCards);
    }
}