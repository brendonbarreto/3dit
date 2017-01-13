using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicBrainzAPI.Json
{
	public class Medium
	{
		public int Position { get; set; }

		public string Format { get; set; }

		public Track[] Track { get; set; }

		[JsonProperty(PropertyName = "track-count")]
		public int TrackCount { get; set; }

		public int TrackOffset { get; set; }
	}
}
