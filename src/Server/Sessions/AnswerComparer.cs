using System.Globalization;
using System.Text;

namespace BingoLingo.Server.Sessions
{
    public class AnswerComparer
    {
        public bool IsAcceptable(string s1, string s2)
        {
            var n1 = Normalize(s1);
            var n2 = Normalize(s2);

            return string.Equals(n1, n2, StringComparison.OrdinalIgnoreCase);
        }

        private string Normalize(string s)
        {
            var normalized = s.Normalize(NormalizationForm.FormD);
            bool Acceptable(char c) => 
                CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark &&
               (c == '\'' || !Char.IsPunctuation(c));
            var chars = normalized
                .Where(Acceptable).ToArray();
            return new string(chars);
        }
    }
}
