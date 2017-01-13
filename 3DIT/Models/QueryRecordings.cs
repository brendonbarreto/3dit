using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace _3DIT.Models
{
	public class QueryRecordings
	{
		public DateTime created { get; set; }
		public int count { get; set; }
		public int offset { get; set; }
		public Recording[] recordings { get; set; }
	}

	public class Recording
	{
		public string id { get; set; }

		public string score { get; set; }

		public string title { get; set; }

		public object video { get; set; }

		[JsonProperty(PropertyName = "artist-credit")]
		public ArtistCredit[] artistcredit { get; set; }

		public Release[] releases { get; set; }

		public Tag[] tags { get; set; }

		public int length { get; set; }
	}

	public class ArtistCredit
	{
		public Artist artist { get; set; }
		public string joinphrase { get; set; }
	}

	public class Artist
	{
		public string id { get; set; }
		public string name { get; set; }

		[JsonProperty(PropertyName = "sort-name")]
		public string sortname { get; set; }
	}

	public class Release
	{
		public string id { get; set; }
		public string title { get; set; }
		public string status { get; set; }

		[JsonProperty(PropertyName = "artist-credit")]
		public ArtistCredit1[] artistcredit { get; set; }

		[JsonProperty(PropertyName = "release-group")]
		public ReleaseGroup releasegroup { get; set; }

		public string date { get; set; }
		public string country { get; set; }

		[JsonProperty(PropertyName = "release-events")]
		public ReleaseEvents[] releaseevents { get; set; }

		[JsonProperty(PropertyName = "track-count")]
		public int trackcount { get; set; }

		public Medium[] media { get; set; }
	}

	public class ReleaseGroup
	{
		public string id { get; set; }

		[JsonProperty(PropertyName = "primary-type")]
		public string primarytype { get; set; }

		[JsonProperty(PropertyName = "secondary-types")]
		public string[] secondarytypes { get; set; }
	}

	public class ArtistCredit1
	{
		public Artist1 artist { get; set; }

		[JsonProperty(PropertyName = "join-phrase")]
		public string joinphrase { get; set; }
	}

	public class Artist1
	{
		public string id { get; set; }
		public string name { get; set; }

		[JsonProperty(PropertyName = "sort-name")]
		public string sortname { get; set; }
		public string disambiguation { get; set; }
	}

	public class ReleaseEvents
	{
		public string date { get; set; }
		public Area area { get; set; }
	}

	public class Area
	{
		public string id { get; set; }
		public string name { get; set; }

		[JsonProperty(PropertyName = "sort-name")]
		public string sortname { get; set; }
		//public string[] iso31661codes { get; set; }
	}

	public class Medium
	{
		public int position { get; set; }

		public string format { get; set; }

		public Track[] track { get; set; }

		[JsonProperty(PropertyName = "track-count")]
		public int trackcount { get; set; }

		public int trackoffset { get; set; }
	}

	public class Track
	{
		public string id { get; set; }

		public string number { get; set; }

		public string title { get; set; }

		public int length { get; set; }
	}

	public class Tag
	{
		public int count { get; set; }

		public string name { get; set; }
	}

}