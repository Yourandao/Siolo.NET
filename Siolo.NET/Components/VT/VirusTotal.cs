using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Net.Http;
using System.Security.Cryptography;
using Newtonsoft.Json;

namespace Siolo.NET.Components.VT
{
   class VirusTotal
   {
      private readonly string sApiBase = "https://www.virustotal.com/api/v3/";
      private readonly bool bPublicApi = true;
      private readonly int iPublicSleep = 10;
      private readonly string sApiKeysFile;
      private readonly string sSigsFile;

      private static DateTime dtLastPublicCall = new DateTime(0);

      public VirusTotal(string api_keys_file, string sigs_file)
      {
         sApiKeysFile = api_keys_file;
         sSigsFile = sigs_file;
      }

      public static string GetMd5FromBytes(byte[] file_bytes)
      {
         using (MD5 md5Hash = MD5.Create())
         {
            byte[] hash = md5Hash.ComputeHash(file_bytes);

            return string.Join("", (from b in hash select b.ToString("x2")));
         }
      }

      public static string GetMd5File(string file_path)
      {
         return GetMd5FromBytes(File.ReadAllBytes(file_path));
      }

      private void PublicWait()
      {
         if (!bPublicApi)
            return;

         TimeSpan delta = DateTime.Now - dtLastPublicCall;
         dtLastPublicCall = DateTime.Now;

         if (delta.TotalSeconds > iPublicSleep)
         {
            return;
         }

         Thread.Sleep(iPublicSleep * 1000 - delta.Milliseconds);
      }

      private string GetCurrentKey()
      {
         PublicWait();

         try
         {
            string[] keys = File.ReadAllLines(sApiKeysFile);

            File.WriteAllLines(sApiKeysFile, keys.Skip(1).Concat(keys.Take(1)));

            return keys[0];
         }
         catch (Exception exc)
         {
            Console.WriteLine($"[Error] <VirusTotal::GetCurrentKey: {exc.Message}");
            return "";
         }
      }

      private HttpClient GetHttpClient()
      {
         HttpClient result = new HttpClient();
         result.DefaultRequestHeaders.Add("x-apikey", GetCurrentKey());

         return result;
      }

      public async Task<string> GetVtFileInfoAsync(string file_hash)
      {
         try
         {
            // TODO: if hash not in mongo:
            string url = $"{sApiBase}files/{file_hash}";
            HttpClient client = GetHttpClient();

            return await client.GetStringAsync(url);
         }
         catch (Exception exc)
         {
            Console.WriteLine($"[Error] <VirusTotal::GetVtFileInfoAsync: {exc.Message}");
            return "";
         }
      }

      public async Task<string> GetKasperskyShortVerdictAsync(string file_hash)
      {
         try
         {
            VTReport report = JsonConvert.DeserializeObject<VTReport>(await GetVtFileInfoAsync(file_hash));

            if (report.error != null)
            {
               return "UNKN";
            }

            Kaspersky kasp_obj = report.data.attributes.last_analysis_results.Kaspersky;

            if (kasp_obj.category == "undetected")
            {
               return "CLER";
            }

            string long_verdict = kasp_obj.result;
            int colon_idx = long_verdict.IndexOf(':') + 1;
            string short_verdict = long_verdict.Substring(colon_idx, long_verdict.IndexOf('.') - colon_idx);

            return short_verdict.Substring(0, 4).ToUpper();
         }
         catch (Exception exc)
         {
            Console.WriteLine($"[Error] <VirusTotal::GetKasperskyShortVerdictAsync: {exc.Message}");
            return "UNKN";
         }
      }

      public async Task<string> GetKasperskyShortVerdictFromBytesAsync(byte[] file_bytes)
      {
         return await GetKasperskyShortVerdictAsync(GetMd5FromBytes(file_bytes));
      }

      public string GetFileTypeFromBytes(byte[] file_bytes)
      {
         try
         {
            if (file_bytes == null || file_bytes.Length == 0)
            {
               return "UNKN";
            }


            VTSigRootobject data = JsonConvert.DeserializeObject<VTSigRootobject>(File.ReadAllText(sSigsFile));

            foreach (VTSig elem in data.sig_array)
            {
               foreach (string sig in elem.signature)
               {
                  int offset = elem.offset * 2 + elem.offset;

                  string cur_sig = string.Join(" ", (from b in file_bytes.Skip(elem.offset).Take((sig.Length + 1) / 3)
                                                     select b.ToString("X2")).ToArray());

                  if (sig.Equals(cur_sig))
                  {
                     return elem.type;
                  }
               }
            }

            return "UNKN";
         }
         catch (Exception exc)
         {
            Console.WriteLine($"[Error] <VirusTotal::GetFileTypeFromBytes: {exc.Message}");
            return "UNKN";
         }
      }

      public async Task<string> GetFullFileClassFromBytesAsync(byte[] file_bytes)
      {
         return $"{await GetKasperskyShortVerdictFromBytesAsync(file_bytes)}:{GetFileTypeFromBytes(file_bytes)}";
      }

      public async Task<string> GetFullFileClassAsync(string file_path)
      {
         return await GetFullFileClassFromBytesAsync(File.ReadAllBytes(file_path));
      }
   }
}
