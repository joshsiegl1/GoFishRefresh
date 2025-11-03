#region Using Statements
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endregion 

public class PlayedCards
{
    public List<List<Card>> CardsPlayed { get; private set; }

    public PlayedCards()
    {
        CardsPlayed = new List<List<Card>>();
    }

    public void AddNewHandPlayed(List<Card> hand)
    {
        CardsPlayed.Add(hand);
    }
}