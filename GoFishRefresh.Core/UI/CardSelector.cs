#region Using Statements
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
#endregion
public class CardSelector
{
    MouseState MS, previousMS;
    private int drawIndex = 0; 
    private List<SelectableCard> selectableCards;
    private Vector2 position;
    public CardSelector()
    {
        selectableCards = new List<SelectableCard>();
        position = new Vector2(1400, 325);
        foreach (Card.Suits suit in System.Enum.GetValues(typeof(Card.Suits)))
        {
            foreach (Card.Ranks rank in System.Enum.GetValues(typeof(Card.Ranks)))
            {
                selectableCards.Add(new SelectableCard(new Card(suit, rank), position));
            }
        }
    }

    public void Update(GameTime gameTime, GraphicsDeviceManager graphics)
    {
        MS = Mouse.GetState();

        int scrollDelta = MS.ScrollWheelValue - previousMS.ScrollWheelValue;
        if (scrollDelta < 0)
        {
            if (drawIndex <= 0)
            {
                drawIndex = Deck.LIMIT - 1;
            }
            else drawIndex--;
        }
        if (scrollDelta > 0)
        {
            if (drawIndex >= Deck.LIMIT - 1)
            {
                drawIndex = 0;
            }
            else drawIndex++;
        }
        previousMS = MS;
    }
    public void LoadContent(ContentManager Content)
    {
        foreach (var card in selectableCards)
        {
            card.LoadContent(Content);
        }
    }
    public void Draw(SpriteBatch spriteBatch)
    {
        selectableCards[drawIndex].Draw(spriteBatch);
    }
}