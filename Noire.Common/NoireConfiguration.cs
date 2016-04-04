using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Noire.Common {
    public static class NoireConfiguration {

        static NoireConfiguration() {
            ResourceBase = string.Empty;
        }

        public static string ResourceBase { get; set; }

        public static string GetFullResourcePath(string relativePath) {
            return Path.Combine(ResourceBase, relativePath);
        }

    }
}
