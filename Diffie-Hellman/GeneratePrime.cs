using System.Collections;

namespace Diffie_Hellman;

public static class GeneratePrime
{
    // bitLength has to be 64 since all operations will be done with unsigned long vars 
    private static readonly int BitLength = 64;

    static readonly Random Rand = new();
    public static ulong GetRandomPrimeCandidate()
    {
        byte[] initByteArray = new byte[BitLength / 8];
        Rand.NextBytes(initByteArray); // Generate {bitLength} number of random bits.
    
        BitArray initBitArray = new BitArray(initByteArray);
        
        // set msb to 0 to ensure primeCandidate is equal and smaller than 2^63,
        // because in Karatsuba algorithm numbers might exceed 64 bits if modulus is 64 bits
        initBitArray.Set(BitLength - 1, false);
        
        initBitArray.Set(BitLength - 2, true); // set second msb to 1 to ensure the number is large 
        initBitArray.Set(0, true); // set lsb to 1 to ensure number is odd

        int[] tempIntArray = new int[2];
        initBitArray.CopyTo(tempIntArray, 0);

        ulong randomCandidate = (uint)tempIntArray[0] + ((ulong)(uint)tempIntArray[1] << 32);

        return randomCandidate;
    }

    private static readonly ulong[] FirstPrimes =
    {
        2, 3, 5, 7, 11, 13, 17, 19, 23, 29,
        31, 37, 41, 43, 47, 53, 59, 61, 67,
        71, 73, 79, 83, 89, 97, 101, 103,
        107, 109, 113, 127, 131, 137, 139,
        149, 151, 157, 163, 167, 173, 179,
        181, 191, 193, 197, 199, 211, 223,
        227, 229, 233, 239, 241, 251, 257,
        263, 269, 271, 277, 281, 283, 293,
        307, 311, 313, 317, 331, 337, 347, 349
    };
    public static bool SieveOfEratosthenes(ulong primeCandidate) // Low level primality test
    {
        foreach (var smallPrime in FirstPrimes)
        {
            if (primeCandidate % smallPrime == 0) return false;
        }

        return true;
    }

    public static bool RabinMiller(ulong primeCandidate, int trialNumber = 128)
    {
        // (n-1) = r*(2^s), n: prime candidate
        ulong s = 0;
        ulong r = primeCandidate - 1;

        // find r and s
        while ((int)(r & 1) == 0) // check if the lsb is 0, in other words, is it even
        {
            s += 1;
            r >>= 1;
        }

        if (ModulusCalc.Pow(2, s) * r != primeCandidate - 1) return false; // formula did not hold for primeCandidate
        
        byte[] randomBytes = new byte[BitLength / 8];
        ulong min = 1;
        ulong max = primeCandidate - 2;
        ulong range = max - min;
        
        for (int trialIndex = 0; trialIndex < trialNumber; trialIndex++)
        {
            // a^r = 1 (mod n) or a^((2^j)r) = -1 (mod n) for some j such that 0 ≤ j ≤ s-1 (primality condition)
            
            // Generate random a
            Rand.NextBytes(randomBytes);
            ulong a = BitConverter.ToUInt64(randomBytes, 0);
            a = (a % range) + min;

            ulong result =  ModulusCalc.GetModPow(a, r, primeCandidate);
            if (result == 1 || result == primeCandidate - 1) continue; // trial passed try another random base (a)
            
            bool success = false;
            for (ulong testIndex = 1; testIndex < s; testIndex++)
            {
                ulong testResult = ModulusCalc.GetModPow(a, ModulusCalc.Pow(2, testIndex) * r, primeCandidate);
                if (testResult == primeCandidate - 1)
                {
                    success = true;
                    break; // trial passed 
                }
            }

            if (!success) return false; 
            // Trial failed, primeCandidate is likely a composite number, or base a is a strong liar for primeCandidate 
        }

        return true;
    }
}