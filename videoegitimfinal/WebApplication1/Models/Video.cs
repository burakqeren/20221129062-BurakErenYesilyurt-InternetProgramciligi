using System.ComponentModel.DataAnnotations.Schema;

namespace videoegitimfinal.Models
{
    public class Video
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Kayıt { get; set; }

        [ForeignKey(nameof(Egitim))]
        public int EgitimId { get; set; }
        
        public Egitim Egitimi { get; set; }  


    }
}
