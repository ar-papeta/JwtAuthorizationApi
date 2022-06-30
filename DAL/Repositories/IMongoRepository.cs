using DAL.Entities;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories;

public interface IMongoRepository<TDocument> where TDocument : IDocument
{
    IMongoRepository<TDocument> UseCollection(string collectionName);
    IEnumerable<TDocument> FilterBy(Expression<Func<TDocument, bool>> filter = null!);
    TDocument FindOne(Expression<Func<TDocument, bool>> filter);
    void InsertOne(TDocument document);
    void InsertMany(ICollection<TDocument> documents);
    TDocument ReplaceOne(TDocument document);
    TDocument DeleteOne(Expression<Func<TDocument, bool>> filter);
    TDocument DeleteById(string id);
    DeleteResult DeleteMany(Expression<Func<TDocument, bool>> filter = null!);
    TDocument FindById(string id);
    public void DropCollection(string collectionName);
    public void SetFieldAsUnique(string fieldName);
}
