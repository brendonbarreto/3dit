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

		public static List<SongQueryResult> GetResults(QueryRecordings qr)
		{
			List<SongQueryResult> results = new List<SongQueryResult>();

			if (qr != null)
			{
				var recordings = qr.recordings;
				if (recordings != null && recordings.Count() > 0)
				{
					foreach (var rec in recordings)
					{
						SongQueryResult result = new SongQueryResult();
						
						var artists = rec.artistcredit;
						result.SongTitle = rec.title;
						if (artists != null && artists.Count() > 0)
						{
							if (artists != null)
							{
								var artist = artists[0].artist;
								result.ArtistName = artist.name;
							}
						}

						var albums = rec.releases;
						if (albums != null && albums.Count() > 0)
						{
							var album = albums[0];
							if (album != null)
							{
								result.AlbumTitle = album.title;
								result.AlbumYear = album.date != null ? album.date.Substring(0, 4) : null;
								result.AlbumId = album.id;
								var b = album.media;
								if (b != null && b.Count() > 0)
								{
									var c = b[0];
									if (c != null)
									{
										//Numero do disco
										result.DiscNumber = c.position;
										//Número de musicas no disco
										result.TrackCount = c.trackcount;

										var e = c.track;
										if (e != null && e.Count() > 0)
										{
											var f = e[0];
											if (f != null)
											{
												//Numero da musica
												result.TrackNumber = f.number;
												//result.SongTitle = f.title;
											}
										}
									}
								}
							}
						}

						var tags = rec.tags;
						if (tags != null && tags.Count() > 0)
						{
							result.Genre = tags[0].name;
						}

						results.Add(result);
					}
				}
			}

			return results;
		}
	}
}