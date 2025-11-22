#region Using Statements
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
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
            if (hand[0].Rank == hand[1].Rank && hand[2].Rank == hand[3].Rank 
                || hand[0].Rank == hand[2].Rank && hand[1].Rank == hand[3].Rank 
                || hand[0].Rank == hand[3].Rank && hand[1].Rank == hand[2].Rank)
                return HandType.TwoPair;
        }

        if (hand.Count == 5)
        {
            // Improved: Use LINQ GroupBy instead of verbose individual counters
            var suitGroups = hand.GroupBy(c => c.Suit).ToList();
            var rankGroups = hand.GroupBy(c => c.Rank).ToList();
            
            bool isFlush = suitGroups.Any(g => g.Count() == 5);
            var rankCounts = rankGroups.Select(g => g.Count()).OrderByDescending(c => c).ToList();
            
            // Check for Royal Flush (A, K, Q, J, 10 of same suit)
            var ranks = hand.Select(c => (int)c.Rank).ToList();
            bool isRoyalFlush = isFlush && 
                ranks.Contains((int)Card.Ranks.Ten) &&
                ranks.Contains((int)Card.Ranks.Jack) &&
                ranks.Contains((int)Card.Ranks.Queen) &&
                ranks.Contains((int)Card.Ranks.King) &&
                ranks.Contains((int)Card.Ranks.Ace);
            
            if (isRoyalFlush)
                return HandType.RoyalFlush;
            
            // Check for Straight Flush or Straight
            bool isStraight = IsStraight(ranks);
            if (isStraight && isFlush)
                return HandType.StraightFlush;
            if (isStraight)
                return HandType.Straight;
            
            // Check for Flush
            if (isFlush)
                return HandType.Flush;
            
            // Check for Full House (3 of a kind + pair)
            if (rankCounts.Count >= 2 && rankCounts[0] == 3 && rankCounts[1] == 2)
                return HandType.FullHouse;
        }


        return HandType.None;
    }
    
    // Improved: Helper method to check for straights
    private static bool IsStraight(List<int> ranks)
    {
        if (ranks.Count != 5) return false;
        
        // Handle Ace-low straight (A, 2, 3, 4, 5)
        if (ranks.Contains((int)Card.Ranks.Ace) && 
            ranks.Contains((int)Card.Ranks.Two) &&
            ranks.Contains((int)Card.Ranks.Three) &&
            ranks.Contains((int)Card.Ranks.Four) &&
            ranks.Contains((int)Card.Ranks.Five))
        {
            return true;
        }
        
        // Check for regular straights
        ranks = ranks.OrderBy(r => r).ToList();
        for (int i = 1; i < ranks.Count; i++)
        {
            if (ranks[i] != ranks[i - 1] + 1)
                return false;
        }
        return true;
    }
    
    public static string ToString(HandType handType)
    {
        switch (handType)
        {
            case HandType.None:
                return "No Match";
            case HandType.Pair:
                return "Pair";
            case HandType.ThreeOfAKind:
                return "Three of a Kind";
            case HandType.FourOfAKind:
                return "Four of a Kind";
            case HandType.TwoPair:
                return "Two Pair";
            case HandType.FullHouse:
                return "Full House";
            case HandType.Straight:
                return "Straight";
            case HandType.Flush:
                return "Flush";
            case HandType.StraightFlush:
                return "Straight Flush";
            case HandType.RoyalFlush:
                return "Royal Flush";
            default:
                return "";
        }
    }

    public enum HandType
    {
        None,
        Pair,
        ThreeOfAKind,
        FourOfAKind,
        TwoPair, 
        FullHouse,
        Straight,
        Flush,
        StraightFlush,
        RoyalFlush
    }
}