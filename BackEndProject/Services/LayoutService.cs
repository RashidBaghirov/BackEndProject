using BackEndProject.DAL;

namespace BackEndProject.Services
{
    public class LayoutService
    {
        private readonly ProductDbContext _context;

        public LayoutService(ProductDbContext context)
        {
            _context = context;
        }

        public Dictionary<string, string> GetSettings()
        {
            Dictionary<string, string> settings = _context.Settings.ToDictionary(s => s.Key, s => s.Value);

            return settings;
        }
    }
}
