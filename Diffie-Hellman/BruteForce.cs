namespace Diffie_Hellman;

public static class BruteForce
{
    public static void FindSecret(ulong primeNumber, ulong baseNumber,
        ulong sentMessage1, ulong sentMessage2)
    {
        for (ulong trialIndex = primeNumber - 2; trialIndex > 0; trialIndex--)
        {
            if (sentMessage1 == ModulusCalc.GetModPow(baseNumber, trialIndex, primeNumber))
            {
                // trialIndex is the secret of one of the peers
                ulong secret = ModulusCalc.GetModPow(sentMessage2, trialIndex, primeNumber);
                Console.WriteLine($"Shared secret is found successfully. It's {secret}");

                return;
            }
        }
        
        Console.WriteLine("Could not find the secret, please check the provided numbers :(");
    }
}