using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace LibraryManagementSystem.Models
{
    public class BookCategory
    {
        [Key]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "CategoryName is Required")]
        [StringLength(100)]
        public string CategoryName { get; set; }

        [StringLength(100)]
        public string Description { get; set; }

        [JsonIgnore]
        public virtual ICollection<Book> Books { get; set; } = new List<Book>();
    }

}
