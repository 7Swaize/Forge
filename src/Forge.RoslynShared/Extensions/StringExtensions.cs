using System.Text.RegularExpressions;

public static class StringExtensions {
    extension(string self) {
        public string CollapseRedundantWhitespace() {
            return Regex.Replace(self, @"[ \t]+", " ");
        }
    }
}