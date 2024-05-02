
public class Block
{
    public Block(string BlockJSON)
    {

    }

    public string Id { get; set;}
    public string PrevId { get; set;}
    public string[] Data { get; set;}
    public string nonce { get; set;}
    public long height { get; set;}
}