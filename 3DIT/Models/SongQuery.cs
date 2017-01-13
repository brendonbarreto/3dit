using System;
using System.Collections.Generic;
using System.Linq;
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



