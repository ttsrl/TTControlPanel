using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TTControlPanel.Models;
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

        public async Task<string> GenerateProdutKey(Application app, ApplicationVersion appV, Client client)
        {
            try
            {
                var prods = await _dB.ProductKeys.Select(p => p.Key).ToListAsync();
                while (true)
                {
                    StringBuilder str = new StringBuilder();
                    str.Append(RandomLetter());
                    str.Append(client.Code);
                    str.Append(RandomLetter());
                    str.Append(app.Code);
                    str.Append(RandomLetter());
                    str.Append(VersionToInt(appV.GetVersion()).ToString("000000"));
                    str.Append(RandomLetter());
                    str.Append(appV.ReleaseDate.ToString("yMMdd"));
                    str.Append(RandomLetter());
                    str.Append(DateTime.Now.ToString("yMMdd"));
                    str.Append(RandomLetter());

                    if (!prods.Contains(str.ToString()))
                        return str.ToString();
                }
            }
            catch { return null; }
        }

        public string GenerateConfirmCode(ProductKey pk, string hid)
        {
            return Sha256Hash(pk + "-" + hid, 32);
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
    }
}
