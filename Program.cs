using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;
namespace Steganography
{
    internal class Program
    {
        public static string path = "";
        private static string file = "";
        private static string userMessage = "";
        static void Main(string[] args)
        {
            path = Directory.GetCurrentDirectory();
            Console.WriteLine("The result will be saved as results.jpg. The decoded version will be saved as: decoded.txt \n Pease specify the file to work with at(e.g. image.jpg): " + path + @"\" + "\n");
            file = path + @"\" + Console.ReadLine();
            Console.WriteLine("0 - Encode \n1 - Decode");
            int answer = Convert.ToInt32(Console.ReadLine());
            if (answer == 0)
            {
                Console.Write("Type the message you want to include in the image: ");
                userMessage = Console.ReadLine();
                Encode(userMessage);
            }
            else if (answer == 1)
            {
                Decode();
            }
            Console.ReadLine();
        }
        public static void SaveOutput(string saveName, string toSave)
        {
            StreamWriter sw = new StreamWriter(saveName);
            sw.Write(toSave);
            sw.Close();
        }
        public static void Decode()
        {
            //Reading bitmap
            List<byte> message = new List<byte>();
            Bitmap bmap = new Bitmap(file, true);
            int binaryIndex = 0;

            int[] currentByte = new int[8];
            int currentByteIndex = 0;
            for (int x = 0; x < bmap.Width; x++)
            {
                for (int y = 0; y < bmap.Height; y++)
                {
                    if(currentByteIndex == 7)
                    {
                        currentByteIndex = 0;
                        message.Add(ConvertStringByteToByte(CreateStringByte(currentByte)));
                        binaryIndex++;
                    }
                    Color color = bmap.GetPixel(x, y);
                    byte red = color.R;
                    currentByte[currentByteIndex] = IsBitSet(red, 0);
                    currentByteIndex++;
                }
            }


            //Decoding bytes
            string final = Encoding.ASCII.GetString(message.ToArray());
            SaveOutput(path + @"\decoded.txt", final);
            Console.WriteLine(final);
        }
        public static void Encode(string toEncode)
        {
            //Constructing message
            string message = toEncode;
            byte[] binaryData = Encoding.ASCII.GetBytes(message);

            //Making Bitmap
            Bitmap bmap = new Bitmap(file, true);
            int binaryIndex = 0;
            int[] currentByte = new int[8];
            int currentByteIndex = 7;

            ///<summary>
            ///Basically, we have converted the message to bytes.
            ///Now, we loop through all those bytes bit by bit(currentByte + currentByteIndex)
            ///And put them in the last bit of the color's R
            ///After we reached the end of the byte, we get a new one and continue until we got all the bytes
            ///</summary>
            //The !((binaryIndex >= binaryData.Length) && currentByteIndex == 7) check is needed, because the last byte wouldn't get rendered properly,
            //as the binaryIndex would have gone up way before the currentByteIndex could finish rendering the byte(so at currentByteIndex = 1, it would stop)
            for (int x = 0; x < bmap.Width && !((binaryIndex >= binaryData.Length) && currentByteIndex == 7); x++)
            {
                for (int y = 0; y < bmap.Height && !((binaryIndex >= binaryData.Length) && currentByteIndex == 7); y++)
                {
                    if(currentByteIndex == 7)
                    {
                        currentByteIndex = 0;
                        currentByte = ByteToBit(binaryData[binaryIndex]);
                        binaryIndex++;
                    }
                    Color color = bmap.GetPixel(x, y);
                    int oRed = color.R;
                    //    int newRed = binaryData[binaryIndex]; --red stores 1 byte
                    int newRed = ChangeBitInInt(oRed, 0, currentByte[currentByteIndex]);
                    color = Color.FromArgb(color.A, newRed, color.G, color.B);
                    bmap.SetPixel(x, y, color);
                    currentByteIndex++;
                }
            }
            Bitmap result = new Bitmap(bmap);
            result.Save(path + @"\result.jpg");
        }
        //If a bit is set, returns 1, else 0(pos 0 = least important bit)
        public static int IsBitSet(byte b, int pos)
        {
            return ((b >> pos) & 1);
        }
        //Creates an array of ones and zeroes with the first array element corresponding to the least important bit
        private static int[] ByteToBit(byte toConvert)
        {
            int[] returning = new int[8];
            for(int i = 0; i < 8; i++)
            {
                returning[i] = IsBitSet(toConvert, i);
            }
            return returning;
        }
        private static int ChangeBitInInt(int orig, int pos, int value)
        {
            if (value == 1)
            {
                // Set the bit at 'pos' to 1 using bitwise OR
                return orig | (1 << pos);
            }
            else
            {
                // Set the bit at 'pos' to 0 using bitwise AND with inverted mask
                return orig & ~(1 << pos);
            }
        }
        private static byte ConvertStringByteToByte(string input)
        {
            return Convert.ToByte(input, 2);
        }
        private static string CreateStringByte(int[] bits)
        {
            string returning = "";
            for(int i = 7; i >= 0; i--)
            {
                returning += bits[i];
            }
            return returning;
        }
    }
}
