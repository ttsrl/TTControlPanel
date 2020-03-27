using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TTControlPanel.Models;
using TTControlPanel.Utilities;
using static TTControlPanel.Models.ProductKey;
using static TTControlPanel.Utilities.Utilities;

namespace TTControlPanel.Services
{
    public class Utils
    {
        private readonly DBContext _dB;

        public Utils()
        {
            _dB = DBContext.Instance;
        }

        public async Task<string> GenerateProdutKey(PKType type, ApplicationVersion appV, Client client, TimeSpan? time = null)
        {
            try
            {
                var prods = await _dB.ProductKeys.Select(p => p.Key).ToListAsync();
                while (true)
                {
                    var pk = "";
                    if (type == PKType.Normal)
                        pk = generatePkNormal(appV, client);
                    else if (type == PKType.Time)
                        pk = generatePkTime(appV, client, (TimeSpan)time);
                    else if (type == PKType.Trial)
                        pk = generatePkTrial(appV, client, (TimeSpan)time);
                    if (!prods.Contains(pk))
                        return pk;
                }
            }
            catch { return null; }
        }

        private string generatePkNormal(ApplicationVersion appV, Client client)
        {
            StringBuilder str = new StringBuilder();
            str.Append("LV");
            str.Append(client.Code);
            str.Append(RandomLetterUpper());
            str.Append(appV.Application.Code);
            str.Append(RandomLetterUpper());
            str.Append(VersionToInt(appV.GetVersion()).ToString("000000"));
            str.Append(RandomLetterUpper());
            str.Append(DateTime.Now.ToString("yMMdd"));
            str.Append(RandomLetterUpper());
            var r = RandomNumber(1, 99);
            foreach(var c in TTLL.Models.ProductKey.CryptoWord)
            {
                var indx = Array.IndexOf(TTLL.Models.ProductKey.RandomCryptoSequence, c);
                var newIndx = (indx + r) % TTLL.Models.ProductKey.RandomCryptoSequence.Length;
                if (newIndx < TTLL.Models.ProductKey.RandomCryptoSequence.Length)
                    str.Append(TTLL.Models.ProductKey.RandomCryptoSequence[newIndx]);
                else
                    throw new Exception("error pk");
            }
            str.Append(r.ToString("00"));
            return str.ToString();
        }

        private string generatePkTime(ApplicationVersion appV, Client client, TimeSpan endTime)
        {
            StringBuilder str = new StringBuilder();
            str.Append("LT");
            str.Append(client.Code);
            str.Append(RandomLetterUpper());
            str.Append(appV.Application.Code);
            str.Append(RandomLetterUpper());
            str.Append(VersionToInt(appV.GetVersion()).ToString("000000"));
            str.Append(RandomLetterUpper());
            str.Append(DateTime.Now.ToString("yMMdd"));
            str.Append(RandomLetterUpper());
            var bits = Convert.ToInt32(endTime.TotalHours).ToBoolArray();
            Array.Reverse(bits);
            var bitsG = groupBits(bits);
            var clearBits = new List<bool>();
            var find = false;
            foreach (var g in bitsG)
            {
                if (!find)
                {
                    if (g.Contains(true))
                    {
                        clearBits.AddRange(g);
                        find = true;
                    }
                }
                else
                    clearBits.AddRange(g);
            }
            str.Append(getConfidicBits(clearBits));
            return str.ToString();
        }

        private string generatePkTrial(ApplicationVersion appV, Client client, TimeSpan endTime)
        {
            StringBuilder str = new StringBuilder();
            str.Append("TR");
            str.Append(client.Code);
            str.Append(RandomLetterUpper());
            str.Append(appV.Application.Code);
            str.Append(RandomLetterUpper());
            str.Append(VersionToInt(appV.GetVersion()).ToString("000000"));
            str.Append(RandomLetterUpper());
            str.Append(DateTime.Now.ToString("yMMdd"));
            str.Append(RandomLetterUpper());
            var bits = Convert.ToInt32(endTime.TotalHours).ToBoolArray();
            Array.Reverse(bits);
            var bitsG = groupBits(bits);
            var clearBits = new List<bool>();
            var find = false;
            foreach (var g in bitsG)
            {
                if (!find)
                {
                    if (g.Contains(true))
                    {
                        clearBits.AddRange(g);
                        find = true;
                    }
                }
                else
                    clearBits.AddRange(g);
            }
            str.Append(getConfidicBits(clearBits));
            return str.ToString();
        }

        private List<bool[]> groupBits(bool[] bits)
        {
            List<bool[]> o = new List<bool[]>();
            if (bits.Length % 4 > 0)
                throw new Exception("division error");
            for(var i = 0; i < (bits.Length / 4); i++)
            {
                var indS = i * 4;
                o.Add(new bool[] { bits[indS], bits[indS + 1], bits[indS + 2], bits[indS + 3] });
            }
            return o;
        }

        private string getConfidicBits(List<bool> bits)
        {
            string o = "";
            foreach(var b in bits)
            {
                if (b)
                    o += RandomCifre().ToString();
                else
                    o += RandomLetterUpper();
            }
            return o;
        }

        private string generateCode()
        {
           return RandomCifre().ToString() + RandomCifre().ToString() + RandomCifre().ToString() + RandomCifre().ToString();
        }

        public async Task<string> GenerateApplicationCode()
        {
            var appC = await _dB.Applications.Select(p => p.Code).ToListAsync();
            while (true)
            {
                var tmpC = generateCode();
                if (!appC.Contains(tmpC))
                    return tmpC;
            }
        }

        public async Task<string> GenerateClientCode()
        {
            var cC = await _dB.Clients.Select(p => p.Code).ToListAsync();
            while (true)
            {
                var tmpC = generateCode();
                if (!cC.Contains(tmpC))
                    return tmpC;
            }
        }

        public async Task<bool> ValidateClientCode(string code)
        {
            var cC = await _dB.Clients.Select(p => p.Code).ToListAsync();
            return !cC.Contains(code) && code.Length == 4;
        }

        public async Task<bool> ValidateApplicationCode(string code)
        {
            int i;
            var r = int.TryParse(code, out i);
            if (!r)
                return false;
            var appC = await _dB.Applications.Select(p => p.Code).ToListAsync();
            return !appC.Contains(code) && code.Length == 4;
        }

        public async Task<bool> ValidateApplicationName(string name)
        {
            var appC = await _dB.Applications.Select(p => p.Name.ToLower()).ToListAsync();
            return !appC.Contains(name.ToLower());
        }

        public async Task<bool> ValidateVAT(string vat)
        {
            var cV = await _dB.Clients.Select(c => c.VAT).ToListAsync();
            return !cV.Contains(vat);
        }

        public string GetUsername(string name, string surname)
        {
            return name.ToLower().Replace("'", "").Replace(" ", "") + surname.ToLower().Replace("'", "").Replace(" ", "");
        }
    }
}
