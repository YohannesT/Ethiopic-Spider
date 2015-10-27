using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Redips.Utility
{
    public static class EthiopicExtensions
    {
        public static List<string> GetEthiopicWords(this string text)
        {
            var split = new[] { ';', ' ', ':', '.', '\n' };
            return text.Split(split).Where(ContainsEthiopic).Select(w => w.Trim()).ToList();
        }

        public static List<string> GetEthiopicParagraphs(this string text)
        {
            var split = new[] {'\n' };
            return text.Split(split).Where(ContainsEthiopic).Select(w => w.Trim()).ToList();
        }

        public static bool ContainsEthiopic(this string text)
        {
            return text.ToCharArray().ToList().Any(t => (int)t > 4608 && (int)t < 4988);
        }

        public async static Task<List<string>> GetEthiopicParagraphsAsync(this string text)
        {
            return await Task.Run(() =>
            {
                var split = new[] {'\n'};
                return text.Split(split).Where(ContainsEthiopic).Select(w => w.Trim()).ToList();
            });
        }

        public async static Task<List<string>> GetEthiopicWordsAsync(this string text)
        {
            return await Task.Run(() =>
            {
                var split = new[] { ';', ' ', ':', '.' };
                return text.Split(split).Where(ContainsEthiopic).Select(w => w.Trim()).ToList();
            });
        }

        public static async Task<bool> ContainsEthiopicAsync(this string text)
        {
            return await Task.Run(() => text.ToCharArray().ToList().Any(t => (int)t > 4608 && (int)t < 4988));
        }
    }
}