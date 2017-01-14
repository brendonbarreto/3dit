using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace _3DIT.Models
{
	public class FileValidator
	{
		private static List<FileValidatorType> Types
		{
			get
			{
				return new List<FileValidatorType>()
				{
					new FileValidatorType()
					{
						For = FileType.Audio,
						ContentTypes = new []{"audio/mpeg", "audio/mp3" },
						Extensions = new []{"mp3"},
						MaxSize = 20971520
					},
					new FileValidatorType()
					{
						For = FileType.Image,
						ContentTypes = new []{"image/png", "image/jpg", "image/jpeg" },
						Extensions = new []{"png", "jpg"},
						MaxSize = 5242880
					}
				};
			}
		}

		public static FileValidationResult Validate(ValidationSettings settings)
		{
			FileValidatorType validator = Types.SingleOrDefault(m => m.For == settings.AllowedType);
			if (settings.FileSize > validator.MaxSize)
			{
				return new FileValidationResult(false, "Tamanho excede o limite(" + validator.MaxSize / 1048576 + "MB)");
			}

			if (settings.ByExtension)
			{
				if (!validator.Extensions.Contains(Path.GetExtension(settings.FileName).Replace(".", "")))
				{
					return new FileValidationResult(false, "Tipo de arquivo inválido");
				}
			}
			else
			{
				if (!validator.ContentTypes.Contains(settings.ContentType))
				{
					return new FileValidationResult(false, "Tipo de arquivo inválido");
				}
			}

			return new FileValidationResult(true);
		}
	}

	public class FileValidatorType
	{
		public long MaxSize { get; set; }

		public string[] Extensions { get; set; }

		public string[] ContentTypes { get; set; }

		public FileType For { get; set; }

	}

	public class ValidationSettings
	{
		public long FileSize { get; set; }

		public FileType AllowedType { get; set; }

		public bool ByExtension { get; set; }

		public string FileName { get; set; }

		public string ContentType { get; set; }
	}

	public class FileValidationResult
	{
		public bool IsValid { get; set; }

		public string Message { get; set; }

		public FileValidationResult()
		{

		}

		public FileValidationResult(bool isValid)
		{
			IsValid = isValid;
		}

		public FileValidationResult(bool isValid, string message)
		{
			IsValid = isValid;
			Message = message;
		}
	}

	public enum FileType
	{
		Audio, Image
	}
}