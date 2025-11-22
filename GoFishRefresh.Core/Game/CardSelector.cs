#region Using Statements
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using System;
#endregion
public class CardSelector
{
    MouseState MS, previousMS;
    private int drawIndex = 0; 
    private List<SelectableCard> selectableCards;
    private Vector2 position;
    public Card SelectedCard = null;
    public event EventHandler onCardSelected;
    public CardSelector()
    {
        selectableCards = new List<SelectableCard>();
        position = new Vector2(1400, 325);
        foreach (Card.Suits suit in System.Enum.GetValues(typeof(Card.Suits)))
        {
            foreach (Card.Ranks rank in System.Enum.GetValues(typeof(Card.Ranks)))
            {
                SelectableCard card = new SelectableCard(new Card(suit, rank), position);
                card.onSelect += (s, e) =>
                {
                    SelectedCard = card.Card;
                    onCardSelected?.Invoke(this, EventArgs.Empty);
                };
                selectableCards.Add(card);
            }
        }
    }
    
    public void Update(GameTime gameTime, GraphicsDeviceManager graphics)
    {
        if (selectableCards == null || selectableCards.Count == 0)
            return;
            
        MS = Mouse.GetState();

        int scrollDelta = MS.ScrollWheelValue - previousMS.ScrollWheelValue;
        if (scrollDelta < 0)
        {
            drawIndex = (drawIndex <= 0) ? selectableCards.Count - 1 : drawIndex - 1;
        }
        else if (scrollDelta > 0)
        {
            drawIndex = (drawIndex >= selectableCards.Count - 1) ? 0 : drawIndex + 1;
        }

        // Improved: Add bounds checking to prevent index out of range
        if (drawIndex >= 0 && drawIndex < selectableCards.Count)
        {
            selectableCards[drawIndex].UpdateSelection(MS, graphics);
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