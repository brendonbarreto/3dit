using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicBrainzAPI.Json
{
	public class QueryRecordings
	{
		public DateTime Created { get; set; }

		public int Count { get; set; }

		public int Offset { get; set; }

		public Recording[] Recordings { get; set; }
	}
}
