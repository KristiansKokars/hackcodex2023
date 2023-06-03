public class DocumentService
{
    private DigitexDb _db;

    public DocumentService(DigitexDb db)
    {
        _db = db;
    }

    public List<Document> GetDocuments()
    {
        return _db.Documents.ToList();
    }

    public List<Document> GetFaultyDocuments()
    {
        return _db.Documents.Where(d => d.Status == "Faulty").ToList();
    }

    public Document GetDocumentById(Guid id)
    {
        return _db.Documents.Where(d => d.Id == id).FirstOrDefault();
    }

    public Guid SaveDocument(string content, Guid userId, string link, string status)
    {
        var document = new Document 
        {
            Content = content,
            UserId = userId,
            Link = link,
            Status = status
        };
        _db.Documents.Add(document);
        _db.SaveChanges();
        // TODO: check if ID is returned
        return document.Id;
    }

    public Guid MarkDocumentSolved(Guid documentId, string updatedContent)
    {
        var documentToUpdate = _db.Documents.Where(d => d.Id == documentId).FirstOrDefault();
        documentToUpdate.Content = updatedContent;
        documentToUpdate.Status = "Correct";
        _db.Documents.Update(documentToUpdate);
        _db.SaveChanges();
        // TODO: check if ID is returned
        return documentToUpdate.Id;
    }
}