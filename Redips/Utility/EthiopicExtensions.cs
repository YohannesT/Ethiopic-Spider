using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Spider.Utility
{
    public static class EthiopicExtensions
    {
        public static List<string> GetEthipicWords(this string text)
        {
            var split = new[] { ';', ' ', ':', '.', '\n' };
            return text.Split(split).Where(ContainsEthipic).Select(w => w.Trim()).ToList();
        }

        public static List<string> GetEthipicParagraphs(this string text)
        {
            var split = new[] {'\n' };
            return text.Split(split).Where(ContainsEthipic).Select(w => w.Trim()).ToList();
        }

        public static bool ContainsEthipic(this string text)
        {
            return text.ToCharArray().ToList().Any(t => (int)t > 4608 && (int)t < 4988);
        }

        public async static Task<List<string>> GetEthipicParagraphsAsync(this string text)
        {
            return await Task.Run(() =>
            {
                var split = new[] {'\n'};
                return text.Split(split).Where(ContainsEthipic).Select(w => w.Trim()).ToList();
            });
        }

        public async static Task<List<string>> GetEthipicWordsAsync(this string text)
        {
            return await Task.Run(() =>
            {
                var split = new[] { ';', ' ', ':', '.' };
                return text.Split(split).Where(ContainsEthipic).Select(w => w.Trim()).ToList();
            });
        }

        public static async Task<bool> ContainsEthipicAsync(this string text)
        {
            return await Task.Run(() => text.ToCharArray().ToList().Any(t => (int)t > 4608 && (int)t < 4988));
        }
    }
}