using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicBrainzAPI.Json
{
	public class ReleaseGroup
	{
		public string Id { get; set; }

		[JsonProperty(PropertyName = "primary-type")]
		public string PrimaryType { get; set; }

		[JsonProperty(PropertyName = "secondary-types")]
		public string[] SecondaryTypes { get; set; }
	}
}
