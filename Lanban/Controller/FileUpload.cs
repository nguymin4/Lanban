using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Lanban
{
    public class FileUpload
    {
        public string uploadFile(HttpContext _context, Query myQuery, int projectID)
        {
            // Upload file
            var uploadFile = _context.Request.Files[0];
            string path = "/Uploads/Project_" + projectID.ToString() + "/" + uploadFile.FileName;
            var filePath = _context.Server.MapPath(path);
            uploadFile.SaveAs(filePath);

            var param = _context.Request.Params;
            // Create new file object and save info of uploaded file to database
            File file = new File();
            file.Task_ID = Convert.ToInt32(param["taskID"]);
            file.User_ID = Convert.ToInt32(_context.Session["UserID"]);
            file.Name = uploadFile.FileName;
            file.Type = getFileType(param["fileType"], file.Name);
            file.Path = path;
            System.Diagnostics.Debug.WriteLine(file.Type);

            return myQuery.linkTaskFile(file);
        }

        string[] generalType = { "image", "audio", "video" };
        string[] docType = { ".document", ".sheet", ".presentation" };
        string[] zipType = { "rar", "zip", "7z" };

        public string getFileType(string fileType, string fileName)
        {
            if (fileName.Contains("pdf")) return "pdf";

            for (int i = 0; i < docType.Length; i++)
                if (fileType.Contains(docType[i])) return docType[i].Substring(1);

            for (int i = 0; i < zipType.Length; i++)
                if (fileType.Contains(zipType[i])) return "zip";

            for (int i = 0; i < generalType.Length; i++)
                if (fileType.Contains(generalType[i])) return generalType[i];

            if (fileType.Contains("image")) return "image";

            return "general";
        }
    }

    public class File
    {
        public int Task_ID { get; set; }
        public int User_ID { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Path { get; set; }
    }
}