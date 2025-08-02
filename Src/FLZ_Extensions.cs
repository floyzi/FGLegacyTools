using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ThatOneRandom3AMProject
{
    internal static class FLZ_Extensions
    {
        public static byte[] GetEmbeddedRes(string path)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var targ = assembly.GetManifestResourceNames().FirstOrDefault(r => r.EndsWith(path, StringComparison.OrdinalIgnoreCase));

            if (targ == null)
            {
                var builder = new StringBuilder();
                foreach (var resource in assembly.GetManifestResourceNames())
                    builder.AppendLine(resource);

                throw new Exception($"NO CONTENT WERE FOUND AT PATH:\n{path}\n\nPossible resources:\n{builder}");
            }

            var resStream = assembly.GetManifestResourceStream(targ);
            var memStream = new MemoryStream();
            resStream.CopyTo(memStream);
            return memStream.ToArray();
        }

    }
}
