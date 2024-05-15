using System;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;

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
                SaveBlockchain();
            }
        }

        public int GetCurrentBlockHeight()
        {
            return blocks.Count;
        }


        public bool TryAddBlockToBlockChain(Block block)
        {
            //

            return false;
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