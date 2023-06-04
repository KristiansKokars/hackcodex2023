public class DocLabelsDto
{
    // public Uri Schema { get; set; }
    // public string Document { get; set; }
    public List<DocLabel> Labels { get; set; }
}

public class DocLabel
{
    public string Field { get; set; }
    public List<FieldValue> Value { get; set; }
}

public class FieldValue
{
    // public string Text { get; set; }
    public List<List<double>> BoundingBoxes { get; set; }
}