﻿using System;
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

        /// <summary>
        /// Update an Item in the database.
        /// </summary>
        /// <param name="item">The item to update</param>
        /// <returns>Whether or not the Item was updated</returns>
        public async Task<bool> UpdateAsync(ItemModel item)
        {
            // Fail if item is undefined
            if (item == null)
            {
                return false;
            }

            // Try updating the item
            var result = await Database.UpdateAsync(item);

            // Fail if it couldn't be updated
            if (result == 0)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Delete an Item from the database.
        /// </summary>
        /// <param name="id">ID of the Item to delete</param>
        /// <returns>Whether or not the Item was deleted</returns>
        public async Task<bool> DeleteAsync(string id)
        {
            // Try getting the item from the database
            var data = await ReadAsync(id);

            // Fail if it couldn't be read
            if (data == null)
            {
                return false;
            }

            // Try deleting the item from the database
            var result = await Database.DeleteAsync(data);
            
            // Fail if it couldn't be deleted
            if (result == 0)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Reads an Item from the database.
        /// </summary>
        /// <param name="id">The ID of the Item</param>
        /// <returns>The specified Item</returns>
        public Task<ItemModel> ReadAsync(string id)
        {
            // Fail if the ID is undefined
            if (id == null)
            {
                return null;
            }

            // Call the database to read the ID
            // Using Linq syntax, find the first record that has the ID that matches
            var result = Database.Table<ItemModel>().FirstOrDefaultAsync(m => m.Id.Equals(id));

            return result;
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
