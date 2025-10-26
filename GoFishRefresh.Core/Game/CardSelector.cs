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
    bool isCardChecked = false; 
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
                    isCardChecked = false; 
                    onCardSelected?.Invoke(this, EventArgs.Empty);
                };
                selectableCards.Add(card);
            }
        }
    }
    
    public void CheckSelection()
    {
        if (!isCardChecked && SelectedCard != null)
        {
            isCardChecked = true; 
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

        selectableCards[drawIndex].UpdateSelection(MS, graphics);

        CheckSelection();

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