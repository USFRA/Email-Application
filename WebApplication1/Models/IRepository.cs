using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApplication1.Models
{
    public interface IRepository<TEntity> : IDisposable where TEntity : IEntity
    {
        IQueryable<TEntity> GetAll();
        TEntity Get(int id);
        void Delete(TEntity entity);
        void DeleteAll(IEnumerable<TEntity> entities);
        void Add(TEntity entity);
        void Update(TEntity entity);
        void Save();
    }

    public interface IEntity
    {
        int Id { get; set; }
    }

    internal class NodeHelper
    {
        public static string GetNameFromPath(string path)
        {
            var folders = path.Split(new char[] { '/' });
            return folders[folders.Length - 1];
        }
        public static int GetNodeIdFromPath(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return 1;
            }

            var folders = path.Split(new char[] { '/' });
            if (folders.Length == 0 || string.IsNullOrEmpty(folders[0]))
            {
                return 1;
            }
            try
            {
                var nodeId = Int32.Parse(folders[0]);
                return nodeId;
            }
            catch (Exception)
            {
                return 1;
            }
        }
    }
}
