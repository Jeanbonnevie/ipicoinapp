﻿using System;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace ipiblockChain
{
    internal class BlockChain
    {
        private string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "ipiblockchain", "blockchain.json");
        private List<Block> blocks;

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
                this.blocks.Add(new Block()
                {
                    
                    timestamp = DateTime.Now
                });
                SaveBlockchain();
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