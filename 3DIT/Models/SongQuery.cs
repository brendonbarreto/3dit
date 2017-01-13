using MusicBrainzAPI.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace _3DIT.Models
{
	public class SongQuery
	{
		public SongQuery()
		{
			Results = new List<SongQueryResult>();
			SearchFields = new List<LuceneBooleanField>();
			SearchOptions = new Dictionary<string, string>();
		}

		public SongApi Api { get; set; }

		public List<SongQueryResult> Results { get; set; }

		private List<LuceneBooleanField> SearchFields { get; set; }

		public Dictionary<string, string> SearchOptions { get; set; }

		public string UserAgent { get; set; }

		public string ApiUrl
		{
			get
			{
				switch (Api)
				{
					case SongApi.MusicBrainz:
						return "http://musicbrainz.org/ws/2/recording/?query";
					default:
						return null;
				}
			}
		}

		public void AddSearchField(LuceneBooleanCondition condition, string key, string value, bool escapeData)
		{
			SearchFields.Add(new LuceneBooleanField()
			{
				Condition = condition,
				FieldPair = new KeyValuePair<string, string>(key, escapeData ? Uri.EscapeDataString(value) : value)
			});
		}

		public void Search()
		{
			var url = GetSearchUrl();
			string json = null;
			using (var webClient = new WebClient())
			{
				webClient.Headers.Add("user-agent", UserAgent);
				json = webClient.DownloadString(url);
			}

			QueryRecordings qr = JsonConvert.DeserializeObject<QueryRecordings>(json, new JsonSerializerSettings()
			{
				ContractResolver = new CamelCasePropertyNamesContractResolver()
			});
			SetResultsFromJson(qr);
		}

		private void SetResultsFromJson(QueryRecordings qr)
		{ 
			if (qr != null)
			{
				var recordings = qr.Recordings;
				if (recordings != null && recordings.Count() > 0)
				{
					foreach (var rec in recordings)
					{
						SongQueryResult result = new SongQueryResult();

						var artists = rec.ArtistCredit;
						result.SongTitle = rec.Title;
						if (artists != null && artists.Count() > 0)
						{
							if (artists != null)
							{
								var artist = artists[0].Artist;
								result.ArtistName = artist.Name;
							}
						}

						var albums = rec.Releases;
						if (albums != null && albums.Count() > 0)
						{
							var album = albums[0];
							if (album != null)
							{
								result.AlbumTitle = album.Title;
								result.AlbumYear = album.Date != null ? album.Date.Substring(0, 4) : null;
								result.AlbumId = album.Id;
								var b = album.Media;
								if (b != null && b.Count() > 0)
								{
									var c = b[0];
									if (c != null)
									{
										result.DiscNumber = c.Position;
										result.TrackCount = c.TrackCount;

										var e = c.Track;
										if (e != null && e.Count() > 0)
										{
											var f = e[0];
											if (f != null)
											{
												result.TrackNumber = f.Number;
											}
										}
									}
								}
							}
						}

						var tags = rec.Tags;
						if (tags != null && tags.Count() > 0)
						{
							result.Genre = tags[0].Name;
						}

						Results.Add(result);
					}
				}
			}
		}

		private string GetSearchUrl()
		{
			var builder = new StringBuilder();
			builder.Append(string.Concat(ApiUrl, "="));
			for(int i = 0; i < SearchFields.Count; i++)
			{
				var sf = SearchFields[i];
				if(i > 0)
				{
					builder.Append(string.Concat(sf.Condition, "%20"));
				}

				builder.AppendFormat("{0}:\"{1}\"%20", sf.FieldPair.Key, sf.FieldPair.Value);
			}

			for(int i = 0; i < SearchOptions.Count; i++)
			{
				var so = SearchOptions.ElementAt(i);
				builder.AppendFormat("&{0}={1}", so.Key, so.Value);
			}

			return builder.ToString();
		}

	}

	public class LuceneBooleanField
	{
		public LuceneBooleanCondition Condition { get; set; }

		public KeyValuePair<string, string> FieldPair { get; set; }
	}

	public enum LuceneBooleanCondition
	{
		AND, NOT
	}
}


//public List<SongQueryResult> GetResults()
//{

//}



