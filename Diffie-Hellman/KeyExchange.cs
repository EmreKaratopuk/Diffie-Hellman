namespace Diffie_Hellman;

public static class KeyExchange
{
    public static (ulong, ulong) GenerateKeys(ulong primeNumber, ulong baseNumber, ulong privateA, ulong privateB)
    {
        ulong x = ModulusCalc.GetModPow(baseNumber, privateA, primeNumber);
        ulong y = ModulusCalc.GetModPow(baseNumber, privateB, primeNumber);
        
        // person a receives y from person b
        ulong secret = ModulusCalc.GetModPow(y, privateA, primeNumber);
        
        // person b receives x from person a
        ulong secret2 = ModulusCalc.GetModPow(x, privateB, primeNumber);

        if (secret != secret2)
        {
            throw new InvalidOperationException("Failed: Shared secrets do not match!");
        }

        Console.WriteLine($"Public prime number p: {primeNumber}");
        Console.WriteLine($"Public primitive root for p: {baseNumber}\n");
        
        Console.WriteLine($"PersonA secret: {privateA}");
        Console.WriteLine($"PersonB secret: {privateB}");
        Console.WriteLine($"PersonA sent: {x}");
        Console.WriteLine($"PersonB sent: {y}\n");

        Console.WriteLine($"Secret: {secret}");

        return (x, y);
    }
}