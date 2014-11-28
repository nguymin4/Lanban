using Lanban.Model;
using System;
using System.IO;
using System.Web;

namespace Lanban
{
    public class FileManager
    {

        public string uploadFile(HttpContext _context, AccessLayer.FileAccess myAccess, int projectID)
        {
            // Upload file
            var uploadFile = _context.Request.Files[0];
            string name = getFileName(uploadFile.FileName);
            string path = "/Uploads/Project_" + projectID.ToString() + "/" + name;
            var filePath = _context.Server.MapPath(path);
            
            try { uploadFile.SaveAs(filePath); }
            catch (Exception) { }

            var param = _context.Request.Params;
            // Create new file object and save info of uploaded file to database
            FileModel file = new FileModel();
            file.Task_ID = Convert.ToInt32(param["taskID"]);
            file.User_ID = Convert.ToInt32(_context.Session["UserID"]);
            file.Name = name;
            file.Type = getFileType(param["fileType"], name);
            file.Path = path;

            return myAccess.linkTaskFile(file);
        }

        public void deleteFile(HttpContext _context, AccessLayer.FileAccess myAccess)
        {
            int fileID = Convert.ToInt32(_context.Request.Params["fileID"]);
            string path = myAccess.getFilePath(fileID);
            try
            {
                System.IO.File.Delete(_context.Server.MapPath(path));
                myAccess.deleteTaskFile(fileID, Convert.ToInt32(_context.Session["userID"]));
            }
            catch (Exception) { };
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

        // Get real name in case of Internet Explorer
        public string getFileName(string name)
        {
            var index = name.LastIndexOf("\\");
            if (index != -1) return name.Substring(index + 1);
            return name;
        }

        // Upload screenshot of a project
        public void uploadScreenshot(HttpContext _context, int projectID)
        {
            // Upload screenshot
            string screenshot = _context.Request.Params["screenshot"].ToString();
            screenshot.Trim('\0');
            string path = "/Uploads/Project_" + projectID.ToString() + "/screenshot.jpg";
            var filePath = _context.Server.MapPath(path);

            // Delete old one
            System.IO.File.Delete(filePath);

            // Create new one

            using (FileStream fs = new FileStream(filePath, FileMode.Create))
            {
                try
                {
                    using (BinaryWriter bw = new BinaryWriter(fs))
                    {
                        byte[] data = Convert.FromBase64String(screenshot);
                        bw.Write(data);
                        bw.Close();
                    }
                }
                catch (Exception)
                {
                    string path2 = _context.Server.MapPath("~/images/screenshot.jpg");
                    System.IO.File.Copy(path2, screenshot);
                }
            }
        }

        // Create project folder and copy default screen shot
        public void createProjectFolder(HttpContext _context, string projectID)
        {
            // Create folder 
            string newFolder = _context.Server.MapPath("~/Uploads/Project_" + projectID);
            Directory.CreateDirectory(newFolder);

            // Copy screenshot
            string path2 = _context.Server.MapPath("~/images/screenshot.jpg");
            System.IO.File.Copy(path2, newFolder + "/screenshot.jpg");
        }

        public void deleteProjectFolder(HttpContext _context, int projectID)
        {
            string path = _context.Server.MapPath("~/Uploads/Project_" + projectID.ToString());
            Directory.Delete(path, true);
        }
    }
}