using Diffie_Hellman;

(ulong primeNumber, ulong baseNumber, ulong userSecret1, ulong userSecret2) = UserInput.GetNumbers();
(ulong sentMessage1, ulong sentMessage2) = KeyExchange.GenerateKeys(primeNumber, baseNumber, userSecret1, userSecret2);
// sentMessage = baseNumber ^ userSecret (mod primeNumber)

bool attemptBruteForce = UserInput.AskBruteForce();
if (attemptBruteForce) BruteForce.FindSecret(primeNumber, baseNumber, sentMessage1, sentMessage2);

