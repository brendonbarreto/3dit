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
						ContentTypes = new []{"image/png", "image/jpg" },
						Extensions = new []{"png", "jpg"},
						MaxSize = 5242880
					}
				};
			}
		}



		public static FileValidationResult ValidateByName(string fileName, long size)
		{
			string ext = Path.GetExtension(fileName).Replace(".", "");
			FileValidatorType validator = Types.SingleOrDefault(m => m.Extensions.Contains(ext));
			return Validate(validator, size);
		}

		public static FileValidationResult ValidateByContentType(string contentType, long size)
		{
			FileValidatorType validator = Types.SingleOrDefault(m => m.ContentTypes.Contains(contentType));
			return Validate(validator, size);
		}

		private static FileValidationResult Validate(FileValidatorType validator, long size)
		{
			if (validator != null)
			{
				if (size > validator.MaxSize)
				{
					return new FileValidationResult(false, "Tamanho excede o limite(" + validator.MaxSize / 1048576 + "MB)");
				}

				return new FileValidationResult(true);
			}
			else
			{
				return new FileValidationResult(false, "Formato de arquivo inválido");
			}
		}
	}

	public class FileValidatorType
	{
		public long MaxSize { get; set; }

		public string[] Extensions { get; set; }

		public string[] ContentTypes { get; set; }

		public FileType For { get; set; }

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