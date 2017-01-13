using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace _3DIT.Models
{
	public class SongQueryResult
	{
		public string AlbumId { get; set; }

		public string AlbumTitle { get; set; }

		public string AlbumYear { get; set; }

		public string ArtistName { get; set; }

		public int DiscNumber { get; set; }

		public string Genre { get; set; }

		public string SongTitle { get; set; }

		public int TrackCount { get; set; }

		public string TrackNumber { get; set; }

		public string AlbumCoverUrl
		{
			get
			{
				if (!string.IsNullOrEmpty(AlbumId))
				{
					return string.Format("http://coverartarchive.org/release/{0}/front", AlbumId);
				}
				return null;
			}
		}
	}
}