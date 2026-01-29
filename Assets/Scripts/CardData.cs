using UnityEngine;
using System.Collections.Generic;

public enum CardColor
{
    Red,
    Green,
    Blue,
    Grey
}
public enum CardType
{
    Circle,
    Square,
    Triangle,
    NoneShape
}

[CreateAssetMenu(fileName = "NewCard", menuName = "UnoLovePrototype/CardData")]

public class CardData : ScriptableObject
{
    [Header("Card Properties")]
    [SerializeField]
    private CardColor color;
    [SerializeField]
    private CardType type;
    [SerializeField]
    private Sprite cardImage;

    public CardColor Color => color;
    public CardType Type => type;
    public Sprite CardImage => cardImage;
}