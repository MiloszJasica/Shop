using Projekt.Data;

namespace Projekt.Services
{
    public class CategoryService
    {
        private readonly ApplicationDbContext _context;

        public CategoryService(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<string> GetCategories()
        {
            return new List<string>
            {
                "owoce",
                "warzywa",
                "cukierki",
                "mięso",
                "nabiał",
                "przekąski"
            };
        }
    }
}
