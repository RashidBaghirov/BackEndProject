using BackEndProject.Entities;
using BackEndProject.Utilities.Comparer;
using Microsoft.EntityFrameworkCore;

namespace BackEndProject.Utilities.Extension
{
    public static class ExtensionMethods
    {
        public static List<Product> Related(IQueryable<Product> queryable, Product product, int id)
        {
            List<Product> relateds = new();

            product.ProductCategories.ForEach(pc =>
            {
                List<Product> relatedByCategory = queryable
                    .Include(p => p.ProductImages)
                    .Include(p => p.ProductCategories)
                        .ThenInclude(pc => pc.Category)
                    .Include(p => p.ProductTags)
                        .ThenInclude(pt => pt.Tag)
                    .Include(c => c.Collections)
                    .AsEnumerable()
                    .Where(
                        p => p.ProductCategories.Contains(pc, new ProductCategoryComparer())
                        && p.Id != id
                        && !relateds.Contains(p, new ProductComparer())
                    )
                    .ToList();
                relateds.AddRange(relatedByCategory);
            });

            product.ProductTags.ForEach(pt =>
            {
                List<Product> relatedByTag = queryable
                    .Include(p => p.ProductImages)
                    .Include(p => p.ProductCategories)
                        .ThenInclude(pc => pc.Category)
                    .Include(p => p.ProductTags)
                        .ThenInclude(pt => pt.Tag)
                    .Include(c => c.Collections)
                    .AsEnumerable()
                    .Where(
                        p => p.ProductTags.Contains(pt, new ProductTagComparer())
                        && p.Id != id
                        && !relateds.Contains(p, new ProductComparer())
                    )
                    .ToList();
                relateds.AddRange(relatedByTag);
            });

            return relateds;
        }


        public static async Task<string> CreateImage(this IFormFile file, string imagepath, string folder)
        {
            var destinationpath = Path.Combine(imagepath, folder);
            Random r = new();
            int random = r.Next(0, 1000);
            var filename = string.Concat(random, file.FileName);
            var path = Path.Combine(destinationpath, filename);
            using (FileStream stream = new(path, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return filename;
        }

        public static bool DeleteImage(string path)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
                return true;
            }
            return false;
        }
        public static bool IsValidLength(this IFormFile file, double size)
        {
            return (double)file.Length / 1024 / 1024 <= size;
        }

        public static bool IsValidFile(this IFormFile file, string type)
        {
            return file.ContentType.Contains(type);
        }
    }
}
