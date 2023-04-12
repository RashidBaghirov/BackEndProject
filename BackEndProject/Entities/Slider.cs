using System.ComponentModel.DataAnnotations.Schema;

namespace BackEndProject.Entities
{
	public class Slider : BaseEntity
	{
		public string? ImagePath { get; set; }
		public string? Title { get; set; }
		public string? Buttontitle { get; set; }
		public byte? Order { get; set; }
		public bool? IsVideo { get; set; }

		[NotMapped]

		public IFormFile? Image { get; set; }
	}
}
