using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Game
{
	public class Card
	{
		public Number Number { get; set; }
		public Suit Suit { get; set; }

 

        public override string ToString()
		{
			return $"{Number} of {Suit}";
		}
	}
}
