namespace CanvasLMS.Utilities
{
    public class PasswordUtility
    {
        private const string UppercaseChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private const string LowercaseChars = "abcdefghijklmnopqrstuvwxyz";
        private const string NumericChars = "0123456789";

        public static string GenerateRandomPassword(int length)
        {
            var random = new Random();
            var passwordChars = UppercaseChars + LowercaseChars + NumericChars;

            var password = new char[length];
            password[0] = UppercaseChars[random.Next(UppercaseChars.Length)]; // Ensure at least one uppercase
            password[1] = LowercaseChars[random.Next(LowercaseChars.Length)]; // Ensure at least one lowercase
            password[2] = NumericChars[random.Next(NumericChars.Length)]; // Ensure at least one number

            for (int i = 3; i < length; i++)
            {
                password[i] = passwordChars[random.Next(passwordChars.Length)];
            }

            // Shuffle the password characters
            for (int i = 0; i < length; i++)
            {
                int swapIndex = random.Next(length);
                char temp = password[i];
                password[i] = password[swapIndex];
                password[swapIndex] = temp;
            }

            return new string(password);
        }
    }
}
