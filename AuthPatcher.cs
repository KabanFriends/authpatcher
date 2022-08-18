using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using Newtonsoft.Json.Linq;

namespace authpatcher
{
    class AuthPatcher
    {

        private static Dictionary<string, PatchConfig> binConfigs = new Dictionary<string, PatchConfig>()
        {
            {
                "19ac306ce237057791a2897a61c35b5f749f21ca",
                new PatchConfig(0x0003B1C0, 15, 0x0003B200, 15, 0x0000D9B3)
            }
        };

        static void Main(string[] args)
        {
            if (args.Length >= 1)
            {
                if (args.Contains("-d"))
                {
                    Console.WriteLine("PARTY☆NIGHT :D");
                    Process.Start("https://www.youtube.com/watch?v=8_BB1TNCpHw");
                    return;
                }

                if (!File.Exists("patch.json"))
                {
                    Console.WriteLine("patch.json not found! ratio + l bozo");
                    return;
                }

                JObject json;

                using (var reader = new StreamReader("patch.json", Encoding.UTF8))
                {
                    json = JObject.Parse(reader.ReadToEnd());
                }

                string fileName = args[0];

                if (!File.Exists(fileName))
                {
                    Console.WriteLine("file not found! ratio + l bozo");
                    return;
                }

                string hash;

                using (var stream = File.OpenRead(fileName))
                {
                    SHA1 sha = new SHA1CryptoServiceProvider();
                    hash = BitConverter.ToString(sha.ComputeHash(stream)).Replace("-", "").ToLower();

                    if (args.Contains("-h"))
                    {
                        Console.WriteLine("sha1 hash of " + fileName + ": " + hash);
                        return;
                    }
                }

                string newFileName = fileName.Substring(0, fileName.Length - 4) + "-patched.exe";

                if (File.Exists(newFileName))
                {
                    File.Delete(newFileName);
                }

                File.Copy(fileName, newFileName);

                using (var stream = File.OpenWrite(newFileName))
                {
                    PatchConfig config;
                    if (binConfigs.TryGetValue(hash, out config))
                    {
                        string routerIp = json.Value<string>("routerIp");
                        string serverIp = json.Value<string>("serverIp");

                        if (routerIp.Length > config.routerMaxLength)
                        {
                            Console.WriteLine("router ip too long! (max " + config.routerMaxLength + ")");
                            return;
                        }

                        if (serverIp.Length > config.serverMaxLength)
                        {
                            Console.WriteLine("server ip too long! (max " + config.serverMaxLength + ")");
                            return;
                        }

                        byte[] routerBytes = Encoding.ASCII.GetBytes(routerIp);
                        stream.Seek(config.routerLocation, SeekOrigin.Begin);
                        foreach (byte b in routerBytes)
                        {
                            stream.WriteByte(b);
                        }
                        stream.WriteByte(0x00);

                        byte[] serverBytes = Encoding.ASCII.GetBytes(serverIp);
                        stream.Seek(config.serverLocation, SeekOrigin.Begin);
                        foreach (byte b in serverBytes)
                        {
                            stream.WriteByte(b);
                        }
                        stream.WriteByte(0x00);

                        stream.Seek(config.serverLengthLocation, SeekOrigin.Begin);
                        stream.WriteByte((byte)serverBytes.Length);

                        Console.WriteLine("patching done!");
                    } else
                    {
                        Console.WriteLine("file cannot be patched! (no matching sha1)");
                    }
                }

                return;
            }

            Console.WriteLine(Assembly.GetExecutingAssembly().GetName().Name + " <exeFile> [parameter]");
            Console.WriteLine("-h: check file hash (does not patch)");
            Console.WriteLine("-d: my favorite video");
            
        }
    }
}
