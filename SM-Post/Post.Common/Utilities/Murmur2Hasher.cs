using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Post.Common.Utilities
{
    public static class Murmur2Hasher
    {
        public static int Hash(string key)
        {
            byte[] data = Encoding.UTF8.GetBytes(key);
            const uint seed = 0x9747b28c;
            const uint m = 0x5bd1e995;
            const int r = 24;

            int length = data.Length;
            uint h = seed ^ (uint)length;
            int currentIndex = 0;

            while (length >= 4)
            {
                uint k = BitConverter.ToUInt32(data, currentIndex);

                k *= m;
                k ^= k >> r;
                k *= m;

                h *= m;
                h ^= k;

                currentIndex += 4;
                length -= 4;
            }

            switch (length)
            {
                case 3:
                    h ^= (uint)(data[currentIndex + 2] << 16);
                    goto case 2;
                case 2:
                    h ^= (uint)(data[currentIndex + 1] << 8);
                    goto case 1;
                case 1:
                    h ^= data[currentIndex];
                    h *= m;
                    break;
            }

            h ^= h >> 13;
            h *= m;
            h ^= h >> 15;

            return (int)(h & 0x7FFFFFFF); // Ensure non-negative
        }
    }
}
