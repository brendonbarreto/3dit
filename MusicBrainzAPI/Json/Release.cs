using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicBrainzAPI.Json
{
	public class Release
	{
		public string Id { get; set; }

		public string Title { get; set; }

		public string Status { get; set; }

		[JsonProperty(PropertyName = "artist-credit")]
		public ArtistCredit[] ArtistCredit { get; set; }

		[JsonProperty(PropertyName = "release-group")]
		public ReleaseGroup ReleaseGroup { get; set; }

		public string Date { get; set; }

		public string Country { get; set; }

		[JsonProperty(PropertyName = "release-events")]
		public ReleaseEvents[] ReleaseEvents { get; set; }

		[JsonProperty(PropertyName = "track-count")]
		public int TrackCount { get; set; }

		public Medium[] Media { get; set; }
	}
}
