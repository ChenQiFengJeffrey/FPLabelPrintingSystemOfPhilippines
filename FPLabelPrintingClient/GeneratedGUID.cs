using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPLabelPrintingClient 
{
    public class GeneratedGUID
    {
        public static String RandomNum()
        {
            long tick = DateTime.Now.Ticks;
            Random rad = new Random((int)(tick & 0xffffffffL) | (int)(tick >> 32));
            int a = rad.Next(0, 9);
            Random random = new Random(GetRandomSeedbyGuid());
            int b = rad.Next(0, 9);
            Random rand = new Random(GetRandomSeedbyGuid());
            int c = rand.Next(0, 9);
            DateTime now = DateTime.Now;
            return now.ToString("yyMMdd") + c + GetRandomByGuid(random, 3) + a + b + GetRandomByGuid(random, 4);
        }

        private static String GetRandomByGuid(Random rand, int len)
        {
            var result = new StringBuilder();
            for (int i = 0; i < len; i++)
            {
                result.Append(rand.Next(0, 10));
            }
            return result.ToString();
        }

        /// <summary>
        /// 使用Guid生成种子
        /// </summary>
        /// <returns></returns>
        private static int GetRandomSeedbyGuid()
        {
            return Guid.NewGuid().GetHashCode();
        }

        /// <summary>
        /// 由连字符分隔的32位数字
        /// </summary>
        /// <returns></returns>
        public static string GetGuid()
        {
            System.Guid guid = new Guid();
            guid = Guid.NewGuid();
            return guid.ToString();
        }

        /// <summary>
        /// 由连字符分隔的19位唯一字符串  
        /// </summary>
        /// <returns></returns>
        public static string GuidTo19String()
        {
            string guid = Pivots.Commons.Snowflake.Instance().GetId().ToString().Trim();
            return guid;
        }

        /// <summary>  
        /// 根据GUID获取16位的唯一字符串  
        /// </summary>  
        /// <param name=\"guid\"></param>  
        /// <returns></returns>  
        public static string GuidTo16String()
        {
            long i = 1;
            foreach (byte b in Guid.NewGuid().ToByteArray())
                i *= ((int)b + 1);
            return string.Format("{0:x}", i - DateTime.Now.Ticks);
        }
        /// <summary>  
        /// 根据GUID获取19位的唯一数字序列  
        /// </summary>  
        /// <returns></returns>  
        public static long GuidToLongID()
        {
            byte[] buffer = Guid.NewGuid().ToByteArray();
            return BitConverter.ToInt64(buffer, 0);
        }

        /// <summary>
        /// 唯一订单号生成
        /// </summary>
        /// <returns></returns>
        public static string GenerateOrderNumber()
        {
            string strDateTimeNumber = DateTime.Now.ToString("yyyyMMddHHmmssms");
            string strRandomResult = NextRandom(1000, 1).ToString();
            return strDateTimeNumber + strRandomResult;
        }
        /// <summary>
        /// 参考：msdn上的RNGCryptoServiceProvider例子
        /// </summary>
        /// <param name="numSeeds"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        private static int NextRandom(int numSeeds, int length)
        {
            // Create a byte array to hold the random value.  
            byte[] randomNumber = new byte[length];
            // Create a new instance of the RNGCryptoServiceProvider.  
            System.Security.Cryptography.RNGCryptoServiceProvider rng = new System.Security.Cryptography.RNGCryptoServiceProvider();
            // Fill the array with a random value.  
            rng.GetBytes(randomNumber);
            // Convert the byte to an uint value to make the modulus operation easier.  
            uint randomResult = 0x0;
            for (int i = 0; i < length; i++)
            {
                randomResult |= ((uint)randomNumber[i] << ((length - 1 - i) * 8));
            }
            return (int)(randomResult % numSeeds) + 1;
        }
    }
}
