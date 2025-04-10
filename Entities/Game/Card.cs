using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Game
{
	public class Card
	{
		public Number number;
		public Suit suit;

		public override string ToString()
		{
			return $"{number} of {suit}";
		}
	}
}
