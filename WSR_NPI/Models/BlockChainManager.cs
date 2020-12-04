using IronPython.Hosting;
using Microsoft.Scripting.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WSR_NPI.DataBase;
using WSR_NPI.DataBase.Models;

namespace WSR_NPI.Models
{
    public static class BlockChainManager
    {
        public static List<Block> BlockChain { get; set; }

        public static string PubKey { get; set; }

        public static dynamic Sign { get; set; }    
        
        public static string Path { get; set; }

        public static List<Block> GetBlockChain()
        {
            return new Context().Blocks.ToList();
        }

        public static void GenerateNextBlock(string blockData, int indexUser)
        {
            ScriptEngine engine = Python.CreateEngine();
            ScriptScope scope = engine.CreateScope();
            scope.SetVariable("msg", blockData);
            scope.SetVariable("pubKey", PubKey);
            engine.ExecuteFile(Path, scope);
            dynamic result = scope.GetVariable("result");

            if (result)
            {
                var previousBlock = GetLatestBlock();
                var nextIndex = previousBlock.Index + 1;
                var nextTimestamp = DateTime.UtcNow.Ticks;
                var nextHash = CalculateHash(nextIndex, previousBlock.Hash, nextTimestamp, blockData);

                var db = new Context();

                db.Blocks.Add(new Block(nextIndex, previousBlock.Hash, nextTimestamp, blockData, nextHash, indexUser));
                db.SaveChanges();
            }
        }

        public static void ReplaceChain(List<Block> newBlocks)
        {
            if (IsValidChain(newBlocks) && newBlocks.Count() > BlockChain.Count())
            {
                Context db = new Context();
                BlockChain = newBlocks;
            }
        }

        private static bool IsValidChain(List<Block> newBlocks)
        {
            for (int i = 0; i < newBlocks.Count() - 1; i++)
            {
                if (!IsValidNewBlock(newBlocks[i + 1], newBlocks[i]))
                {
                    return false;
                }
            }

            return true;
        }

        public static bool IsValidNewBlock(Block newBlock, Block previousBlock)
        {
            if (previousBlock.Index + 1 != newBlock.Index)
            {
                return false;
            }
            else if (previousBlock.Hash != newBlock.PreviousHash)
            {
                return false;
            }
            else if (!CalculateHashForBlock(newBlock).Equals(newBlock.Hash))
            {
                return false;
            }
            return true;
        }

        public static string CalculateHashForBlock(Block block)
        {
            return CalculateHash(block.Index, block.PreviousHash, block.TimeStamp, block.Data);
        }

        public static Block GetLatestBlock()
        {
            var context = new Context();

            return context.Blocks.ToList().Last();
        }

        public static string CalculateHash(int index, string previousHash, long timestamp, string data)
        {
            return CreateMD5(index + previousHash + timestamp + data);
        }

        public static string CreateMD5(string input)
        {
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);


                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString();
            }
        }
    }
}