using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Game
{
	public class Pot
	{
		public float Amount { get; set; }
		public List<Player> Players { get; set; }
	}
}
