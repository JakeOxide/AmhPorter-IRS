using Microsoft.AspNetCore.Mvc;

namespace AmhPorterTest.Controllers
{
    public class DocumentController : Controller
    {
        private readonly string _customPath = Path.Combine(Environment.CurrentDirectory, "Resource", "Corpus");
        private readonly string _customPathBackup = Path.Combine(Environment.CurrentDirectory, "Resource", "CorpusBackup");

        public IActionResult Upload() => View("UploadDocuments");

        [HttpPost]
        public IActionResult UploadDocuments(List<IFormFile> files)
        {
            if (files == null || files.Count == 0)
            {
                ViewData["Message"] = "No files selected.";
                return View("UploadDocuments");
            }

            List<string> uploadedFiles = new List<string>();

            foreach (var file in files)
            {
                if (Path.GetExtension(file.FileName).ToLower() != ".txt")
                {
                    ViewData["Message"] = "Only .txt files are allowed.";
                    return View("UploadDocuments");
                }

                try
                {
                    string filePath = Path.Combine(_customPath, file.FileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }
                    uploadedFiles.Add(file.FileName);
                }
                catch (Exception ex)
                {
                    ViewData["Message"] = $"Error uploading {file.FileName}: {ex.Message}";
                    return View("UploadDocuments");
                }
            }


            foreach (var file in files)
            {   
                try
                {
                    string filePath = Path.Combine(_customPathBackup, file.FileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }



            //ViewData["Message"] = $"Successfully uploaded {uploadedFiles.Count()} Documents including: {string.Join(", ", uploadedFiles)}";

            ViewData["Message"] = $"Successfully uploaded {uploadedFiles.Count()} Documents including: " +
            $"{string.Join(", ", uploadedFiles.Take(3))}" +
            $"{(uploadedFiles.Count > 5 ? ", ..." : "")}" +
            $"{(uploadedFiles.Count > 3 ? ", " + string.Join(", ", uploadedFiles.Skip(Math.Max(0, uploadedFiles.Count - 2))) : "")}";

            return View("UploadDocuments");
        }


        /*
        public IActionResult Upload() => View();

        [HttpPost]
        public IActionResult UploadDocuments(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                ViewData["Message"] = "No file selected.";
                return View("Upload");
            }

            if (Path.GetExtension(file.FileName).ToLower() != ".txt")
            {
                ViewData["Message"] = "Only .txt files are allowed.";
                return View("Upload");
            }

            try
            {
                string filePath = Path.Combine(_customPath, file.FileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }

                ViewData["Message"] = $"File '{file.FileName}' uploaded successfully!";
            }
            catch (Exception ex)
            {
                ViewData["Message"] = $"Error uploading file: {ex.Message}";
            }

            try
            {
                string filePath = Path.Combine(_customPathBackup, file.FileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return View("Upload");
        }


        */


    }
}
