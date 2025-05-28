
using System.Text;
namespace KcTabWindow.ViewData;
public class DES
{
    public static string StrEnc(string data, string firstKey, string secondKey, string thirdKey)
    {
        int leng = data.Length;
        string encData = "";
        List<int[]> firstKeyBt = null, secondKeyBt = null, thirdKeyBt = null;
        int firstLength = 0, secondLength = 0, thirdLength = 0;

        if (!string.IsNullOrEmpty(firstKey))
        {
            firstKeyBt = GetKeyBytes(firstKey);
            firstLength = firstKeyBt.Count;
        }
        if (!string.IsNullOrEmpty(secondKey))
        {
            secondKeyBt = GetKeyBytes(secondKey);
            secondLength = secondKeyBt.Count;
        }
        if (!string.IsNullOrEmpty(thirdKey))
        {
            thirdKeyBt = GetKeyBytes(thirdKey);
            thirdLength = thirdKeyBt.Count;
        }

        if (leng > 0)
        {
            if (leng < 4)
            {
                int[] bt = StrToBt(data);
                int[] encByte = null;

                if (firstKey != null && secondKey != null && thirdKey != null)
                {
                    int[] tempBt = bt;
                    for (int x = 0; x < firstLength; x++)
                    {
                        tempBt = Enc(tempBt, firstKeyBt[x]);
                    }
                    for (int y = 0; y < secondLength; y++)
                    {
                        tempBt = Enc(tempBt, secondKeyBt[y]);
                    }
                    for (int z = 0; z < thirdLength; z++)
                    {
                        tempBt = Enc(tempBt, thirdKeyBt[z]);
                    }
                    encByte = tempBt;
                }
                else if (firstKey != null && secondKey != null)
                {
                    int[] tempBt = bt;
                    for (int x = 0; x < firstLength; x++)
                    {
                        tempBt = Enc(tempBt, firstKeyBt[x]);
                    }
                    for (int y = 0; y < secondLength; y++)
                    {
                        tempBt = Enc(tempBt, secondKeyBt[y]);
                    }
                    encByte = tempBt;
                }
                else if (firstKey != null)
                {
                    int[] tempBt = bt;
                    for (int x = 0; x < firstLength; x++)
                    {
                        tempBt = Enc(tempBt, firstKeyBt[x]);
                    }
                    encByte = tempBt;
                }
                encData = Bt64ToHex(encByte);
                Console.WriteLine("01:" + encData);
            }
            else
            {
                int iterator = leng / 4;
                int remainder = leng % 4;
                for (int i = 0; i < iterator; i++)
                {
                    string tempData = data.Substring(i * 4, 4);
                    int[] tempByte = StrToBt(tempData);
                    int[] encByte = null;

                    if (firstKey != null && secondKey != null && thirdKey != null)
                    {
                        int[] tempBt = tempByte;
                        for (int x = 0; x < firstLength; x++)
                        {
                            tempBt = Enc(tempBt, firstKeyBt[x]);
                        }
                        for (int y = 0; y < secondLength; y++)
                        {
                            tempBt = Enc(tempBt, secondKeyBt[y]);
                        }
                        for (int z = 0; z < thirdLength; z++)
                        {
                            tempBt = Enc(tempBt, thirdKeyBt[z]);
                        }
                        encByte = tempBt;
                    }
                    else if (firstKey != null && secondKey != null)
                    {
                        int[] tempBt = tempByte;
                        for (int x = 0; x < firstLength; x++)
                        {
                            tempBt = Enc(tempBt, firstKeyBt[x]);
                        }
                        for (int y = 0; y < secondLength; y++)
                        {
                            tempBt = Enc(tempBt, secondKeyBt[y]);
                        }
                        encByte = tempBt;
                    }
                    else if (firstKey != null)
                    {
                        int[] tempBt = tempByte;
                        for (int x = 0; x < firstLength; x++)
                        {
                            tempBt = Enc(tempBt, firstKeyBt[x]);
                        }
                        encByte = tempBt;
                    }
                    encData += Bt64ToHex(encByte);
                    Console.WriteLine("02:" + encData);
                }
                if (remainder > 0)
                {
                    string remainderData = data.Substring(iterator * 4, leng - iterator * 4);
                    int[] tempByte = StrToBt(remainderData);
                    int[] encByte = null;

                    if (firstKey != null && secondKey != null && thirdKey != null)
                    {
                        int[] tempBt = tempByte;
                        for (int x = 0; x < firstLength; x++)
                        {
                            tempBt = Enc(tempBt, firstKeyBt[x]);
                        }
                        for (int y = 0; y < secondLength; y++)
                        {
                            tempBt = Enc(tempBt, secondKeyBt[y]);
                        }
                        for (int z = 0; z < thirdLength; z++)
                        {
                            tempBt = Enc(tempBt, thirdKeyBt[z]);
                        }
                        encByte = tempBt;
                    }
                    else if (firstKey != null && secondKey != null)
                    {
                        int[] tempBt = tempByte;
                        for (int x = 0; x < firstLength; x++)
                        {
                            tempBt = Enc(tempBt, firstKeyBt[x]);
                        }
                        for (int y = 0; y < secondLength; y++)
                        {
                            tempBt = Enc(tempBt, secondKeyBt[y]);
                        }
                        encByte = tempBt;
                    }
                    else if (firstKey != null)
                    {
                        int[] tempBt = tempByte;
                        for (int x = 0; x < firstLength; x++)
                        {
                            tempBt = Enc(tempBt, firstKeyBt[x]);
                        }
                        encByte = tempBt;
                    }
                    encData += Bt64ToHex(encByte);
                    Console.WriteLine("03:" + encData);
                }
            }
        }
        return encData;
    }

    public static string StrDec(string data, string firstKey, string secondKey, string thirdKey)
    {
        int leng = data.Length;
        string decStr = "";
        List<int[]> firstKeyBt = null, secondKeyBt = null, thirdKeyBt = null;
        int firstLength = 0, secondLength = 0, thirdLength = 0;

        if (!string.IsNullOrEmpty(firstKey))
        {
            firstKeyBt = GetKeyBytes(firstKey);
            firstLength = firstKeyBt.Count;
        }
        if (!string.IsNullOrEmpty(secondKey))
        {
            secondKeyBt = GetKeyBytes(secondKey);
            secondLength = secondKeyBt.Count;
        }
        if (!string.IsNullOrEmpty(thirdKey))
        {
            thirdKeyBt = GetKeyBytes(thirdKey);
            thirdLength = thirdKeyBt.Count;
        }

        int iterator = leng / 16;
        for (int i = 0; i < iterator; i++)
        {
            string tempData = data.Substring(i * 16, 16);
            string strByte = HexToBt64(tempData);
            int[] intByte = new int[64];
            for (int j = 0; j < 64; j++)
            {
                intByte[j] = int.Parse(strByte[j].ToString());
            }
            int[] decByte = null;

            if (firstKey != null && secondKey != null && thirdKey != null)
            {
                int[] tempBt = intByte;
                for (int x = thirdLength - 1; x >= 0; x--)
                {
                    tempBt = Dec(tempBt, thirdKeyBt[x]);
                }
                for (int y = secondLength - 1; y >= 0; y--)
                {
                    tempBt = Dec(tempBt, secondKeyBt[y]);
                }
                for (int z = firstLength - 1; z >= 0; z--)
                {
                    tempBt = Dec(tempBt, firstKeyBt[z]);
                }
                decByte = tempBt;
            }
            else if (firstKey != null && secondKey != null)
            {
                int[] tempBt = intByte;
                for (int x = secondLength - 1; x >= 0; x--)
                {
                    tempBt = Dec(tempBt, secondKeyBt[x]);
                }
                for (int y = firstLength - 1; y >= 0; y--)
                {
                    tempBt = Dec(tempBt, firstKeyBt[y]);
                }
                decByte = tempBt;
            }
            else if (firstKey != null)
            {
                int[] tempBt = intByte;
                for (int x = firstLength - 1; x >= 0; x--)
                {
                    tempBt = Dec(tempBt, firstKeyBt[x]);
                }
                decByte = tempBt;
            }
            decStr += ByteToString(decByte);
        }
        return decStr;
    }

    private static List<int[]> GetKeyBytes(string key)
    {
        List<int[]> keyBytes = new List<int[]>();
        int leng = key.Length;
        int iterator = leng / 4;
        int remainder = leng % 4;

        for (int i = 0; i < iterator; i++)
        {
            keyBytes.Add(StrToBt(key.Substring(i * 4, 4)));
        }
        if (remainder > 0)
        {
            keyBytes.Add(StrToBt(key.Substring(iterator * 4, leng - iterator * 4)));
        }
        return keyBytes;
    }

    private static int[] StrToBt(string s)
    {
        int[] bt = new int[64];
        int leng = s.Length;

        for (int i = 0; i < leng; i++)
        {
            int k = s[i];
            for (int j = 0; j < 16; j++)
            {
                int powVal = 1;
                for (int m = 15; m > j; m--)
                {
                    powVal *= 2;
                }
                bt[16 * i + j] = (k / powVal) % 2;
            }
        }
        for (int p = leng; p < 4; p++)
        {
            int k = 0;
            for (int q = 0; q < 16; q++)
            {
                int powVal = 1;
                for (int m = 15; m > q; m--)
                {
                    powVal *= 2;
                }
                bt[16 * p + q] = (k / powVal) % 2;
            }
        }
        return bt;
    }

    private static string Bt4ToHex(string binary)
    {
        Dictionary<string, string> mapping = new Dictionary<string, string>
        {
            { "0000", "0" },
            { "0001", "1" },
            { "0010", "2" },
            { "0011", "3" },
            { "0100", "4" },
            { "0101", "5" },
            { "0110", "6" },
            { "0111", "7" },
            { "1000", "8" },
            { "1001", "9" },
            { "1010", "A" },
            { "1011", "B" },
            { "1100", "C" },
            { "1101", "D" },
            { "1110", "E" },
            { "1111", "F" }
        };
        return mapping[binary];
    }

    private static string HexToBt4(string hexDigit)
    {
        Dictionary<string, string> mapping = new Dictionary<string, string>
        {
            { "0", "0000" },
            { "1", "0001" },
            { "2", "0010" },
            { "3", "0011" },
            { "4", "0100" },
            { "5", "0101" },
            { "6", "0110" },
            { "7", "0111" },
            { "8", "1000" },
            { "9", "1001" },
            { "A", "1010" },
            { "B", "1011" },
            { "C", "1100" },
            { "D", "1101" },
            { "E", "1110" },
            { "F", "1111" },
            { "a", "1010" },
            { "b", "1011" },
            { "c", "1100" },
            { "d", "1101" },
            { "e", "1110" },
            { "f", "1111" }
        };
        return mapping[hexDigit];
    }

    private static string ByteToString(int[] byteData)
    {
        StringBuilder s = new StringBuilder();
        for (int i = 0; i < 4; i++)
        {
            int count = 0;
            for (int j = 0; j < 16; j++)
            {
                int powVal = 1;
                for (int m = 15; m > j; m--)
                {
                    powVal *= 2;
                }
                count += byteData[16 * i + j] * powVal;
            }
            if (count != 0)
            {
                s.Append((char)count);
            }
        }
        return s.ToString();
    }

    private static string Bt64ToHex(int[] byteData)
    {
        StringBuilder hexStr = new StringBuilder();
        for (int i = 0; i < 16; i++)
        {
            StringBuilder bt = new StringBuilder();
            for (int j = 0; j < 4; j++)
            {
                bt.Append(byteData[i * 4 + j]);
            }
            hexStr.Append(Bt4ToHex(bt.ToString()));
        }
        return hexStr.ToString();
    }

    private static string HexToBt64(string hexStr)
    {
        StringBuilder binary = new StringBuilder();
        for (int i = 0; i < 16; i++)
        {
            binary.Append(HexToBt4(hexStr[i].ToString()));
        }
        return binary.ToString();
    }

    private static int[] Enc(int[] dataByte, int[] keyByte)
    {
        int[][] keys = GenerateKeys(keyByte);
        int[] ipByte = InitPermute(dataByte);
        int[] ipLeft = new int[32];
        int[] ipRight = new int[32];
        Array.Copy(ipByte, 0, ipLeft, 0, 32);
        Array.Copy(ipByte, 32, ipRight, 0, 32);

        for (int i = 0; i < 16; i++)
        {
            int[] tempLeft = ipLeft;
            ipLeft = ipRight;
            int[] key = keys[i];
            int[] tempRight = Xor(Permute(SBoxPermute(Xor(ExpandPermute(ipRight), key))), tempLeft);
            ipRight = tempRight;
        }
        int[] finalData = new int[64];
        Array.Copy(ipRight, 0, finalData, 0, 32);
        Array.Copy(ipLeft, 0, finalData, 32, 32);
        return FinallyPermute(finalData);
    }

    private static int[] Dec(int[] dataByte, int[] keyByte)
    {
        int[][] keys = GenerateKeys(keyByte);
        int[] ipByte = InitPermute(dataByte);
        int[] ipLeft = new int[32];
        int[] ipRight = new int[32];
        Array.Copy(ipByte, 0, ipLeft, 0, 32);
        Array.Copy(ipByte, 32, ipRight, 0, 32);

        for (int i = 15; i >= 0; i--)
        {
            int[] tempLeft = ipLeft;
            ipLeft = ipRight;
            int[] key = keys[i];
            int[] tempRight = Xor(Permute(SBoxPermute(Xor(ExpandPermute(ipRight), key))), tempLeft);
            ipRight = tempRight;
        }
        int[] finalData = new int[64];
        Array.Copy(ipRight, 0, finalData, 0, 32);
        Array.Copy(ipLeft, 0, finalData, 32, 32);
        return FinallyPermute(finalData);
    }

    private static int[] InitPermute(int[] originalData)
    {
        int[] ipByte = new int[64];
        for (int i = 0; i < 4; i++)
        {
            int m = 1 + i * 2;
            int n = 0 + i * 2;
            for (int j = 0; j < 8; j++)
            {
                int k = j;
                ipByte[i * 8 + k] = originalData[(7 - j) * 8 + m];
                ipByte[i * 8 + k + 32] = originalData[(7 - j) * 8 + n];
            }
        }
        return ipByte;
    }

    private static int[] ExpandPermute(int[] rightData)
    {
        int[] epByte = new int[48];
        for (int i = 0; i < 8; i++)
        {
            if (i == 0)
            {
                epByte[i * 6 + 0] = rightData[31];
            }
            else
            {
                epByte[i * 6 + 0] = rightData[i * 4 - 1];
            }
            epByte[i * 6 + 1] = rightData[i * 4 + 0];
            epByte[i * 6 + 2] = rightData[i * 4 + 1];
            epByte[i * 6 + 3] = rightData[i * 4 + 2];
            epByte[i * 6 + 4] = rightData[i * 4 + 3];
            if (i == 7)
            {
                epByte[i * 6 + 5] = rightData[0];
            }
            else
            {
                epByte[i * 6 + 5] = rightData[i * 4 + 4];
            }
        }
        return epByte;
    }

    private static int[] Xor(int[] byteOne, int[] byteTwo)
    {
        int[] result = new int[byteOne.Length];
        for (int i = 0; i < byteOne.Length; i++)
        {
            result[i] = byteOne[i] ^ byteTwo[i];
        }
        return result;
    }

    private static int[] SBoxPermute(int[] expandByte)
    {
        int[] sBoxByte = new int[32];
        int[][] s1 = new int[][]
        {
            new int[] { 14, 4, 13, 1, 2, 15, 11, 8, 3, 10, 6, 12, 5, 9, 0, 7 },
            new int[] { 0, 15, 7, 4, 14, 2, 13, 1, 10, 6, 12, 11, 9, 5, 3, 8 },
            new int[] { 4, 1, 14, 8, 13, 6, 2, 11, 15, 12, 9, 7, 3, 10, 5, 0 },
            new int[] { 15, 12, 8, 2, 4, 9, 1, 7, 5, 11, 3, 14, 10, 0, 6, 13 }
        };
        int[][] s2 = new int[][]
        {
            new int[] { 15, 1, 8, 14, 6, 11, 3, 4, 9, 7, 2, 13, 12, 0, 5, 10 },
            new int[] { 3, 13, 4, 7, 15, 2, 8, 14, 12, 0, 1, 10, 6, 9, 11, 5 },
            new int[] { 0, 14, 7, 11, 10, 4, 13, 1, 5, 8, 12, 6, 9, 3, 2, 15 },
            new int[] { 13, 8, 10, 1, 3, 15, 4, 2, 11, 6, 7, 12, 0, 5, 14, 9 }
        };
        int[][] s3 = new int[][]
        {
            new int[] { 10, 0, 9, 14, 6, 3, 15, 5, 1, 13, 12, 7, 11, 4, 2, 8 },
            new int[] { 13, 7, 0, 9, 3, 4, 6, 10, 2, 8, 5, 14, 12, 11, 15, 1 },
            new int[] { 13, 6, 4, 9, 8, 15, 3, 0, 11, 1, 2, 12, 5, 10, 14, 7 },
            new int[] { 1, 10, 13, 0, 6, 9, 8, 7, 4, 15, 14, 3, 11, 5, 2, 12 }
        };
        int[][] s4 = new int[][]
        {
            new int[] { 7, 13, 14, 3, 0, 6, 9, 10, 1, 2, 8, 5, 11, 12, 4, 15 },
            new int[] { 13, 8, 11, 5, 6, 15, 0, 3, 4, 7, 2, 12, 1, 10, 14, 9 },
            new int[] { 10, 6, 9, 0, 12, 11, 7, 13, 15, 1, 3, 14, 5, 2, 8, 4 },
            new int[] { 3, 15, 0, 6, 10, 1, 13, 8, 9, 4, 5, 11, 12, 7, 2, 14 }
        };
        int[][] s5 = new int[][]
        {
            new int[] { 2, 12, 4, 1, 7, 10, 11, 6, 8, 5, 3, 15, 13, 0, 14, 9 },
            new int[] { 14, 11, 2, 12, 4, 7, 13, 1, 5, 0, 15, 10, 3, 9, 8, 6 },
            new int[] { 4, 2, 1, 11, 10, 13, 7, 8, 15, 9, 12, 5, 6, 3, 0, 14 },
            new int[] { 11, 8, 12, 7, 1, 14, 2, 13, 6, 15, 0, 9, 10, 4, 5, 3 }
        };
        int[][] s6 = new int[][]
        {
            new int[] { 12, 1, 10, 15, 9, 2, 6, 8, 0, 13, 3, 4, 14, 7, 5, 11 },
            new int[] { 10, 15, 4, 2, 7, 12, 9, 5, 6, 1, 13, 14, 0, 11, 3, 8 },
            new int[] { 9, 14, 15, 5, 2, 8, 12, 3, 7, 0, 4, 10, 1, 13, 11, 6 },
            new int[] { 4, 3, 2, 12, 9, 5, 15, 10, 11, 14, 1, 7, 6, 0, 8, 13 }
        };
        int[][] s7 = new int[][]
        {
            new int[] { 4, 11, 2, 14, 15, 0, 8, 13, 3, 12, 9, 7, 5, 10, 6, 1 },
            new int[] { 13, 0, 11, 7, 4, 9, 1, 10, 14, 3, 5, 12, 2, 15, 8, 6 },
            new int[] { 1, 4, 11, 13, 12, 3, 7, 14, 10, 15, 6, 8, 0, 5, 9, 2 },
            new int[] { 6, 11, 13, 8, 1, 4, 10, 7, 9, 5, 0, 15, 14, 2, 3, 12 }
        };
        int[][] s8 = new int[][]
        {
            new int[] { 13, 2, 8, 4, 6, 15, 11, 1, 10, 9, 3, 14, 5, 0, 12, 7 },
            new int[] { 1, 15, 13, 8, 10, 3, 7, 4, 12, 5, 6, 11, 0, 14, 9, 2 },
            new int[] { 7, 11, 4, 1, 9, 12, 14, 2, 0, 6, 10, 13, 15, 3, 5, 8 },
            new int[] { 2, 1, 14, 7, 4, 10, 8, 13, 15, 12, 9, 0, 3, 5, 6, 11 }
        };

        for (int m = 0; m < 8; m++)
        {
            int i = expandByte[m * 6 + 0] * 2 + expandByte[m * 6 + 5];
            int j = (expandByte[m * 6 + 1] << 3) + (expandByte[m * 6 + 2] << 2) + (expandByte[m * 6 + 3] << 1) + expandByte[m * 6 + 4];
            int[] sBox = null;
            switch (m)
            {
                case 0: sBox = s1[i]; break;
                case 1: sBox = s2[i]; break;
                case 2: sBox = s3[i]; break;
                case 3: sBox = s4[i]; break;
                case 4: sBox = s5[i]; break;
                case 5: sBox = s6[i]; break;
                case 6: sBox = s7[i]; break;
                case 7: sBox = s8[i]; break;
            }
            string binary = GetBoxBinary(sBox[j]);
            sBoxByte[m * 4 + 0] = int.Parse(binary[0].ToString());
            sBoxByte[m * 4 + 1] = int.Parse(binary[1].ToString());
            sBoxByte[m * 4 + 2] = int.Parse(binary[2].ToString());
            sBoxByte[m * 4 + 3] = int.Parse(binary[3].ToString());
        }
        return sBoxByte;
    }

    private static int[] Permute(int[] sBoxByte)
    {
        int[] pBoxPermute = new int[32];
        pBoxPermute[0] = sBoxByte[15];
        pBoxPermute[1] = sBoxByte[6];
        pBoxPermute[2] = sBoxByte[19];
        pBoxPermute[3] = sBoxByte[20];
        pBoxPermute[4] = sBoxByte[28];
        pBoxPermute[5] = sBoxByte[11];
        pBoxPermute[6] = sBoxByte[27];
        pBoxPermute[7] = sBoxByte[16];
        pBoxPermute[8] = sBoxByte[0];
        pBoxPermute[9] = sBoxByte[14];
        pBoxPermute[10] = sBoxByte[22];
        pBoxPermute[11] = sBoxByte[25];
        pBoxPermute[12] = sBoxByte[4];
        pBoxPermute[13] = sBoxByte[17];
        pBoxPermute[14] = sBoxByte[30];
        pBoxPermute[15] = sBoxByte[9];
        pBoxPermute[16] = sBoxByte[1];
        pBoxPermute[17] = sBoxByte[7];
        pBoxPermute[18] = sBoxByte[23];
        pBoxPermute[19] = sBoxByte[13];
        pBoxPermute[20] = sBoxByte[31];
        pBoxPermute[21] = sBoxByte[26];
        pBoxPermute[22] = sBoxByte[2];
        pBoxPermute[23] = sBoxByte[8];
        pBoxPermute[24] = sBoxByte[18];
        pBoxPermute[25] = sBoxByte[12];
        pBoxPermute[26] = sBoxByte[29];
        pBoxPermute[27] = sBoxByte[5];
        pBoxPermute[28] = sBoxByte[21];
        pBoxPermute[29] = sBoxByte[10];
        pBoxPermute[30] = sBoxByte[3];
        pBoxPermute[31] = sBoxByte[24];
        return pBoxPermute;
    }

    private static int[] FinallyPermute(int[] endByte)
    {
        int[] fpByte = new int[64];
        fpByte[0] = endByte[39];
        fpByte[1] = endByte[7];
        fpByte[2] = endByte[47];
        fpByte[3] = endByte[15];
        fpByte[4] = endByte[55];
        fpByte[5] = endByte[23];
        fpByte[6] = endByte[63];
        fpByte[7] = endByte[31];
        fpByte[8] = endByte[38];
        fpByte[9] = endByte[6];
        fpByte[10] = endByte[46];
        fpByte[11] = endByte[14];
        fpByte[12] = endByte[54];
        fpByte[13] = endByte[22];
        fpByte[14] = endByte[62];
        fpByte[15] = endByte[30];
        fpByte[16] = endByte[37];
        fpByte[17] = endByte[5];
        fpByte[18] = endByte[45];
        fpByte[19] = endByte[13];
        fpByte[20] = endByte[53];
        fpByte[21] = endByte[21];
        fpByte[22] = endByte[61];
        fpByte[23] = endByte[29];
        fpByte[24] = endByte[36];
        fpByte[25] = endByte[4];
        fpByte[26] = endByte[44];
        fpByte[27] = endByte[12];
        fpByte[28] = endByte[52];
        fpByte[29] = endByte[20];
        fpByte[30] = endByte[60];
        fpByte[31] = endByte[28];
        fpByte[32] = endByte[35];
        fpByte[33] = endByte[3];
        fpByte[34] = endByte[43];
        fpByte[35] = endByte[11];
        fpByte[36] = endByte[51];
        fpByte[37] = endByte[19];
        fpByte[38] = endByte[59];
        fpByte[39] = endByte[27];
        fpByte[40] = endByte[34];
        fpByte[41] = endByte[2];
        fpByte[42] = endByte[42];
        fpByte[43] = endByte[10];
        fpByte[44] = endByte[50];
        fpByte[45] = endByte[18];
        fpByte[46] = endByte[58];
        fpByte[47] = endByte[26];
        fpByte[48] = endByte[33];
        fpByte[49] = endByte[1];
        fpByte[50] = endByte[41];
        fpByte[51] = endByte[9];
        fpByte[52] = endByte[49];
        fpByte[53] = endByte[17];
        fpByte[54] = endByte[57];
        fpByte[55] = endByte[25];
        fpByte[56] = endByte[32];
        fpByte[57] = endByte[0];
        fpByte[58] = endByte[40];
        fpByte[59] = endByte[8];
        fpByte[60] = endByte[48];
        fpByte[61] = endByte[16];
        fpByte[62] = endByte[56];
        fpByte[63] = endByte[24];
        return fpByte;
    }

    private static string GetBoxBinary(int i)
    {
        Dictionary<int, string> mapping = new Dictionary<int, string>
        {
            { 0, "0000" },
            { 1, "0001" },
            { 2, "0010" },
            { 3, "0011" },
            { 4, "0100" },
            { 5, "0101" },
            { 6, "0110" },
            { 7, "0111" },
            { 8, "1000" },
            { 9, "1001" },
            { 10, "1010" },
            { 11, "1011" },
            { 12, "1100" },
            { 13, "1101" },
            { 14, "1110" },
            { 15, "1111" }
        };
        return mapping[i];
    }

    private static int[][] GenerateKeys(int[] keyByte)
    {
        int[] key = new int[56];
        int[][] keys = new int[16][];
        int[] loop = new int[] { 1, 1, 2, 2, 2, 2, 2, 2, 1, 2, 2, 2, 2, 2, 2, 1 };

        for (int i = 0; i < 7; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                int k = 7 - j;
                key[i * 8 + j] = keyByte[k * 8 + i];
            }
        }

        for (int i = 0; i < 16; i++)
        {
            for (int j = 0; j < loop[i]; j++)
            {
                int tempLeft = key[0];
                int tempRight = key[28];
                for (int k = 0; k < 27; k++)
                {
                    key[k] = key[k + 1];
                    key[28 + k] = key[29 + k];
                }
                key[27] = tempLeft;
                key[55] = tempRight;
            }
            int[] tempKey = new int[48];
            tempKey[0] = key[13];
            tempKey[1] = key[16];
            tempKey[2] = key[10];
            tempKey[3] = key[23];
            tempKey[4] = key[0];
            tempKey[5] = key[4];
            tempKey[6] = key[2];
            tempKey[7] = key[27];
            tempKey[8] = key[14];
            tempKey[9] = key[5];
            tempKey[10] = key[20];
            tempKey[11] = key[9];
            tempKey[12] = key[22];
            tempKey[13] = key[18];
            tempKey[14] = key[11];
            tempKey[15] = key[3];
            tempKey[16] = key[25];
            tempKey[17] = key[7];
            tempKey[18] = key[15];
            tempKey[19] = key[6];
            tempKey[20] = key[26];
            tempKey[21] = key[19];
            tempKey[22] = key[12];
            tempKey[23] = key[1];
            tempKey[24] = key[40];
            tempKey[25] = key[51];
            tempKey[26] = key[30];
            tempKey[27] = key[36];
            tempKey[28] = key[46];
            tempKey[29] = key[54];
            tempKey[30] = key[29];
            tempKey[31] = key[39];
            tempKey[32] = key[50];
            tempKey[33] = key[44];
            tempKey[34] = key[32];
            tempKey[35] = key[47];
            tempKey[36] = key[43];
            tempKey[37] = key[48];
            tempKey[38] = key[38];
            tempKey[39] = key[55];
            tempKey[40] = key[33];
            tempKey[41] = key[52];
            tempKey[42] = key[45];
            tempKey[43] = key[41];
            tempKey[44] = key[49];
            tempKey[45] = key[35];
            tempKey[46] = key[28];
            tempKey[47] = key[31];
            keys[i] = tempKey;
        }
        return keys;
    }
}