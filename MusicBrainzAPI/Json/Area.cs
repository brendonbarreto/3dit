﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicBrainzAPI.Json
{
	public class Area
	{
		public string Id { get; set; }

		public string Name { get; set; }

		[JsonProperty(PropertyName = "sort-name")]
		public string SortName { get; set; }
		//public string[] iso31661codes { get; set; }
	}
}
