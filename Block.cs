using System;
using Newtonsoft.Json;

public class Block
{
    public static Block CreateBlock(string BlockJSON)
    {
        Block block = JsonConvert.DeserializeObject<Block>(BlockJSON);

        if (block.id.Length != 256) throw new Exception("Id error " + block.id.Length);
        if (block.prevId.Length != 256) throw new Exception("previd error");
        if (block.nonce.Length != 256) throw new Exception("nonce error");
        if (block.height < 0 || block.height >= long.MaxValue) throw new Exception("height error");
        if (block.timestamp > DateTime.Now) throw new Exception("timestamp error");

        return block;
    }

    public string id;
    public string prevId;
    public string[] data;
    public string nonce;
    public long height;
    public DateTime timestamp;
}