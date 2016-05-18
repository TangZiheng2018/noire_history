using System.IO;

namespace Noire.Common {
    public static class NoireConfiguration {

        static NoireConfiguration() {
            ResourceBase = string.Empty;
        }

        public static string ResourceBase { get; set; }

        public static string GetFullResourcePath(string relativePath) {
            return Path.Combine(Directory.GetCurrentDirectory(), ResourceBase, relativePath);
        }

    }
}
