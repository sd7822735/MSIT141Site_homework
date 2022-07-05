using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MSIT141Site.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MSIT141Site.Controllers
{
    public class ApiController : Controller
    {
        private readonly DemoContext _context;
        private readonly IWebHostEnvironment _host;


        public ApiController(DemoContext conetxt,IWebHostEnvironment hostEnvironment)
        {
            _context = conetxt;
            _host = hostEnvironment;
        }

        public IActionResult Index(User user)
        {
            //System.Threading.Thread.Sleep(5000);
            if (string.IsNullOrEmpty(user.name))
            {
                user.name = "無名氏";
            }
            return Content($"{user.name}，您好，您的年齡是{user.age} 歲，" +
                $"信箱是 : {user.email}", "text/plain",System.Text.Encoding.UTF8);
        }
        
        public IActionResult CheckName(string name)
        {
            var datas = _context.Members.Any(m => m.Name == name);

            return Content(datas.ToString(), "text/plain");
            
        }
        public IActionResult Register(Member member,IFormFile file)
        {
            //檔案上傳要有實際路徑
            //C:\Users\Student\Documents\Ajax\MSIT141Site\wwwroot\uploads
            /*string path = _host.ContentRootPath;*/ //會取得專案資料夾的實際路徑
            string path = Path.Combine(_host.WebRootPath, "uploads", file.FileName);
            using(var fileStream=new FileStream(path, FileMode.Create))
            {
                file.CopyTo(fileStream);
            }
            byte[] imgByte = null;
            using(var memoryStream=new MemoryStream())
            {
                file.CopyTo(memoryStream);
                imgByte = memoryStream.ToArray();
            }
            member.FileName = file.FileName;
            member.FileData = imgByte;
            _context.Members.Add(member);
            _context.SaveChanges();
            string info = $"{file.FileName}  -  {file.ContentType}  -  {file.Length}";
            return Content(info, "text/plain", System.Text.Encoding.UTF8);
        }


        public IActionResult City()
        {
            var cities = _context.Addresses.Select(a => a.City).Distinct();
            return Json(cities);
        }
        public IActionResult Districts(string city)
        {
            var districts = _context.Addresses.Where(a => a.City == city).Select(a => a.SiteId).Distinct();
            return Json(districts);
        }
        public IActionResult Roads(string district)
        {
            var roads = _context.Addresses.Where(a => a.SiteId == district).Select(a => a.Road);
            return Json(roads);
        }

        public IActionResult GetImageBytes(int id=1)
        {
            Member member = _context.Members.Find(id);
            byte[] img = member.FileData;
            return File(img,"image/jpeg");
        }
        public IActionResult Members()
        {
            return Json(_context.Members);
        }
    }
}
