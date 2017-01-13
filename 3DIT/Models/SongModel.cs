using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace _3DIT.Models
{
	public class SongModel
	{
		public string Title { get; set; }

		public string Artist { get; set; }

		public string Album { get; set; }

		public uint Year { get; set; }

		public uint TrackNumber { get; set; }

		public uint AlbumTrackLenght { get; set; }

		public uint DiscNumber { get; set; }

		public uint AlbumDiscLenght { get; set; }

		public string Genre { get; set; }

		public string Composer { get; set; }

		public string AlbumArt { get; set; }
	}
}