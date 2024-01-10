namespace videoegitimfinal.Models
{
    public class Egitim
    {
        public int Id { get; set; }

        public string kategoriAdı { get; set; }

        public ICollection<Video> Videos { get; set; }
    }
}
