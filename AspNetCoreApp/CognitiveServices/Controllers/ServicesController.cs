using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.Http.Headers;
using System.IO;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.FileProviders;
using OfficeOpenXml;
using System.Text;
using CognitiveServices.Models;
using System.Data;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CognitiveServices.Controllers
{
    [Route("api/[controller]/[action]")]
    public class ServicesController : Controller
    {
        //SubscribeKey
        const string subscriptionKey = "4790585b64214422abc22a51ec77c6e7";
        //ApiURL
        const string uriBase = "https://eastus.api.cognitive.microsoft.com/face/v1.0/detect";

        private readonly IHostingEnvironment _hostingEnvironment;

        public ServicesController(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        [HttpPost]
        public async Task<ActionResult> GetImageByteArry(List<IFormFile> files)
        {
            try
            {
                if (Request.HasFormContentType)
                {
                    if (files.Count > 0)
                    {
                        byte[] FileContent;
                        using (var memoryStream = new MemoryStream())
                        {
                            await files[0].CopyToAsync(memoryStream);
                            memoryStream.Seek(0, SeekOrigin.Begin);
                            FileContent = new byte[memoryStream.Length];
                            await memoryStream.ReadAsync(FileContent, 0, FileContent.Length);
                        }
                        return Ok(MakeAnalysisRequest(FileContent));
                    }
                    return BadRequest("请至少选择一个文件");
                }
                else
                {
                    return BadRequest("ContentType有误");
                }
            }
            catch (Exception e)
            {
                return BadRequest("出现错误" + e.Message);
            }
        }


        /// <summary>
        /// 上传文件到wwwroot指定文件夹
        /// </summary>
        /// <param name="files"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> FileUpLoad(List<IFormFile> files)
        {
            try
            {

                long size = files.Sum(f => f.Length);
                //wwwroot目录
                string webRoot = _hostingEnvironment.WebRootPath;
                //项目根目录
                string serverPath = _hostingEnvironment.ContentRootPath;
                //合并文件路径
                var folder = Path.Combine(webRoot, @"FaceImage");
                //判断文件夹是否存在，如果不存在就创建
                if (!Directory.Exists(folder))
                {
                    DirectoryInfo directory = Directory.CreateDirectory(folder);
                }
                if (files.Count > 0)
                { //从文件列表中循环
                    foreach (var formFile in files)
                    {
                        if (formFile.Length > 0)
                        {
                            string src = Path.Combine(webRoot, formFile.FileName);
                            var fileName = formFile.FileName;
                            using (var stream = new FileStream(src, FileMode.Create, FileAccess.ReadWrite))
                            {
                                await formFile.CopyToAsync(stream);
                            }
                            //using (FileStream fs = File.Create(src, Convert.ToInt32(formFile.Length)))
                            //{
                            //    formFile.CopyTo(fs);
                            //    fs.Flush();
                            //}
                        }
                    }
                    return Ok(new { count = files.Count, size, folder });
                }
                else
                {
                    return BadRequest("请至少选择一个文件");
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> DownloadFile(string filename)
        {
            //wwwroot目录
            string webRoot = _hostingEnvironment.WebRootPath;
            if (filename == null)
            {
                return NotFound("文件名不能为空");
            }
            else
            {
                DirectoryInfo directory = new DirectoryInfo(webRoot);
                FileInfo[] file = directory.GetFiles(filename);
                if (file.Length > 0)
                {
                    //foreach (var item in file)
                    //{
                    //    var paths = item.FullName;
                    //}
                    var memory = new MemoryStream();
                    string filePath = file[0].FullName;
                    using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                    {
                        await stream.CopyToAsync(memory);
                    }
                    memory.Position = 0;
                    //return File(memory, GetContentType(path), Path.GetFileName(path));
                    return File(memory, "multipart/form-data", file[0].Name);
                }
                return BadRequest("文件不存在");
                //var path = Path.Combine(webRoot, filename);
                //var memory = new MemoryStream();
                //using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read))
                //{
                //    await stream.CopyToAsync(memory);
                //}
                //memory.Position = 0;
                //return File(memory, "multipart/form-data",file[0].Name);
            }

        }

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="FileName"></param>
        /// <returns></returns>
        [HttpDelete]
        public ActionResult DeleteFile(string FileName)
        {
            //wwwroot目录
            string webRoot = _hostingEnvironment.WebRootPath;
            if (FileName == null)
            {
                return NotFound("文件名不能为空");
            }
            else
            {
                DirectoryInfo directory = new DirectoryInfo(webRoot);
                FileInfo[] file = directory.GetFiles(FileName);
                if (file.Length > 0)
                {
                    try
                    {
                        file[0].Delete();
                        return Ok("删除成功");
                    }
                    catch (Exception ex)
                    {
                        return BadRequest("删除失败:" + ex.Message);
                    }
                }
                return BadRequest("该文件不存在或已删除");
            }
        }

        /// <summary>
        /// 使用Computer Vision REST API获取指定图像文件的分析
        /// </summary>
        /// <param name="imageFilePath">The image file.</param>
        static async Task<string> MakeAnalysisRequest(byte[] array)
        {
            try
            {
                HttpClient client = new HttpClient();
                // 请求头
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", subscriptionKey);

                // 选择要返回的详细参数
                string requestParameters = "returnFaceId=true&returnFaceLandmarks=false&returnFaceAttributes=age,gender,headPose,smile,facialHair,glasses,emotion,hair,makeup,occlusion,accessories,blur,exposure,noise";

                // Assemble the URI for the REST API Call.
                string uri = uriBase + "?" + requestParameters;

                HttpResponseMessage response;

                // Request body. Posts a locally stored JPEG image.
                byte[] byteData = array;

                using (ByteArrayContent content = new ByteArrayContent(byteData))
                {
                    // 这个例子中Content-Type是"application/octet-stream".
                    // 请求的Header类型一般是"application/json" and "multipart/form-data".
                    content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

                    // Execute the REST API call.
                    response = await client.PostAsync(uri, content);

                    // Get the JSON response.
                    string contentString = await response.Content.ReadAsStringAsync();
                    return contentString;
                }
            }
            catch (Exception e)
            {

                return e.Message;
            }
        }

        [HttpGet]
        public IActionResult Export()
        {
            string sWebRootFolder = _hostingEnvironment.WebRootPath;
            Random rand = new Random();
            int index = rand.Next(1, 99);
            string sFileName = $"{"Uchiha" + index}.xlsx";
            FileInfo file = new FileInfo(Path.Combine(sWebRootFolder, sFileName));
            using (ExcelPackage package = new ExcelPackage(file))
            {
                // 添加表名
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Student");
                List<Product> list = Product.GetList();
                List<string> attr = Product.GetAttr(new Product());
                Product pro = new Product();
                //添加头
                for (int i = 0; i < attr.Count; i++)
                {
                    worksheet.Cells[1, i + 1].Value = attr[i];
                }
                //添加内容
                for (int i = 0; i < list.Count; i++)
                {
                    worksheet.Cells[i + 2, 1].Value = list[i].ID;
                    worksheet.Cells[i + 2, 2].Value = list[i].Name;
                    worksheet.Cells[i + 2, 3].Value = list[i].Brand;
                    worksheet.Cells[i + 2, 4].Value = list[i].Price;
                }
                package.Save();
            }
            return File(sFileName, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }
        
        [HttpPost]
        public async Task<ActionResult> Import()
        {
            try
            {
                List<Product> list = new List<Product>();
                //获取请求中的第一个文件，并判断是否为空
                IFormFile files = Request.Form.Files[0];                
                if (files.Length > 0)
                {  //创建StringBuilder存储对象
                    StringBuilder sb = new StringBuilder();
                    //创建内存流对象
                    using (var memoryStream = new MemoryStream())
                    {
                        //把文件存到内存流中
                        await files.CopyToAsync(memoryStream);
                        memoryStream.Seek(0, SeekOrigin.Begin);
                        //创建Excel包对象
                        using (ExcelPackage package = new ExcelPackage(memoryStream))
                        {
                            //获取第一张表
                            ExcelWorksheet worksheet = package.Workbook.Worksheets[1];
                            //获取总行数
                            int rowCount = worksheet.Dimension.Rows;
                            //获取总列数
                            int ColCount = worksheet.Dimension.Columns;
                            //是否有行标题
                            bool bHeaderRow = true;
                            //循环遍历
                            for (int row = 1; row <= rowCount; row++)
                            {
                                for (int col = 1; col <= ColCount; col++)
                                {
                                    if (bHeaderRow)
                                    {
                                        sb.Append(worksheet.Cells[row, col].Value.ToString() + "\t");
                                    }
                                    else
                                    {
                                        Product pro = new Product();                                        
                                        sb.Append(worksheet.Cells[row, col].Value.ToString() + "\t");
                                    }
                                }
                                sb.Append(Environment.NewLine);
                            }
                        }
                    }
                    
                    return Ok(sb.ToString());
                }
                else
                {
                    return NotFound("请至少选择一个文件");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        /// <summary>
        /// ImportToList
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> ImportToList()
        {
            try
            {
                List<Product> list = new List<Product>();
                //获取请求中的第一个文件，并判断是否为空
                IFormFile files = Request.Form.Files[0];
                if (files.Length > 0)
                {  //创建StringBuilder存储对象
                    StringBuilder sb = new StringBuilder();
                    //创建内存流对象
                    using (var memoryStream = new MemoryStream())
                    {
                        //把文件存到内存流中
                        await files.CopyToAsync(memoryStream);
                        memoryStream.Seek(0, SeekOrigin.Begin);
                        //创建Excel包对象
                        using (ExcelPackage package = new ExcelPackage(memoryStream))
                        {
                            //获取第一张表
                            ExcelWorksheet worksheet = package.Workbook.Worksheets[1];
                            //获取总行数
                            int rowCount = worksheet.Dimension.Rows;
                            //获取总列数
                            int ColCount = worksheet.Dimension.Columns;
                            //是否有行标题
                            bool bHeaderRow = true;
                            //循环遍历
                            for (int row = 1; row <= rowCount; row++)
                            {
                                //如果有行标题，从第二行开始读
                                if (bHeaderRow)
                                {
                                    if (row > 1)
                                    {                                       
                                        Product pro = new Product();
                                        pro.ID = Convert.ToInt32(worksheet.Cells[row, 1].Value);
                                        pro.Name = worksheet.Cells[row, 2].Value.ToString();
                                        pro.Brand = worksheet.Cells[row, 3].Value.ToString();
                                        pro.Price = Convert.ToDouble(worksheet.Cells[row, 4].Value);
                                        list.Add(pro);
                                    }
                                }
                                //如果没有列标题，从第一行开始读取
                                else
                                {
                                    Product pro = new Product();
                                    pro.ID = Convert.ToInt32(worksheet.Cells[row, 1].Value);
                                    pro.Name = worksheet.Cells[row, 2].Value.ToString();
                                    pro.Brand = worksheet.Cells[row, 3].Value.ToString();
                                    pro.Price = Convert.ToDouble(worksheet.Cells[row, 4].Value);
                                    list.Add(pro);
                                }
                            }
                        }
                    }
                    return Ok(list);
                }
                else
                {
                    return NotFound("请至少选择一个文件");
                }
            }
            catch (Exception ex)
            {
                return BadRequest("错误："+ex.Message);
            }

        }

    }
}
