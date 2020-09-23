using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;


public static class HashCalculator
{
    static string GetLongestCommonPrefix(string[] s)
    {
        int k = s[0].Length;
        for (int i = 1; i < s.Length; i++)
        {
            k = Math.Min(k, s[i].Length);
            for (int j = 0; j < k; j++)
                if (s[i][j] != s[0][j])
                {
                    k = j;
                    break;
                }
        }

        return s[0].Substring(0, k);
    }

    static string getDir(string path) =>
        System.IO.Path.GetDirectoryName(path);

    static string getRelativePath(string prefix, string filename) =>
        filename.Substring(prefix.Length);

    static string calcHash(string str) =>
        GetHashSha256(str);

    public static string HashFiles(IEnumerable<(string name, string contents)> files)
    {
        IEnumerable<string> dirs = files.Select(x => getDir(x.name));
        string prefix = GetLongestCommonPrefix(dirs.ToArray());
        IEnumerable<(string relativeName, string contents)> relativeDirs =
            files.Select(x => (getRelativePath(prefix, x.name), x.contents));
        IEnumerable<(string relativeName, string contents)> sortedFiles = relativeDirs.OrderBy(x => x.relativeName);
        IEnumerable<string> hashes =
            relativeDirs.Select(x => calcHash(calcHash(x.relativeName) + calcHash(x.contents)));
        string gluedContent = hashes.OrderBy(x => x).Aggregate((a, b) => a + b);
        string hash = calcHash(gluedContent);
        return hash;
    }

    public static string GetHashSha256(string text)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(text);
        SHA256Managed hashstring = new SHA256Managed();
        byte[] hash = hashstring.ComputeHash(bytes);
        string hashString = string.Empty;
        foreach (byte x in hash)
        {
            hashString += String.Format("{0:x2}", x);
        }

        return hashString;
    }

}