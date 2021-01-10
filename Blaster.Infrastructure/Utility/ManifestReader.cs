using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Blaster.Infrastructure.Utility
{
    internal class ManifestDataReader
    {
        internal static string Get<TSource>(string embeddedFileName, object data) where TSource : class
        {
            var assembly = typeof(TSource).GetTypeInfo().Assembly;
            var resourceName = assembly.GetManifestResourceNames().First(s => s.EndsWith(embeddedFileName, StringComparison.CurrentCultureIgnoreCase));

            using (var stream = assembly.GetManifestResourceStream(resourceName))
            {
                if (stream == null)
                {
                    throw new InvalidOperationException("Could not load manifest resource stream.");
                }

                var sb = new StringBuilder();

                using (var reader = new StreamReader(stream))
                {
                    while (reader.Peek() >= 0)
                    {
                        sb.Append(reader.ReadLine());
                    }

                    var dataDict = data.ToDictionary();
                    if (dataDict != null)
                    {
                        foreach (var item in dataDict)
                        {
                            sb = sb.Replace(string.Concat("{", item.Key, "}"), item.Value.ToString());
                        }
                    }

                    return sb.ToString();
                }
            }
        }

        
    }
}
