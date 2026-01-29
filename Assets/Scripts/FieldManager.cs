using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mono.Cecil;

public class FieldManager : MonoBehaviour



{
    [SerializeField]
    private FieldCardView fieldCardView;

    private List<CardData> fieldCards = new List<CardData>();

    public List<CardData> FieldCards => fieldCards;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
    public void Initialieze()
    {
        fieldCards.Clear();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void AddCard(CardData card)
    {
        fieldCards.Add(card);
        fieldCardView.UpdateImage(card);
        
    }
    
        
    
}
