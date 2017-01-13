using _3DIT.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using TagLib;

namespace _3DIT.Controllers
{
	public class HomeController : Controller
	{
		public ActionResult Index()
		{
			return View();
		}

		public ActionResult SearchSongPopup()
		{
			return PartialView();
		}

		public JsonResult SearchSong(string title, string artist)
		{
			//removed %20AND%20primarytype:album%20AND%20status:official
			//removed %20NOT%20secondarytype:Live%20NOT%20secondarytype:Compilation
			string url = string.Format("http://musicbrainz.org/ws/2/recording/?query={0}%20AND%20artist:\"{1}\"%20NOT%20secondarytype:Live&limit=10&fmt=json",
				Uri.EscapeDataString(title),
				Uri.EscapeDataString(artist));
			string json;
			using (var webClient = new WebClient())
			{
				webClient.Headers.Add("user-agent", "apitest/1.0 ( brendonbarreto@hotmail.com )");
				json = webClient.DownloadString(url);
			}

			QueryRecordings qr = JsonConvert.DeserializeObject<QueryRecordings>(json);
			List<SongQueryResult> results = SongQueryResult.GetResults(qr);
			return Json(results);
		}

		public byte[] ResizeImage(byte[] imageBytes, int width, int height)
		{
			ImageConverter converter = new ImageConverter();
			Image image = (Bitmap)converter.ConvertFrom(imageBytes);

			var destRect = new Rectangle(0, 0, width, height);
			var destImage = new Bitmap(width, height);

			destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

			using (var graphics = Graphics.FromImage(destImage))
			{
				graphics.CompositingMode = CompositingMode.SourceCopy;
				graphics.CompositingQuality = CompositingQuality.HighQuality;
				graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
				graphics.SmoothingMode = SmoothingMode.HighQuality;
				graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

				using (var wrapMode = new ImageAttributes())
				{
					wrapMode.SetWrapMode(WrapMode.TileFlipXY);
					graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
				}
			}


			return (byte[])converter.ConvertTo(destImage, typeof(byte[]));
		}

		public string GetJoined(string[] list)
		{
			if (list != null && list.Count() > 0)
			{
				return string.Join(", ", list);
			}

			return string.Empty;
		}

		public string[] SetJoined(string text)
		{
			if (!string.IsNullOrEmpty(text))
			{
				string[] list = text.Split(',');

				for (int i = 0; i < list.Count(); i++)
				{
					string s = list[i];
					s = s.Trim();
				}

				return list;
			}
			return new string[0];
		}

		public string GetUniqueKey(int maxSize)
		{
			char[] chars = new char[62];
			chars =
			"abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".ToCharArray();
			byte[] data = new byte[1];
			using (RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider())
			{
				crypto.GetNonZeroBytes(data);
				data = new byte[maxSize];
				crypto.GetNonZeroBytes(data);
			}
			StringBuilder result = new StringBuilder(maxSize);
			foreach (byte b in data)
			{
				result.Append(chars[b % (chars.Length)]);
			}
			return result.ToString();
		}

		public JsonResult GetSongFromURL(string url)
		{
			try
			{
				string path = null;

				System.Net.WebRequest req = System.Net.HttpWebRequest.Create(url);
				req.Method = "HEAD";
				using (System.Net.WebResponse resp = req.GetResponse())
				{
					var result = FileValidator.ValidateByContentType(resp.ContentType, resp.ContentLength);

					if (!result.IsValid)
					{
						return Json(new AjaxResponse(false, result.Message));
					}
				}

				using (var client = new WebClient())
				{
					path = Path.Combine(Path.GetTempPath(), string.Concat(GetUniqueKey(50), ".mp3"));
					client.DownloadFile(url, path);
				}

				return GenerateTags(path);
			}
			catch (Exception e)
			{
				return Json(new AjaxResponse(false, "Erro inesperado"));
			}
		}

		public JsonResult UploadSong()
		{
			try
			{
				var file = Request.Files[0];
				var result = FileValidator.ValidateByName(file.FileName, file.ContentLength);

				if (!result.IsValid)
				{
					return Json(new AjaxResponse(false, result.Message));
				}
				else
				{
					string filePath = Path.Combine(Path.GetTempPath(), string.Concat(GetUniqueKey(50), ".mp3"));
					file.SaveAs(filePath);
					return GenerateTags(filePath);
				}
			}
			catch (Exception e)
			{
				return Json(new AjaxResponse(false, "Erro inesperado"));
			}

		}

		public TagLib.File GetSongFile()
		{
			return TagLib.File.Create(Session["FileName"] as string);
		}

		public JsonResult GenerateTags(string filePath)
		{

			TagLib.File songFile = TagLib.File.Create(filePath);
			//Session.Add("Song", songFile);

			string coverX64 = null;
			if (songFile.Tag.Pictures.Length > 0)
			{
				byte[] coverBytes = ResizeImage((byte[])songFile.Tag.Pictures[0].Data.Data, 500, 500);
				coverX64 = Convert.ToBase64String(coverBytes);
			}

			AjaxResponse response = new AjaxResponse(true);
			response.Objects.Add(new SongModel
			{
				Title = songFile.Tag.Title,
				Album = songFile.Tag.Album,
				AlbumDiscLenght = songFile.Tag.DiscCount,
				AlbumTrackLenght = songFile.Tag.TrackCount,
				Artist = GetJoined(songFile.Tag.AlbumArtists),
				Composer = GetJoined(songFile.Tag.Composers),
				DiscNumber = songFile.Tag.Disc,
				Genre = GetJoined(songFile.Tag.Genres),
				TrackNumber = songFile.Tag.Track,
				Year = songFile.Tag.Year,
				AlbumArt = coverX64
			});

			Session.Add("FileName", filePath);
			return Json(response);

		}

		public void Save(SongModel song)
		{
			TagLib.File songFile = GetSongFile();
			songFile.Tag.Title = song.Title;
			songFile.Tag.Album = song.Album;
			songFile.Tag.DiscCount = song.AlbumDiscLenght;
			songFile.Tag.TrackCount = song.AlbumTrackLenght;
			songFile.Tag.AlbumArtists = SetJoined(song.Artist);
			songFile.Tag.Composers = SetJoined(song.Composer);
			songFile.Tag.Disc = song.DiscNumber;
			songFile.Tag.Genres = SetJoined(song.Genre);
			songFile.Tag.Track = song.TrackNumber;
			songFile.Tag.Year = song.Year;

			songFile.Save();
		}

		public void DownloadFile()
		{
			string fileName = Session["FileName"] as string;
			TagLib.File songFile = GetSongFile();
			string title = null;
			string artist = GetJoined(songFile.Tag.AlbumArtists);
			if (!string.IsNullOrEmpty(artist) && !string.IsNullOrEmpty(songFile.Tag.Title))
			{
				title = string.Concat(artist, " - ", songFile.Tag.Title, ".mp3");
			}
			else
			{
				title = Path.GetFileName(fileName);
			}

			Response.Clear();
			Response.ContentType = MimeMapping.GetMimeMapping(fileName);
			Response.AppendHeader("Content-Disposition", string.Format("attachment; filename={0}", title));
			Response.TransmitFile(fileName);
			Response.End();

			System.IO.File.Delete(fileName);
			//return File(Session["FileName"] as string, MimeMapping.GetMimeMapping(fileName));
		}

		public JsonResult ChangeAlbumArt()
		{
			var file = Request.Files[0];
			var result = FileValidator.ValidateByName(file.FileName, file.ContentLength);

			if (!result.IsValid)
			{
				return Json(new AjaxResponse(false, result.Message));
			}
			else
			{
				Picture picture = new Picture(ByteVector.FromStream(file.InputStream));
				TagLib.File songFile = GetSongFile();
				songFile.Tag.Pictures = new IPicture[1] { picture };
				songFile.Save();

				byte[] coverBytes = ResizeImage((byte[])songFile.Tag.Pictures[0].Data.Data, 500, 500);
				string coverX64 = Convert.ToBase64String(coverBytes);

				AjaxResponse response = new AjaxResponse(true);
				response.Objects.Add(coverX64);
				return Json(response);
			}
		}

		public JsonResult SelectSearchSong(SongQueryResult item)
		{
			SongModel model = new SongModel();
			model.Album = item.AlbumTitle;
			model.AlbumTrackLenght = Convert.ToUInt32(item.TrackCount);
			model.Artist = item.ArtistName;
			model.DiscNumber = Convert.ToUInt32(item.DiscNumber);
			model.Genre = item.Genre;
			model.Title = item.SongTitle;
			model.TrackNumber = Convert.ToUInt32(item.TrackNumber);
			model.Year = Convert.ToUInt32(item.AlbumYear);

			if (!string.IsNullOrEmpty(item.AlbumCoverUrl))
			{
				string  path = null;

				try
				{
					using (var client = new WebClient())
					{
						path = Path.Combine(Path.GetTempPath(), string.Concat(GetUniqueKey(50), ".jpg"));
						client.DownloadFile(item.AlbumCoverUrl, path);
					}

					Picture picture = new Picture(path);
					TagLib.File songFile = GetSongFile();
					songFile.Tag.Pictures = new IPicture[1] { picture };
					songFile.Save();
					byte[] coverBytes = ResizeImage((byte[])songFile.Tag.Pictures[0].Data.Data, 500, 500);
					string coverX64 = Convert.ToBase64String(coverBytes);
					model.AlbumArt = coverX64;
				}
				catch (Exception e)
				{

				}
				
			}

			//model.Composer
			//model.AlbumDiscLenght = item.
			//model.AlbumArt
			return Json(model);
		}
	}
}