using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ExcelDataReader;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using TTControlPanel.Filters;
using TTControlPanel.Models;
using TTControlPanel.Models.ViewModel;
using TTControlPanel.Services;

namespace TTControlPanel.Controllers
{
    public class ProductController : Controller
    {

        private readonly Utils _utils;
        private readonly DBContext _db;

        public ProductController(DBContext db, Utils utils)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
            _utils = utils ?? throw new ArgumentNullException(nameof(utils));
        }

        [HttpGet]
        [Authentication]
        public async Task<IActionResult> Index(string orderby, int error = 0)
        {
            var query = _db.Products.Where(d => d.Id > 0);
            var order = "id";
            if (!string.IsNullOrEmpty(orderby))
            {
                if (orderby == "-id")
                {
                    order = "-id";
                    query = query.OrderByDescending(q => q.Id);
                }
                if (orderby == "code")
                {
                    order = "code";
                    query = query.OrderBy(q => q.Code);
                }
                if (orderby == "name")
                {
                    order = "name";
                    query = query.OrderBy(q => q.Name);
                }
            }
            var prods = await query.ToListAsync();
            return View(new IndexProductModel { Error = error, Products = prods, OrderBy = order });
        }

        [HttpPost]
        [Authentication]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Import(IFormFile file)
        {
            var actProds = await _db.Products.ToListAsync();

            FileStream fs;
            List<Product> prods = new List<Product>();

            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            //file load
            try
            {
                if (file == null)
                    return View("Index", new IndexProductModel { Products = actProds, Error = 1 });
                if (file.Length <= 0)
                    return View("Index", new IndexProductModel { Products = actProds, Error = 1 });
                var extension = Path.GetExtension(file.FileName);
                if (!extension.Contains("xls") && !extension.Contains("xlsx"))
                    return View("Index", new IndexProductModel { Products = actProds, Error = 2 });
                var filePath = Path.GetTempFileName();
                fs = new FileStream(filePath, FileMode.Create);
                await file.CopyToAsync(fs);
            }
            catch { return View("Index", new IndexProductModel { Products = actProds, Error = 3 }); }

            //load xls
            try
            {
                using (fs)
                {
                    using (var reader = ExcelReaderFactory.CreateReader(fs))
                    {
                        int indexCode = 0;
                        int indexName = 1;
                        int indexVat = 2;
                        int indexSellPr = 3;
                        int indxAvrPr = 4;
                        int vatd = 22;

                        if (reader.FieldCount < 5)
                            return View("Index", new IndexProductModel { Products = actProds, Error = 4 });

                        while (reader.Read()) //Each row of the file
                        {
                            //salto righe vuote
                            var c0 = reader.GetValue(indexCode);
                            var c1 = reader.GetValue(indexName);
                            var c2 = reader.GetValue(indexVat);
                            var c3 = reader.GetValue(indexSellPr);
                            var c4 = reader.GetValue(indxAvrPr);
                            if (c0 == null || c1 == null || c2 == null || c3 == null || c4 == null)
                                continue;
                            //check validate row
                            if (!validateExcelRow(c0, c1, c2, c3, c4))
                                continue;
                            int v2;
                            double v3, v4;
                            //vat
                            if (string.IsNullOrEmpty(c2.ToString()))
                                v2 = vatd;
                            else
                            {
                                int tmp;
                                var res = int.TryParse(c2.ToString(), out tmp);
                                v2 = (res) ? tmp : vatd;
                            }
                            //sell price
                            if (string.IsNullOrEmpty(c3.ToString()))
                                v3 = 0;
                            else
                            {
                                double tmp;
                                var res = double.TryParse(c3.ToString(), out tmp);
                                v3 = (res) ? tmp : 0;
                            }
                            //aver price
                            if (string.IsNullOrEmpty(c4.ToString()))
                                v4 = 0;
                            else
                            {
                                double tmp;
                                var res = double.TryParse(c4.ToString(), out tmp);
                                v4 = (res) ? tmp : 0;
                            }
                            var prod = new Product { Code = c0.ToString(), Name = c1.ToString(), VAT = v2, SellingPrice = v3, AveragePrice = v4 };
                            prods.Add(prod);
                        }
                    }
                }
            }
            catch { return View("Index", new IndexProductModel { Products = actProds, Error = 4 }); }

            //update database
            var actCodes = actProds.Select(p => p.Code).ToList();
            var actNames = actProds.Select(p => p.Name).ToList();
            foreach (var rProd in prods)
            {
                if (actCodes.Contains(rProd.Code) || actNames.Contains(rProd.Name))
                    continue;
                await _db.AddAsync(rProd);
            }
            await _db.SaveChangesAsync();
            return View("Index", new IndexProductModel { Products = actProds });
        }

        [HttpGet]
        [Authentication]
        public async Task<IActionResult> Delete(int id)
        {
            var prod = await _db.Products.Where(p => p.Id == id).FirstOrDefaultAsync();
            if(prod == null)
                return RedirectToAction("Index");
            _db.Products.Remove(prod);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpGet]
        [Authentication]
        public async Task<IActionResult> Edit(int id)
        {
            var prod = await _db.Products.Where(p => p.Id == id).FirstOrDefaultAsync();
            if (prod == null)
                return RedirectToAction("Index");
            return View(new EditProductGetModel { Product = prod });
        }

        [HttpPost]
        [Authentication]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EditProductPostModel model)
        {
            var prod = await _db.Products.Where(p => p.Id == id).FirstOrDefaultAsync();
            if (prod == null)
                return RedirectToAction("Index");
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(model.Code) || string.IsNullOrEmpty(model.Name))
                    return View(new EditProductGetModel { Error = 1, Product = prod });
                if (string.IsNullOrEmpty(model.VAT))
                    model.VAT = "0";
                if (string.IsNullOrEmpty(model.SellingPrice))
                    model.SellingPrice = "0";
                if (string.IsNullOrEmpty(model.AveragePrice))
                    model.AveragePrice = "0";
                int vat;
                double sp, ap;
                bool ret1 = int.TryParse(model.VAT, out vat);
                bool ret2 = double.TryParse(model.SellingPrice, out sp);
                bool ret3 = double.TryParse(model.AveragePrice, out ap);
                if (!ret1 || !ret2 || !ret3)
                    return View(new EditProductGetModel { Error = 1, Product = prod });
                prod.Code = model.Code;
                prod.Name = model.Name;
                prod.VAT = vat;
                prod.SellingPrice = sp;
                prod.AveragePrice = ap;
                await _db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(new EditProductGetModel { Error = 1, Product = prod });
        }

        [HttpGet]
        [Authentication]
        public IActionResult New()
        {
            return View(new NewProductGetModel { });
        }

        [HttpPost]
        [Authentication]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> New(NewProductPostModel model)
        {
            if (ModelState.IsValid)
            {
               if(string.IsNullOrEmpty(model.Code) || string.IsNullOrEmpty(model.Name))
                    return View(new NewProductGetModel { Error = 1 });
                var prod = await _db.Products.Where(p => p.Code.ToLower() == model.Code.ToLower()).FirstOrDefaultAsync();
                if(prod != null)
                    return View(new NewProductGetModel { Error = 2 });
                if (string.IsNullOrEmpty(model.VAT))
                    model.VAT = "0";
                if (string.IsNullOrEmpty(model.SellingPrice))
                    model.SellingPrice = "0";
                if (string.IsNullOrEmpty(model.AveragePrice))
                    model.AveragePrice = "0";
                int vat;
                double sp, ap;
                bool ret1 = int.TryParse(model.VAT, out vat);
                bool ret2 = double.TryParse(model.SellingPrice, out sp);
                bool ret3 = double.TryParse(model.AveragePrice, out ap);
                if(!ret1 || !ret2 || !ret3)
                    return View(new NewProductGetModel { Error = 1 });
                var newp = new Product() { Code = model.Code, Name = model.Name, VAT = vat, SellingPrice = sp, AveragePrice = ap };
                await _db.Products.AddAsync(newp);
                await _db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(new NewProductGetModel { Error = 1 });
        }

        private bool validateExcelRow(object c0, object c1, object c2, object c3, object c4)
        {
            int v1;
            double v2, v3;
            bool valc2 = (string.IsNullOrEmpty(c2.ToString())) ? true : int.TryParse(c2.ToString(), out v1);
            bool valc3 = (string.IsNullOrEmpty(c3.ToString())) ? true : double.TryParse(c3.ToString(), out v2);
            bool valc4 = (string.IsNullOrEmpty(c4.ToString())) ? true : double.TryParse(c4.ToString(), out v3);
            return (!string.IsNullOrEmpty(c0.ToString()) && !string.IsNullOrEmpty(c1.ToString()) && valc2 && valc3 && valc4);
        }
    }
}
