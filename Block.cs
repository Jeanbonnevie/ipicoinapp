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

        if (block.id.Length != 256) throw new Exception("Id error " + block.id.Length);
        if (block.prevId.Length != 256) throw new Exception("previd error");
        if (block.nonce.Length != 256) throw new Exception("nonce error");
        if (block.height < 0 || block.height >= long.MaxValue) throw new Exception("height error");
        if (block.timestamp > DateTime.Now) throw new Exception("timestamp error");

        return block;
    }

    public void GenNonce()
    {
        using (SHA256 sha256Hash = SHA256.Create())
        {
            byte[] seedBytes = Encoding.UTF8.GetBytes("Gael");

            // Concatenate seed with random data
            byte[] randomBytes = new byte[16];
            using (RandomNumberGenerator rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(randomBytes);
            }
            byte[] combinedBytes = new byte[seedBytes.Length + randomBytes.Length];
            Buffer.BlockCopy(seedBytes, 0, combinedBytes, 0, seedBytes.Length);
            Buffer.BlockCopy(randomBytes, 0, combinedBytes, seedBytes.Length, randomBytes.Length);

            Console.WriteLine(sha256Hash.ComputeHash(combinedBytes).Length);
            nonce = sha256Hash.ComputeHash(combinedBytes).ToString();
        }
    }

    public void AddTransaction(List<Transaction> transactions)
    {
        if (data == null) data = transactions.ToArray();

        int initialSize = data.Length;
        List<Transaction> list = data.ToList();
        foreach (var t in transactions)
        {
            if (!list.Contains(t))
            {
                list.Add(t);
            }
        }

        if (list.Count > initialSize)
            timestamp = DateTime.Now;
    }

    internal bool CanBeUsed()
    {
        if (data == null) return false;
        return data.Length >= 1;
    }

    public string id;
    public string prevId;
    public Transaction[] data;
    public string nonce;
    public long height;
    public DateTime timestamp;
}

public static class BlockEntensions
{
    public static Block GetBiggestBlock(this Block block_1, Block block_2)
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