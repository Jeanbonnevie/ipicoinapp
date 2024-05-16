using System;
using System.Collections.Concurrent;
using Newtonsoft.Json;
using System.Linq;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

[Serializable]
public class Block
{
    public static Block CreateBlock(string BlockJSON)
    {
        Block block = JsonConvert.DeserializeObject<Block>(BlockJSON);

        if (block.id.Length != 64 && block.id.Length != 0) throw new Exception("Id error " + block.id.Length);
        if (block.prevId.Length != 64) throw new Exception("previd error");
        if (block.nonce.Length != 64) throw new Exception("nonce error");
        if (block.height < 0 || block.height >= long.MaxValue) throw new Exception("height error");
        if (block.timestamp > DateTimeOffset.UtcNow.ToUnixTimeSeconds()) throw new Exception("timestamp error");

        return block;
    }

    private static Random random = new Random();

    public void GenNonce()
    {
        using (SHA256 sha256Hash = SHA256.Create())
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            byte[] bytes = sha256Hash.ComputeHash(
                Encoding.UTF8.GetBytes(new string(Enumerable.Repeat(chars, 10)
                    .Select(s => s[random.Next(s.Length)]).ToArray()))
                );

            StringBuilder builder = new StringBuilder();

            for (int i = 0; i < bytes.Length; i++)
            {
                builder.Append(bytes[i].ToString("x2"));
            }

            nonce = builder.ToString();
        }
    }

    public void AddTransaction(Transaction transaction)
    {
        int initialSize = data != null ? data.Length : 0;
        List<Transaction> ts = data != null ? data.ToList() : new List<Transaction>();
        ts.Add(transaction);
        data = ts.ToArray();

        if (data.Length > initialSize)
            timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
    }

    public string GetId()
    {
        var sz = JsonConvert.SerializeObject(this);

        using (SHA256 sha256Hash = SHA256.Create())
        {
            byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(sz));
            StringBuilder builder = new StringBuilder();

            for (int i = 0; i < bytes.Length; i++)
            {
                builder.Append(bytes[i].ToString("x2"));
            }

            int zeroCondition = 0;
            builder.ToString().ToList().ForEach(c =>
            {
                if (c == 0) zeroCondition++;
            });
            
            return builder.ToString();
        }
    }

    internal bool CanBeUsed()
    {
        if (data == null) return false;
        return data.Length >= 1;
    }

    public string id = "";
    public string prevId = "";
    public Transaction[] data;
    public string nonce = "";
    public long height;
    public long timestamp;
}

public static class BlockEntensions
{
    public static Block GetBestBlock(this Block block_1, Block block_2)
    {
        if (block_1 == null) return block_2;
        if (block_2 == null) return block_1;

        int zero_1 = 0;
        int zero_2 = 0;
        block_1.id.ToList().ForEach(c => { if (c == '0') zero_1++; });
        block_2.id.ToList().ForEach(c => { if (c == '0') zero_2++; });

        if(zero_1 > zero_2) return block_1;

        if (block_1.data.Length > block_2.data.Length) return block_1;
        if (block_2.data.Length > block_1.data.Length) return block_2;

        return block_1;
    }
}