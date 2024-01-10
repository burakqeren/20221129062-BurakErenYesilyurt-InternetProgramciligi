using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using videoegitimfinal.Models;

namespace videoegitimfinal.ModelViews
{
    public class VideoViewModel
    {
        public int Id { get; set; }

        [Display(Name = "İçerik Başlığını Giriniz ")]
        [Required(ErrorMessage = "İçerik Başlığını Giriniz")]
        public string Name { get; set; }

        [Display(Name = "Video İçeriğini Giriniz ")]
        [Required(ErrorMessage = "Video İçeriğini Giriniz")]
        public string Description { get; set; }


        public string Kayıt { get; set; }

        [Display(Name = "Video Seçiniz ")]
        [Required(ErrorMessage = "Video Seçiniz")]
        public IFormFile Videolar {  get; set; }

        [Display(Name = "Video Kategori Seçiniz ")]
        [Required(ErrorMessage = "Video Kategori Seçiniz")]
        [ForeignKey(nameof(Egitim))]
        public int EgitimId { get; set; }



    }
}
