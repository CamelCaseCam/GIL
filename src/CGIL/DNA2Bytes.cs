using System;
using System.Text;
using System.Collections;

public static class DNA2Bytes
{
    public static byte[] ToBytes(string DNA)    //[ a t c g g ] => [ 11 10 01 00, 00 00 00 00 ]
    {
        int NumBits = DNA.Length * 2;    //Get the number of bits needed
        byte[] NumEncodedBits = BitConverter.GetBytes(NumBits);    //Store this in the first 4 bytes

        if (NumEncodedBits.Length > 4)
        {
            HelperFunctions.WriteError("Error GIL12: Cannot encode sequence bigger than max length (2,147,483,648 chars)");
        }

        byte[] Output = new byte[((NumBits + 8 - (NumBits % 8))/8) + 4];    //Create the byte array
        for (int i = 0; i < 4; i++)
        {
            Output[i] = NumEncodedBits[i];    //Store the number of encoded bytes in the first 4 bytes
        }
        int BitIdx = 32;
        int CurrentBit = 0b0;
        int CurrentByte = 0b0;
        foreach (char letter in DNA.ToLower())    //the first bit corresponds to whether it's at or cg and the second one corresponds to whether it's ac or tg 
        {
            switch (letter)
            {
                case 'a':
                    CurrentBit = 0b11;
                    break;
                case 'u':
                    goto case 't';
                case 't':
                    CurrentBit = 0b10;
                    break;
                case 'c':
                    CurrentBit = 0b01;
                    break;
                case 'g':
                    CurrentBit = 0b00;
                    break;
                default:
                    HelperFunctions.WriteError("Can't convert non-DNA/RNA char to byte");
                    break;
            }
            switch (BitIdx % 8)
            {
                case 0:
                    CurrentByte += CurrentBit * 64;
                    break;
                case 2:
                    CurrentByte += CurrentBit * 16;
                    break;
                case 4:
                    CurrentByte += CurrentBit * 4;
                    break;
                case 6:
                    CurrentByte += CurrentBit;
                    break;
                default:
                    HelperFunctions.WriteError("Error converting DNA to bytes");
                    break;

            }
            BitIdx += 2;
            if (BitIdx % 8 == 0)
            {
                Output[(BitIdx / 8) - 1] = (byte)CurrentByte;
                CurrentByte = 0b0;
            }
        }
        if (CurrentByte != 0b0)
        {
            Output[Output.Length - 1] = (byte)CurrentByte;
        }
        return Output;
    }

    public static string FromBytes(byte[] bytes)
    {
        uint NumBits = 0;
        for (int i = 0; i < 4; i++)
        {
            NumBits += bytes[i] * (uint)Math.Pow(256, i);
        }
        StringBuilder OutputString = new StringBuilder("", (int)NumBits / 2);
        uint CurrentBit = 0;
        for (int i = 4; i < bytes.Length; i++)
        {
            var bits = new BitArray( new byte[] {bytes[i]});
            for (int b = 7; b > 0; b -= 2)
            {
                if (CurrentBit >= NumBits)
                {
                    break;
                }

                if (bits[b])
                {
                    if (bits[b - 1])
                    {
                        OutputString.Append('a');
                    } else
                    {
                        OutputString.Append('t');
                    }
                } else
                {
                    if (bits[b - 1])
                    {
                        OutputString.Append('c');
                    } else
                    {
                        OutputString.Append('g');
                    }
                }

                CurrentBit += 2;
            }
        }
        return OutputString.ToString();
    }
}