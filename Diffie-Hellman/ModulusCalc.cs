namespace Diffie_Hellman;

public static class ModulusCalc
{
    public static ulong Pow(ulong value, ulong exponent)
    {
        // bit overflow is possible
        ulong result = value;
        if (exponent == 0) return 1;
        
        for (; exponent > 1; exponent--) result *= value;

        return result;
    }
    public static ulong SquareRoot(ulong number)
    {
        int b = 31;
        ulong r = 0;
        ulong r2 = 0;

        while (b >= 0)
        {
            ulong sr = r;
            ulong sr2 = r2;

            r2 += (r << (1 + b)) + (1ul << (b + b));
            r += (1ul << b);

            if (r2 > number)
            {
                r = sr;
                r2 = sr2;
            }

            b--;
        }

        return r;
    }
    private static int FindMostSignificantSetBit(ulong number)
    {
        if (number == 0) return -1;

        int position = 0;
        while ((number >>= 1) != 0) position++;

        return position;
    }
    
    private static ulong Karatsuba(ulong factor1, int msb1, ulong factor2, int msb2, ulong modulus)
    {
        int smallerMsb = msb1 < msb2 ? msb1 : msb2;
        if (smallerMsb > 31) smallerMsb = 31;
        ulong mask = (1ul << smallerMsb) - 1;

        ulong x1 = factor1 >> smallerMsb;
        ulong x0 = factor1 & mask;

        ulong y1 = factor2 >> smallerMsb;
        ulong y0 = factor2 & mask;

        ulong z2 = x1 * y1;
        ulong z0 = x0 * y0;
        ulong z1 = (x1 + x0) * (y1 + y0) - z2 - z0;

        ulong[] sections = {z2, z1, z0};
        List<ulong> results = new List<ulong>();
        
        for (int degree = 2; degree >= 0; degree--)
        {
            results.Add(sections[2-degree] % modulus);
            for (int power = degree * smallerMsb; power > 0; power--)
            {
                results[2 - degree] <<= 1;
                results[2 - degree] %= modulus;
            }
        }

        ulong temp = (results[0] + results[1]) % modulus;
        ulong result = (temp + results[2]) % modulus;
        
        return result;
    }
    
    public static ulong GetModPow(ulong value, ulong exponent, ulong modulus)
    {
        value %= modulus;
        ulong oddMultiplier = 1;
        int msb1;
        int msb2;
        
        while (exponent > 0)
        {
            if ((exponent & 1) == 1) // if exponent is odd
            {
                msb1 = FindMostSignificantSetBit(oddMultiplier);
                msb2 = FindMostSignificantSetBit(value);
                
                if (msb1 + msb2 + 2 > 64) oddMultiplier = Karatsuba(oddMultiplier, msb1, value, msb2, modulus);
                else oddMultiplier = (oddMultiplier * value) % modulus;
                
                exponent -= 1;
                continue;
            }
            
            msb2 = FindMostSignificantSetBit(value);
            
            if (msb2 * 2 + 2 > 64) value = Karatsuba(value, msb2, value, msb2, modulus);
            else value = value*value % modulus;

            exponent /= 2;
        }
        
        ulong result = oddMultiplier % modulus;
        return result;
    }
}