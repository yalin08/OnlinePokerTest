using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Game
{
	public enum HandRank
	{
		HighCard = 1,
		OnePair,
		TwoPair,
		ThreeOfAKind,
		Straight,
		Flush,
		FullHouse,
		FourOfAKind,
		StraightFlush,
		RoyalFlush
	}
	public  class HandEvaluator
	{
		public static HandRank EvaluateHand(List<Card> hand)
		{
			var sortedHand = hand.OrderByDescending(card => card.Number).ToList();

			bool isFlush = IsFlush(hand);
			bool isStraight = IsStraight(sortedHand);

			if (isFlush && isStraight)
			{
				if (sortedHand[0].Number == Number.Ace)
					return HandRank.RoyalFlush;
				return HandRank.StraightFlush;
			}

			var grouped = hand.GroupBy(card => card.Number).OrderByDescending(group => group.Count()).ThenByDescending(group => group.Key).ToList();
			int highestGroupCount = grouped.First().Count();

			if (highestGroupCount == 4)
				return HandRank.FourOfAKind;

			if (highestGroupCount == 3)
			{
				if (grouped.Count > 1 && grouped[1].Count() == 2)
					return HandRank.FullHouse;
				return HandRank.ThreeOfAKind;
			}

			if (highestGroupCount == 2)
			{
				if (grouped.Count > 1 && grouped[1].Count() == 2)
					return HandRank.TwoPair;
				return HandRank.OnePair;
			}

			if (isFlush)
				return HandRank.Flush;

			if (isStraight)
				return HandRank.Straight;

			return HandRank.HighCard;
		}

		private static bool IsFlush(List<Card> hand)
		{
			return hand.All(card => card.Suit == hand[0].Suit);
		}

		private static bool IsStraight(List<Card> sortedHand)
		{
			for (int i = 0; i < sortedHand.Count - 1; i++)
			{
				if (sortedHand[i].Number != sortedHand[i + 1].Number + 1)
					return false;
			}
			return true;
		}

		public static Player DetermineWinningPlayer(List<Player> PlayersInGame)
		{
			Player winning = null;
			HandRank winningHand =HandRank.HighCard;
			foreach (Player player in PlayersInGame)
			{
				var handrank = GetBestHand(player.Cards, player.Table.CommunityCards);

				if (winning == null || handrank > winningHand)
				{
					// Eğer kazanan yoksa ya da yeni elde daha güçlü bir el varsa
					winning = player;
					winningHand = handrank;
				}
			}

			return winning;
		}
		public static HandRank GetBestHand(List<Card> playerCards, List<Card> communityCards)
		{
			// Oyuncunun kartları ile community kartlarını birleştir
			List<Card> combinedCards = playerCards.Concat(communityCards).ToList();

			// En güçlü eli belirle
			return EvaluateHand(combinedCards);
		}
	}
}
