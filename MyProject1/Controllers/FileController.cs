using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using MyProject1.FileEntity;
using MyProject1.FileEntitys;
using Newtonsoft.Json;

namespace MyProject1.FileControllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("any")]
    public class FileController : ControllerBase
    {
        [HttpPost("getallfile")]
        public string GetAdress(string adress)
        {
            // Console.WriteLine(adress);
            FileEnity file = new FileEnity(adress);
            string json = JsonConvert.SerializeObject(file.displayDirectoryFiles());
            return json;
        }

        [HttpPost("gettext")]
        //[Route("Post")]
        public string GetText(string path)
        {
            FileNumber fileNumber = new FileNumber(@path);
            string json = JsonConvert.SerializeObject(fileNumber);

            return json;

        }


    }
}