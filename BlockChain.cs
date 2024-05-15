using System;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;
using System.Linq;

namespace ipiblockChain
{
    public class BlockChain
    {
        private string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "ipiblockchain", "blockchain.json");
        private List<Block> blocks;
        int difficulty = 2;

        public BlockChain()
        {
            // Ensure directory path exists
            string directoryPath = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            if (File.Exists(filePath))
            {
                // Read blockchain data from the file
                try
                {
                    using (StreamReader file = File.OpenText(filePath))
                    {
                        JsonSerializer serializer = new JsonSerializer();
                        this.blocks = (List<Block>)serializer.Deserialize(file, typeof(List<Block>));
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error while reading blockchain data: " + ex.Message);
                    // Handle the exception as needed
                }
            }
            else
            {
                // If the file doesn't exist, initialize a new blockchain with the first zero block
                this.blocks = new List<Block>();

                Transaction firstTransaction = Transaction.CreateNewTransaction("void", "0000", 10000, DateTimeOffset.UtcNow.ToUnixTimeSeconds(), "0000");

                Block genesisBlock = new Block()
                {
                    
                    height = 0,
                    timestamp = DateTime.Now,
                };

                genesisBlock.AddTransaction(new List<Transaction>() { firstTransaction});
                genesisBlock.GenNonce();
                this.blocks.Add(genesisBlock);
                SaveBlockchain();
            }
        }

        public void InitBlock(Block block)
        {
            block.height = blocks.Count;
            block.prevId = blocks[(int)block.height - 1].id;
        }


        public bool TryAddBlockToBlockChain(Block block)
        {
            var sz = JsonConvert.SerializeObject(block);

            try
            {
                Block desBlock = Block.CreateBlock(sz);
            }
            catch(Exception ex)
            {
                return false;
            }

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

                return zeroCondition >= 2;
            }
        }

        public void SaveBlockchain()
        {
            // Save blockchain data to the file
            try
            {
                using (StreamWriter file = File.CreateText(filePath))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    serializer.Serialize(file, this.blocks);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error while saving blockchain data: " + ex.Message);
                // Handle the exception as needed
            }
        }
    }
}