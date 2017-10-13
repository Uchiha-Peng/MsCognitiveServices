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
    }
}
