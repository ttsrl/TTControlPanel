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

        public string GenerateProdutKey(Application app, ApplicationVersion appV, Client client)
        {
            try
            {
                var prods = _dB.ProductKeys.Select(p => p.Key).ToList();
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

        public string GenerateResponseCode(ProductKey pk, string hid)
        {
            return Sha256Hash(pk + "-" + hid, 32);
        }

        public string GenerateApplicationCode()
        {
           return RandomCifre().ToString() + RandomCifre().ToString() + RandomCifre().ToString() + RandomCifre().ToString();
        }

        public bool ValidateApplicationCode(string code)
        {
            var appC = _dB.Applications.Select(p => p.Code).ToList();
            return !appC.Contains(code) && code.Length == 4;
        }

        public bool ValidateApplicationName(string name)
        {
            var appC = _dB.Applications.Select(p => p.Name).ToList();
            return !appC.Contains(name);
        }
    }
}
