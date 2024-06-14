using System;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;
using System.Linq;
using System.Security.Principal;
using System.Collections.Concurrent;

namespace ipiblockChain
{
    public class BlockChain
    {
        private string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "ipiblockchain", "blockchain.json");
        private List<Block> blocks;
        int difficulty = 2; // 2 c'est trop simple, 3 c'est mieux, ça évite de spamme des micro block [5 c'est trop]

        public event Action<Block> OnExternalBlockModifiesBlockChain;
        public event Action<List<Transaction>> OnTransactionsReceived;

        private List<Transaction> transactions = new List<Transaction>();

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

                Transaction firstTransaction = Transaction.CreateNewTransaction("void", "3436C778A660A2A06F73F9AB7BF090BF40CE3F79", 10000, DateTimeOffset.UtcNow.ToUnixTimeSeconds());

                Block genesisBlock = new Block()
                {           
                    height = 0,
                    timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                };

                genesisBlock.AddTransaction(firstTransaction);
                genesisBlock.nonce = "";
                this.blocks.Add(genesisBlock);
                genesisBlock.id = genesisBlock.GetId();
                SaveBlockchain();

                Console.WriteLine("Block Genesis added");
            }
        }

        public void InitBlock(ref Block block)
        {
            block.height = blocks.Count;
            block.prevId = blocks[(int)block.height - 1].id;
            Console.WriteLine($"Block height :: {block.height}");
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
                string hash = builder.ToString();

                foreach(var c in hash)
                {
                    if (int.TryParse(c.ToString(), out int r))
                    {
                        if (r == 0)
                        {
                            zeroCondition++;
                        }
                        else
                        {
                            break;
                        }
                    }
                    else
                        break;
                }

                if(zeroCondition >= difficulty)
                {
                    Console.WriteLine(builder.ToString());
                    block.id = builder.ToString();
                    AddToBlockChain(block);
                    lock (transactionLock)
                    {
                        block.data.ToList().ForEach(t =>
                        {
                            transactions.Remove(t);
                        });

                        OnTransactionsReceived(transactions);
                    }
                    Console.WriteLine("Block added n°" + block.height);
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        private void AddToBlockChain(Block block)
        {
            blocks.Add(block);
            SaveBlockchain();

            
        }

        private static readonly Object obj = new Object();
        public void SaveBlockchain()
        {
            lock (obj)
            {
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
                }
            }
        }

        internal void CheckBlock(Block block)
        {
            Block ourBlock = blocks.Find(x => x.height == block.height);
            if(ourBlock == null)
            {
                blocks.Add(block);
                OnExternalBlockModifiesBlockChain?.Invoke(block);
            }
            else
            {
                Block bestBlock = block.GetBestBlock(ourBlock);
                if (bestBlock != ourBlock)
                {
                    OnExternalBlockModifiesBlockChain?.Invoke(bestBlock);
                    if (blocks.Count - 1 > bestBlock.height)
                    {
                        blocks.RemoveRange((int)bestBlock.height, blocks.Count - 1);
                        blocks.Add(bestBlock);
                    }
                    else
                    {
                        blocks.Add(bestBlock);
                    }
                }
            }
        }

        private static readonly Object transactionLock = new Object();
        internal void ReceiveTransaction(Transaction transaction)
        {
            if (!CheckTransaction(transaction)) return;

            lock (transactionLock)
            {
                transactions.Add(transaction);
                OnTransactionsReceived?.Invoke(transactions);
                Console.WriteLine("NEW transaction received ::: " + transactions.Count);
            }
        }

        public bool CheckTransaction(Transaction transaction)
        {
            if (transactions.Find(t => t.timestamp == transaction.timestamp) != null) return false;

            //Check if the sender has enought money to send this amount of money
            return true;
        }

        public float GetBalance(string walletId)
        {
            return 10000.0f;
        }
    }
}