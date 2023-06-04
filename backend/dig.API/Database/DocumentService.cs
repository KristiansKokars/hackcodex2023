using Microsoft.EntityFrameworkCore.Storage;

public class DocumentService
{
    private DigitexDb _db;

    // TODO: add data retrieval based on user ID
    public DocumentService(DigitexDb db)
    {
        _db = db;
    }

    [Obsolete("! ALL document retrieval (test purposes) !")]
    public List<Document> GetDocuments()
    {
        return _db.Documents.ToList();
    }

    public List<Document> GetDocuments(Guid userId)
    {
        return _db.Documents.Where(d => d.UserId == userId).ToList();
    }

    [Obsolete("! ALL faulty document retrieval (test purposes) !")]
    public List<Document> GetFaultyDocuments()
    {
        return _db.Documents.Where(d => d.Status == "Faulty").ToList();
    }

    public List<Document> GetFaultyDocuments(Guid userId)
    {
        return _db.Documents
            .Where(d => d.UserId == userId)
            .Where(d => d.Status == "Faulty")
            .ToList();
    }

    [Obsolete("! UNSECURE document retrieval (test purposes) !")]
    public Document GetDocumentById(Guid id)
    {
        return _db.Documents.Where(d => d.Id == id).FirstOrDefault();
    }

    public Document GetDocumentById(Guid userId, Guid documentId)
    {
        return _db.Documents
            .Where(d => d.UserId == userId)
            .Where(d => d.Id == documentId)
            .FirstOrDefault();
    }

    public Guid SaveDocument(string content, Guid userId, string link, string status)
    {
        var document = DocumentHelper(content, userId, link, status);
        _db.Documents.Add(document);
        _db.SaveChanges();
        // TODO: make sure ID is returned
        return document.Id;
    }

    public List<Guid> SaveDocumentBatch(List<Document> docs)
    {
        _db.AddRange(docs);
        _db.SaveChanges();
        // TODO: make sure IDs are returned
        var ids = docs.Select(d => d.Id).ToList();
        return ids;
    }

    public Document DocumentHelper(string content, Guid userId, string link, string status)
    {
        var document = new Document 
        {
            Content = content,
            UserId = userId,
            Link = link,
            Status = status
        };
        return document;
    }

    public Guid MarkDocumentSolved(Guid documentId, string updatedContent)
    {
        var documentToUpdate = _db.Documents.Where(d => d.Id == documentId).FirstOrDefault();
        documentToUpdate.Content = updatedContent;
        documentToUpdate.Status = "Correct";
        _db.Documents.Update(documentToUpdate);
        _db.SaveChanges();
        // TODO: make sure ID is returned
        return documentToUpdate.Id;
    }
}