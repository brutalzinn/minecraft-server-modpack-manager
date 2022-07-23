﻿using Microsoft.Extensions.Options;
using MinecraftServer.Api.Models;
using MongoDB.Driver;

namespace MinecraftServer.Api.Services
{
    public class MongoDBService
    {
        private readonly IMongoCollection<ModPackModel> _mongoDBConnection;

        public MongoDBService(IOptions<MongoDatabaseSettings> bookStoreDatabaseSettings)
        {
            var mongoClient = new MongoClient(
                bookStoreDatabaseSettings.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(
                bookStoreDatabaseSettings.Value.DatabaseName);

            _mongoDBConnection = mongoDatabase.GetCollection<ModPackModel>(
                bookStoreDatabaseSettings.Value.CollectionName);
        }

        public async Task<List<ModPackModel>> GetAsync() =>
            await _mongoDBConnection.Find(_ => true).ToListAsync();

        public async Task<ModPackModel?> GetAsync(string id) =>
            await _mongoDBConnection.Find(x => x.Id.Equals(id)).FirstOrDefaultAsync();

        public async Task CreateAsync(ModPackModel newBook) =>
            await _mongoDBConnection.InsertOneAsync(newBook);

        public async Task UpdateAsync(string id, ModPackModel updatedBook) =>
            await _mongoDBConnection.ReplaceOneAsync(x => x.Id.Equals(id), updatedBook);

        public async Task UpdateKeyPairAsync(string id, Dictionary<string, object> dictionary) 
        {

            // var updateDefinition = Builders<ModPackModel>.Update;
            UpdateDefinition<ModPackModel> updateDefinition = null!;

            foreach (var item in dictionary)
            {
                updateDefinition.Set(item.Key, item.Value);
            }

            var filter = Builders<ModPackModel>.Filter.Eq(x => x.Id, id);

            FindOneAndUpdateOptions<ModPackModel> updateOptions = new FindOneAndUpdateOptions<ModPackModel>
            {
                IsUpsert = false,
                ReturnDocument = ReturnDocument.After,
            };

            await _mongoDBConnection.FindOneAndUpdateAsync(filter, updateDefinition, updateOptions);
        }
        
     

        public async Task RemoveAsync(string id) =>
            await _mongoDBConnection.DeleteOneAsync(x => x.Id.Equals(id));
    }
}
