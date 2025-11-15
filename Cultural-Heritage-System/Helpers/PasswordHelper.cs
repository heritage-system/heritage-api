using System.Security.Cryptography;
using System.Text;

namespace Cultural_Heritage_System.Helpers
{
    public static class PasswordHelper
    {
        private const string Uppercase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private const string Lowercase = "abcdefghijklmnopqrstuvwxyz";
        private const string Digits = "0123456789";
        private const string Special = "!@#$%^&*()-_=+<>?";

        public static string GenerateRandomPassword(int length = 12)
        {
            if (length < 8)
                throw new ArgumentException("Password length should be at least 8 characters.");

            string allChars = Uppercase + Lowercase + Digits + Special;
            StringBuilder password = new StringBuilder();
            using (var rng = RandomNumberGenerator.Create())
            {
                // đảm bảo có ít nhất 1 ký tự của mỗi loại
                password.Append(GetRandomChar(rng, Uppercase));
                password.Append(GetRandomChar(rng, Lowercase));
                password.Append(GetRandomChar(rng, Digits));
                password.Append(GetRandomChar(rng, Special));

                // phần còn lại chọn ngẫu nhiên
                for (int i = 4; i < length; i++)
                {
                    password.Append(GetRandomChar(rng, allChars));
                }
            }

            // Trộn ký tự để tránh pattern cố định
            return ShuffleString(password.ToString());
        }

        private static char GetRandomChar(RandomNumberGenerator rng, string charset)
        {
            byte[] randomByte = new byte[1];
            rng.GetBytes(randomByte);
            return charset[randomByte[0] % charset.Length];
        }

        private static string ShuffleString(string input)
        {
            char[] array = input.ToCharArray();
            using (var rng = RandomNumberGenerator.Create())
            {
                for (int i = array.Length - 1; i > 0; i--)
                {
                    byte[] box = new byte[1];
                    do rng.GetBytes(box);
                    while (!(box[0] < i * (Byte.MaxValue / i)));
                    int j = box[0] % (i + 1);
                    (array[i], array[j]) = (array[j], array[i]);
                }
            }
            return new string(array);
        }
    }
}
