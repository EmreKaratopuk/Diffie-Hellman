namespace Diffie_Hellman;

public static class PrimitiveRoot
{
    
    public static ulong FindPrimitiveRoot(ulong primeNumber, bool saveTime = true)
    {
        ulong smoothNumber;
        if (saveTime) smoothNumber = 63132259;
        else smoothNumber = ulong.MaxValue;
        
        HashSet<ulong>? factors = FindPrimeFactors(primeNumber - 1, smoothNumber: smoothNumber);
        if (factors == null) return 1;
        
        return CheckFactors(primeNumber, factors);
    }

    public static bool CheckPrimitiveRoot(ulong primeNumber, ulong rootCandidate)
    {
        ulong phi = primeNumber - 1;
        HashSet<ulong>? factors = FindPrimeFactors(primeNumber - 1, smoothNumber: ulong.MaxValue);

        bool rootCandidateSuccess = true;
        foreach (var factor in factors!)
        {
            ulong result = ModulusCalc.GetModPow(rootCandidate, phi / factor, primeNumber);
            if (result == 1)
            {
                rootCandidateSuccess = false; 
                break; // rootCandidate failed, rootCandidate is not a valid primitive root for the given prime number
            }

        }
        
        return rootCandidateSuccess;
    }
    
    private static HashSet<ulong>? FindPrimeFactors(ulong phi, ulong smoothNumber = 63132259)
    {
        HashSet<ulong> factors = new HashSet<ulong>{phi};

        // check factors 2 and 3
        for (ulong smallFactor = 2; smallFactor < 4; smallFactor++)
        {
            if (phi % smallFactor == 0)
            {
                factors.Add(smallFactor);
                
                do phi /= smallFactor;
                while (phi % smallFactor == 0);
            }
        }

        // for prime number p, p = 6n + 1 or p = 6n - 1 | p = 6n + 5 or p = 6n + 7 
        // k: 6n
        for (ulong k = 5; k <= phi / 2; k += 6)
        {
            if (k > smoothNumber) return null; // phi(primeNumber) has large factors pick another prime to save time
            foreach (var variance in new ulong[]{0, 2})
            {
                ulong factorCandidate = k + variance;
                if (phi % factorCandidate == 0)
                {
                    factors.Add(factorCandidate);
                    
                    do phi /= factorCandidate;
                    while (phi % factorCandidate == 0);

                    if (GeneratePrime.RabinMiller(phi))
                    {
                        // the smallest factor of phi is phi, since it is a prime 
                        factors.Add(phi);
                        return factors;
                    }
                }   
            }
        }

        if (phi > 1) factors.Add(phi);

        return factors;
    }

    private static ulong CheckFactors(ulong primeNumber, HashSet<ulong> factors)
    {
        ulong phi = primeNumber - 1;

        var rand = new Random();
        ulong min = 2;
        ulong max = phi;
        ulong range = max - min;
        
        for (var index = phi; index > 1; index--)
        {
            ulong rootCandidate = (ulong)rand.NextInt64();
            rootCandidate = (rootCandidate % range) + min;
            
            bool rootCandidateSuccess = true;
            
            foreach (var factor in factors)
            {
                ulong result = ModulusCalc.GetModPow(rootCandidate, phi / factor, primeNumber);
                if (result == 1)
                {
                    rootCandidateSuccess = false; 
                    break; // rootCandidate failed, try the next one
                }
            }
            
            if (rootCandidateSuccess) return rootCandidate;
        }

        return 1; // could not find any
    }
    
}