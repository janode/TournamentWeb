using System.Text;

namespace TournamentWeb.Services
{
    public class FakeConsole
    {
        private static StringBuilder _builder;

        public static void Init()
        {
            _builder = new StringBuilder();
        }

        public static void WriteLine(string value)
        {
            _builder.AppendLine(value);
        }

        public static string Print()
        {
            return _builder.ToString();
        }
    }
}
