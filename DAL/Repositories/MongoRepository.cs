using DAL.Entities;
using DAL.Extensions;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories;

public class MongoRepository<TDocument> : IMongoRepository<TDocument> where TDocument : IDocument
{
    private IMongoCollection<TDocument> _collection = null!;
    private readonly IMongoDatabase _database;

    public MongoRepository(IOptions<MongoDbConfig> mongoOptions)
    {

        var client = new MongoClient(mongoOptions.Value.ConnectionString);

        _database = client.GetDatabase(mongoOptions.Value.DatabaseName);
    }

    public MongoRepository(IOptions<MongoDbConfig> mongoOptions, string collectionName)
    {
        var client = new MongoClient(mongoOptions.Value.ConnectionString);

        _database = client.GetDatabase(mongoOptions.Value.DatabaseName);

        _collection = _database.GetCollection<TDocument>(collectionName);
    }

    public void DropCollection(string collectionName) => _database.DropCollection(collectionName);

    public IMongoRepository<TDocument> UseCollection(string collectionName)
    {
        _collection = _database.GetCollection<TDocument>(collectionName);
        return this;
    }

    public TDocument FindById(string id)
    {
        var filter = Builders<TDocument>.Filter.Eq(doc => doc.Id, id);
        return _collection.Find(filter).SingleOrDefault();
    }

    public TDocument DeleteById(string id)
    {
        var filter = Builders<TDocument>.Filter.Eq(doc => doc.Id, id);
        return _collection.FindOneAndDelete(filter);
    }

    public DeleteResult DeleteMany(Expression<Func<TDocument, bool>> filter = null!)
    {
        return _collection.DeleteMany(filter);
    }

    public TDocument DeleteOne(Expression<Func<TDocument, bool>> filter)
    {
        return _collection.FindOneAndDelete(filter);
    }

    public IEnumerable<TDocument> FilterBy(Expression<Func<TDocument, bool>> filter = null!)
    {
        IQueryable<TDocument> query = _collection.AsQueryable();

        if (filter != null)
        {
            query = query.Where(filter);
        }

        return query.ToList();
    }

    public TDocument FindOne(Expression<Func<TDocument, bool>> filter)
    {
        return _collection.Find(filter).FirstOrDefault();
    }

    public void InsertMany(ICollection<TDocument> documents)
    {
        _collection.InsertMany(documents);
    }

    public void InsertOne(TDocument document)
    {
        _collection.InsertOne(document);
    }

    public TDocument ReplaceOne(TDocument document)
    {
        var filter = Builders<TDocument>.Filter.Eq(doc => doc.Id, document.Id);
        return _collection.FindOneAndReplace(
            filter, 
            document,
            new FindOneAndReplaceOptions<TDocument, TDocument>
            {
                 ReturnDocument = ReturnDocument.After
            });
    }


    public void SetFieldAsUnique(string fieldName)
    {
        var options = new CreateIndexOptions() { Unique = true };
        var field = new StringFieldDefinition<TDocument>(fieldName);
        var indexDefinition = new IndexKeysDefinitionBuilder<TDocument>().Ascending(field);
        _collection.Indexes.CreateOne(indexDefinition, options);
    }
}
