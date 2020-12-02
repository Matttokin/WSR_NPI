using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WSR_NPI.DataBase.Models
{
    public class Block
    {
        public int Id { get; set; }

        public int Index { get; set; }

        public int IndexUser { get; set; }

        public string PreviousHash { get; set; }

        public long TimeStamp { get; set; }

        public string Data { get; set; }

        public string Hash { get; set; }

        public Block() { }

        public Block(int index, string previousHash, long timeStamp, string data, string hash, int indexUser)
        {
            Index = index;
            PreviousHash = previousHash;
            TimeStamp = timeStamp;
            Data = data;
            Hash = hash;
            IndexUser = indexUser;
        }
    }
}