using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CosmOS_Projekt
{
    internal class Memory
    {
        Cosmos.Core.ManagedMemoryBlock newBlock = new Cosmos.Core.ManagedMemoryBlock(16);

        public void writeAt(uint index, byte value)
        {
            newBlock.Write8(index, value);
        }

        public ushort readAt(uint index) 
        {
            ushort rVal = 0;
            rVal = newBlock.Read16(index);

            return rVal;
        }
    }
}
