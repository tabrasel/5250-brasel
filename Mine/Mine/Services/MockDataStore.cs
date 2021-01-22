using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mine.Models;

namespace Mine.Services
{
    public class MockDataStore : IDataStore<ItemModel>
    {
        readonly List<ItemModel> items;

        public MockDataStore()
        {
            items = new List<ItemModel>()
            {
                new ItemModel { Id = Guid.NewGuid().ToString(), Text = "Parmigiano Reggiano", Description="An Italian hard cheese produced from cow's milk.", Value=10 },
                new ItemModel { Id = Guid.NewGuid().ToString(), Text = "Taleggio", Description="An Italian semisoft, washed-rind cheese produced from cow's milk.", Value=5 },
                new ItemModel { Id = Guid.NewGuid().ToString(), Text = "Roquefort", Description="A French blue cheese produced from sheep's milk.", Value=8 },
                new ItemModel { Id = Guid.NewGuid().ToString(), Text = "Mahon", Description="A Spanish soft white cheese produced from cow's milk.", Value=3 },
                new ItemModel { Id = Guid.NewGuid().ToString(), Text = "Camembert", Description="A French soft bloomy-rind cheese produced from cow's milk.", Value=7 }
            };
        }

        public async Task<bool> AddItemAsync(ItemModel item)
        {
            items.Add(item);

            return await Task.FromResult(true);
        }

        public async Task<bool> UpdateItemAsync(ItemModel item)
        {
            var oldItem = items.Where((ItemModel arg) => arg.Id == item.Id).FirstOrDefault();
            items.Remove(oldItem);
            items.Add(item);

            return await Task.FromResult(true);
        }

        public async Task<bool> DeleteItemAsync(string id)
        {
            var oldItem = items.Where((ItemModel arg) => arg.Id == id).FirstOrDefault();
            items.Remove(oldItem);

            return await Task.FromResult(true);
        }

        public async Task<ItemModel> GetItemAsync(string id)
        {
            return await Task.FromResult(items.FirstOrDefault(s => s.Id == id));
        }

        public async Task<IEnumerable<ItemModel>> GetItemsAsync(bool forceRefresh = false)
        {
            return await Task.FromResult(items);
        }
    }
}