using System;
using System.Linq;
using System.Threading.Tasks;
using SQLite;
using Mine.Models;
using System.Collections.Generic;

namespace Mine.Services
{
    public class DatabaseService : IDataStore<ItemModel>
    {
        static readonly Lazy<SQLiteAsyncConnection> lazyInitializer = new Lazy<SQLiteAsyncConnection>(() =>
        {
            return new SQLiteAsyncConnection(Constants.DatabasePath, Constants.Flags);
        });

        static SQLiteAsyncConnection Database => lazyInitializer.Value;
        static bool initialized = false;

        public DatabaseService()
        {
            InitializeAsync().SafeFireAndForget(false);
        }

        async Task InitializeAsync()
        {
            if (!initialized)
            {
                if (!Database.TableMappings.Any(m => m.MappedType.Name == typeof(ItemModel).Name))
                {
                    await Database.CreateTablesAsync(CreateFlags.None, typeof(ItemModel)).ConfigureAwait(false);
                }
                initialized = true;
            }
        }

        /// <summary>
        /// Adds an Item to the database.
        /// </summary>
        /// <param name="item"></param>
        /// <returns>Bool whether or not the Item was added to the database.</returns>
        public async Task<bool> CreateAsync(ItemModel item)
        {
            // Fail if Item is undefined
            if (item == null)
            {
                return false;
            }

            // Try adding the Item to the database
            var result = await Database.InsertAsync(item);
            
            // Fail if it couldn't be added
            if (result == 0)
            {
                return false;
            }

            return true;
        }

        public Task<bool> UpdateAsync(ItemModel item)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task<ItemModel> ReadAsync(string id)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets all the Items in the database.
        /// </summary>
        /// <param name="forceRefresh"></param>
        /// <returns>A List of Items</returns>
        public async Task<IEnumerable<ItemModel>> IndexAsync(bool forceRefresh = false)
        {
            // Request all items and put them in a List
            var result = await Database.Table<ItemModel>().ToListAsync();
            return result;
        }
    }
}
