#region Using Statements
using Microsoft.Xna.Framework;
using System.Collections.Generic; 
#endregion

public class HandMatcher
{
    /// <summary>
    /// Highlighted cards in the player's hand get passed in here. 
    /// Method is used to determine the type of hand the player has selected, if any
    /// </summary>
    /// <param name="hand"></param>
    /// <param name="rank"></param>
    /// <returns></returns>
    public static HandType IsMatch(List<Card> hand)
    {
        if (hand.Count < 2)
            return HandType.None;

        if (hand.Count == 2)
        {
            if (hand[0].Rank == hand[1].Rank)
                return HandType.Pair;
        }

        if (hand.Count == 3)
        {
            if (hand[0].Rank == hand[1].Rank && hand[0].Rank == hand[2].Rank)
                return HandType.ThreeOfAKind;
        }

        if (hand.Count == 4)
        {
            if (hand[0].Rank == hand[1].Rank && hand[0].Rank == hand[2].Rank && hand[0].Rank == hand[3].Rank)
                return HandType.FourOfAKind;
        }

        if (hand.Count == 5)
        {
            if (hand[0].Rank == hand[1].Rank && hand[0].Rank == hand[2].Rank &&
                hand[3].Rank == hand[4].Rank)
                return HandType.FullHouse;

            int SpadeCount, HeartCount, ClubCount, DiamondCount;
            SpadeCount = HeartCount = ClubCount = DiamondCount = 0;
            int TwoCount, ThreeCount, FourCount, FiveCount, SixCount, SevenCount, EightCount, NineCount, TenCount, JackCount, QueenCount, KingCount, AceCount;
            TwoCount = ThreeCount = FourCount = FiveCount = SixCount = SevenCount = EightCount = NineCount = TenCount = JackCount = QueenCount = KingCount = AceCount = 0;

            for (int i = 0; i < hand.Count; i++)
            {
                if (hand[i].Suit == Card.Suits.Spades) SpadeCount++;
                if (hand[i].Suit == Card.Suits.Hearts) HeartCount++;
                if (hand[i].Suit == Card.Suits.Clubs) ClubCount++;
                if (hand[i].Suit == Card.Suits.Diamonds) DiamondCount++;

                if (hand[i].Rank == Card.Ranks.Two) TwoCount++;
                if (hand[i].Rank == Card.Ranks.Three) ThreeCount++;
                if (hand[i].Rank == Card.Ranks.Four) FourCount++;
                if (hand[i].Rank == Card.Ranks.Five) FiveCount++;
                if (hand[i].Rank == Card.Ranks.Six) SixCount++;
                if (hand[i].Rank == Card.Ranks.Seven) SevenCount++;
                if (hand[i].Rank == Card.Ranks.Eight) EightCount++;
                if (hand[i].Rank == Card.Ranks.Nine) NineCount++;
                if (hand[i].Rank == Card.Ranks.Ten) TenCount++;
                if (hand[i].Rank == Card.Ranks.Jack) JackCount++;
                if (hand[i].Rank == Card.Ranks.Queen) QueenCount++;
                if (hand[i].Rank == Card.Ranks.King) KingCount++;
                if (hand[i].Rank == Card.Ranks.Ace) AceCount++;

                if (TenCount > 0 && JackCount > 0 && QueenCount > 0 && KingCount > 0 && AceCount > 0)
                {
                    if (SpadeCount == 5 || HeartCount == 5 || ClubCount == 5 || DiamondCount == 5)
                        return HandType.RoyalFlush;
                }

                if ((TwoCount > 0 && ThreeCount > 0 && FourCount > 0 && FiveCount > 0 && SixCount > 0) ||
                    (ThreeCount > 0 && FourCount > 0 && FiveCount > 0 && SixCount > 0 && SevenCount > 0) ||
                    (FourCount > 0 && FiveCount > 0 && SixCount > 0 && SevenCount > 0 && EightCount > 0) ||
                    (FiveCount > 0 && SixCount > 0 && SevenCount > 0 && EightCount > 0 && NineCount > 0) ||
                    (SixCount > 0 && SevenCount > 0 && EightCount > 0 && NineCount > 0 && TenCount > 0) ||
                    (SevenCount > 0 && EightCount > 0 && NineCount > 0 && TenCount > 0 && JackCount > 0) ||
                    (EightCount > 0 && NineCount > 0 && TenCount > 0 && JackCount > 0 && QueenCount > 0) ||
                    (NineCount > 0 && TenCount > 0 && JackCount > 0 && QueenCount > 0 && KingCount > 0) ||
                    (TenCount > 0 && JackCount > 0 && QueenCount > 0 && KingCount > 0 && AceCount > 0))
                {
                    if (SpadeCount == 5 || HeartCount == 5 || ClubCount == 5 || DiamondCount == 5)
                        return HandType.StraightFlush;
                    else
                        return HandType.Straight;
                }

                if (SpadeCount == 5 || HeartCount == 5 || ClubCount == 5 || DiamondCount == 5)
                {
                    return HandType.Flush;
                }
            }

        }


        return HandType.None; 
    }

    public enum HandType
    {
        None,
        Pair,
        ThreeOfAKind,
        FourOfAKind,
        FullHouse,
        Straight,
        Flush,
        StraightFlush,
        RoyalFlush
    }
}