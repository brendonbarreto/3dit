using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicBrainzAPI.Json
{
	public class Recording
	{
		public string Id { get; set; }

		public string Score { get; set; }

		public string Title { get; set; }

		public object Video { get; set; }

		[JsonProperty(PropertyName = "artist-credit")]
		public ArtistCredit[] ArtistCredit { get; set; }

		public Release[] Releases { get; set; }

		public Tag[] Tags { get; set; }

		public int Length { get; set; }
	}
}
