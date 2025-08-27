namespace Diffie_Hellman;

public static class UserInput
{
    private static (ulong, ulong) GetUserSecretNumbers()
    {
        List<ulong> userSecrets = new List<ulong>();

        while (userSecrets.Count < 2)
        {
            Console.WriteLine(
                "Please enter one of the users secret number, enter 0 to randomly generate");
            string? input = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(input))
            {
                Console.WriteLine("Please enter a number");
                continue;
            }

            ulong numericalInput;
            if (!UInt64.TryParse(input, out numericalInput))
            {
                Console.WriteLine(
                    "Please enter a number smaller than 18,446,744,073,709,551,616 (2^64)");
                continue;
            }

            if (numericalInput == 0)
            {
                // generate a random large number
                userSecrets.Add(GeneratePrime.GetRandomPrimeCandidate());
            }
            else userSecrets.Add(numericalInput);
        }

        return (userSecrets[0], userSecrets[1]);
    }

    public static (ulong, ulong, ulong, ulong) GetNumbers()
    {
        string? input;
        ulong primeNumber;
        ulong baseNumber;

        // get a prime number
        Console.WriteLine("Please enter a prime number, enter 0 to randomly generate");
        Console.WriteLine(
            "If prime number is generated randomly, then primitive root (base number) will be generated randomly as well");

        while (true)
        {
            input = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(input))
            {
                Console.WriteLine("Please enter a number");
                continue;
            }

            if (!UInt64.TryParse(input, out primeNumber))
            {
                Console.WriteLine(
                    "Please enter a number smaller than 9,223,372,036,854,775,808 (2^63)");
                continue;
            }

            if ((primeNumber & (1ul << 63)) ==
                1) // check if number is 64 bits, should be at most 63 bits
            {
                // in Karatsuba algorithm numbers might exceed 64 bits if modulus is 64 bits
                Console.WriteLine(
                    "Please enter a prime number smaller than 9,223,372,036,854,775,808 (2^63)");
                continue;
            }

            if (primeNumber != 0 && (!GeneratePrime.SieveOfEratosthenes(primeNumber) ||
                                     !GeneratePrime.RabinMiller(primeNumber)))
            {
                Console.WriteLine(
                    "Given number is not a prime number, please enter a prime a number");
                continue;
            }

            break;
        }

        if (primeNumber == 0)
        {
            while (true)
            {
                ulong randomPrimeCandidate = GeneratePrime.GetRandomPrimeCandidate();
                if (!GeneratePrime.SieveOfEratosthenes(randomPrimeCandidate)) continue;
                if (!GeneratePrime.RabinMiller(randomPrimeCandidate)) continue;

                ulong root = PrimitiveRoot.FindPrimitiveRoot(randomPrimeCandidate);

                if (root != 1) // if root is 1, program could not find any primitive roots for the given prime
                {
                    primeNumber = randomPrimeCandidate;
                    baseNumber = root;
                    break;
                }
            }
        }

        else
        {
            Console.WriteLine(
                "Please enter a base number which is a primitive root for the given prime number, " +
                "enter 0 to randomly generate");

            while (true)
            {
                input = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(input))
                {
                    Console.WriteLine("Please enter a base number");
                    continue;
                }

                if (!UInt64.TryParse(input, out baseNumber))
                {
                    Console.WriteLine(
                        "Please enter a number smaller than 18,446,744,073,709,551,616 (2^64)");
                    continue;
                }

                if (baseNumber == 0)
                {
                    baseNumber =
                        PrimitiveRoot.FindPrimitiveRoot(primeNumber, saveTime: false);
                    break;
                }

                if (!PrimitiveRoot.CheckPrimitiveRoot(primeNumber, baseNumber))
                {
                    Console.WriteLine(
                        "Given number is not a primitive root for the given prime number");
                    continue;
                }

                break;
            }
        }

        (ulong userSecret1, ulong userSecret2) = GetUserSecretNumbers();

        return (primeNumber, baseNumber, userSecret1, userSecret2);
    }

    public static bool AskBruteForce()
    {
        Console.WriteLine(
            "Would you like to start the brute force process to find the secret? yes/no");
        while (true)
        {
            string? findSecret = Console.ReadLine();
            if (string.IsNullOrEmpty(findSecret))
            {
                Console.WriteLine("Please type your answer");
                continue;
            }

            findSecret = findSecret.ToLower();
            if (findSecret is "yes" or "y") return true;

            if (findSecret is "no" or "n")
            {
                Console.Write("Bye");
                return false;
            }

            Console.WriteLine(
                "Valid answers are yes, y, no, and n. Please type your answer again");
        }
    }
}