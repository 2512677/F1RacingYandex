// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("Yviv6VmzNMidJSJ7sccQNRV3IYKxK+3Qlxway55mdF+rY2Q/bwieQQdL9jDWSiQBDxIy8/MUks6fOhiinDY5k9e4NErNIbJfPOYtSSkqtJwhx4EMEo2hxzL4R0q/dlm8zvLlobHcAUYYwC69Le1BipaHqcrJ43obcxLEr4uwWLBzlTTqs+/eL8ljqgrnCb+d+EFSpiVwvE+zPLSq2pnxudWxg/xWQv9WU7eCS/9BJ9CTr8vh21hWWWnbWFNb21hYWe+NPTKuZKAnhGl6fyTFdZbJnJ60yKztcfifc1Ve1T1s3ZgN1d6zdLnq0lWAFla0adtYe2lUX1Bz3xHfrlRYWFhcWVpMsR/TfjrXlGjluEf3E65mydCd3qSRcse8fJ8TIltaWFlY");
        private static int[] order = new int[] { 5,13,2,7,7,7,10,9,11,13,10,12,13,13,14 };
        private static int key = 89;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
