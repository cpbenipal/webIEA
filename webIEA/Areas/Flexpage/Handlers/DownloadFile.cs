using Flexpage.Code.Common;
using Flexpage.Domain.Business;
using Pluritech.Shared.Abstract.DTO;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;

namespace FlexPage2.Areas.Flexpage.Handlers
{
    public class HttpStatusWithDescription
    {
        public HttpStatusCode Status;
        public string Description;

        public HttpStatusWithDescription(HttpStatusCode status, string description)
        {
            Status = status;
            Description = description;
        }
    }

    public class Download : IHttpHandler
    {
        protected HttpContext _context;

        private int _saveId = -1;
        private bool _isIdSaved = false;
        protected virtual int _id
        {
            get
            {
                if(!_isIdSaved)
                {
                    if(!Int32.TryParse(_context.Request.QueryString["id"], out _saveId))
                    {
                        _saveId = -1;
                        _isIdSaved = true;
                    }
                }
                return _saveId;
            }
        }

        protected int _websiteId
        {
            get
            {
                int websiteId;
                if(ConfigurationManager.AppSettings["WebsiteID"] == null
                    || !Int32.TryParse(ConfigurationManager.AppSettings["WebsiteID"], out websiteId))
                {
                    throw new ConfigurationErrorsException("Wrong or missing WebsiteID. Please specify correct WebsiteID in AppSettings section of the Web.Config.");
                }
                return websiteId;
            }
        }

        protected virtual bool _scanOtherWebsites
        {
            get
            {
                return !"1".Equals(_context.Request.QueryString["noscan"])
                        && ("1".Equals(_context.Request.QueryString["scan"])
                            || "true".Equals(ConfigurationManager.AppSettings["PWDL:ScanWebsites"])
                            || true.Equals(_context.Application["PWDL:ScanWebsites"]));
            }
        }

        private List<int> _ids = null;
        protected virtual List<int> _multipleIds
        {
            get
            {
                if(String.IsNullOrEmpty(_context.Request.QueryString["ids"]))
                    return null;


                if(_ids != null)
                    return _ids;

                _ids = new List<int>();

                foreach(string s in _context.Request.QueryString["ids"]
                    .Split(",;".ToCharArray(), StringSplitOptions.RemoveEmptyEntries))
                {
                    int id = Int32.Parse(s);
                    if(!_ids.Contains(id))
                        _ids.Add(id);
                }

                return _ids;
            }
        }

        private System.IO.MemoryStream _xoutputStream = null;
        protected System.IO.MemoryStream _outputStream
        {
            get
            {
                if(_xoutputStream == null)
                    _xoutputStream = new System.IO.MemoryStream();
                return _xoutputStream;
            }
        }


        private const string _head = @"
<head>
    <title>Download file</title>
    <meta http-equiv=""Content-Type"" content=""text/html; charset=UTF-8"" /> 
</head>";

        private const string _charset = "utf-8";

        private string getPage(string body)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\">");
            sb.AppendLine("<html xmlns=\"http://www.w3.org/1999/xhtml\" >");
            sb.AppendLine(_head);
            sb.AppendLine("<body>");
            sb.AppendLine(body);
            sb.AppendLine("</body>");
            sb.AppendLine("</html>");
            return sb.ToString();
        }

        protected void renderPage(string body)
        {
            _context.Response.Clear();
            _context.Response.ClearHeaders();
            _context.Response.ContentType = "text/html";
            _context.Response.StatusCode = 200;
            _context.Response.Charset = _charset;
            _context.Response.Write(getPage(body));
            _context.Response.End();

        }

        protected void sendStatus(int status)
        {
            _context.Response.Clear();
            _context.Response.ClearHeaders();
            _context.Response.ContentType = "text/html";
            _context.Response.StatusCode = status;
            _context.Response.End();
        }

        #region IHttpHandler Members

        public bool IsReusable
        {
            get { return true; }
        }

        public void ProcessRequest(HttpContext context)
        {
            try
            {
                _context = context;

                if(_scanOtherWebsites && _multipleIds == null)
                {
                    performScan();
                }
                else
                {
                    HttpStatusCode requestStatus = performRequest();
                    if(requestStatus != HttpStatusCode.OK && requestStatus != HttpStatusCode.Found)
                    {
                        sendStatus((int)requestStatus);
                    }
                }
            }
            finally
            {
                if(_xoutputStream != null)
                    _xoutputStream.Dispose();
            }
        }

        #endregion


        private void performScan()
        {
            // Try current website
            HttpStatusCode requestStatus = performRequest();
            if(requestStatus == HttpStatusCode.OK)
            {
                //Document found - return
                return;
            }

            // Scan other websites
            /*List<Triplet> responses = new List<Triplet>();
            foreach(Website website in _dalc.Websites)
            {
                if(website.ID != _websiteId
                    && website.Url != null && website.Url.Length > 0
                    && website.DownloadPage != null && website.DownloadPage.Length > 0)
                {
                    string url = website.Url;
                    if(!url.EndsWith("/") && !website.DownloadPage.StartsWith("/"))
                        url += "/";
                    url += website.DownloadPage;
                    url = UrlHelper.UpdateUrlQueryParameter(url, "noscan", "1");
                    url = processUrlPattern(url);

                    if(onWebsiteRequesting(website, ref url))
                    {
                        HttpStatusWithDescription status;
                        if(requestUrl(url, true, out status))
                        {
                            return;
                        }
                        else
                        {
                            responses.Add(new Triplet(url, website.Url, status));
                        }
                    }
                }
            }

            if((requestStatus == HttpStatusCode.Unauthorized
                || requestStatus == HttpStatusCode.Found)
                && !_context.Request.IsAuthenticated)
            {
                // Document doesn't available from other sites, 
                // but on the current website it's found but unaccessible.
                // Let's try to authorize a user.
                FormsAuthentication.RedirectToLoginPage();
                _context.Response.End();
                return;
            }

            if(responses.Count == 1 &&
                ((HttpStatusWithDescription)responses[0].Third).Status == HttpStatusCode.Found)
            {
                // Document was found on only one site, and user should authorize - redirect he/she 
                _context.Response.Redirect(responses[0].First.ToString());
                return;
            }

            // Show message that file not found
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(Resources.Strings.Download_Failed, _context.Request.Url.OriginalString);
            sb.AppendLine("<br/>");
            sb.AppendFormat(Resources.Strings.Download_FailedReason,
                (int)requestStatus, requestStatus.ToString());
            sb.AppendLine("<br/>");
            sb.AppendLine(Resources.Strings.Download_SitesSearched);
            sb.AppendLine("<br/>");
            sb.AppendLine("<table cellpadding='2' cellspacing='5' border='0'>");
            sb.AppendLine("<tr>");
            sb.AppendFormat("<td>{0}</td><td>{1}</td><td>{2}</td>\n",
                Resources.Strings.Website, Resources.Strings.Response, Resources.Strings.Description);
            sb.AppendLine("</tr>");
            foreach(Triplet triplet in responses)
            {
                sb.AppendLine("<tr>");
                sb.AppendFormat("<td><a href='{0}'>{1}</a></td><td>{2}</td><td>{3}</td>\n",
                    (string)triplet.First, (string)triplet.Second,
                    (int)((HttpStatusWithDescription)triplet.Third).Status,
                    ((HttpStatusWithDescription)triplet.Third).Description
                );
                sb.AppendLine("</tr>");
            }
            sb.AppendLine("</table>");
            renderPage(sb.ToString());*/
        }

        protected virtual string processUrlPattern(string urlPattern)
        {
            return String.Format(urlPattern, _id);
        }

        /// <summary>
        /// Called before website is requested
        /// </summary>
        /// <param name="website">Website to be requested</param>
        /// <param name="url">Url to be requested</param>
        /// <returns>true if website can be request, false if it must be skipped</returns>
        /*protected virtual bool onWebsiteRequesting(Website website, ref string url)
        {
            return true;
        }*/

        private bool requestUrl(string url, bool acceptOnlyData, out HttpStatusWithDescription status)
        {
            HttpWebRequest request;
            try
            {
                // Create a request
                request = (HttpWebRequest)WebRequest.Create(url);
                // Set 10 minutes timeout
                request.Timeout = 10 * 60 * 1000;
                request.AllowAutoRedirect = false;
                request.Referer = _context.Request.Url.OriginalString;
                request.UserAgent = _context.Request.UserAgent;
            }
            catch(Exception ex)
            {
                status = new HttpStatusWithDescription(
                    HttpStatusCode.InternalServerError,
                    String.Format(Resources.Strings.Download_CantCreateRequest, ex.Message));
                return false;
            }

            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                switch(response.StatusCode)
                {
                    case HttpStatusCode.OK:
                        {
                            if(!acceptOnlyData || response.Headers["Content-Disposition"] != null)
                            {
                                _context.Response.Clear();
                                foreach(string key in response.Headers)
                                {
                                    _context.Response.AppendHeader(key, response.Headers[key]);
                                }

                                //Copy data from one stream to another
                                System.IO.Stream stream = response.GetResponseStream();
                                const int bufferSize = 32768;
                                int bytesRead = 0;
                                byte[] buffer = new byte[bufferSize];
                                while((bytesRead = stream.Read(buffer, 0, bufferSize)) > 0)
                                {
                                    _context.Response.OutputStream.Write(buffer, 0, bytesRead);
                                }
                                stream.Close();
                                _context.Response.End();
                                status = new HttpStatusWithDescription(response.StatusCode,
                                    response.StatusDescription);
                                return true;
                            }
                            else
                            {
                                // This situation means the download page returned some HTML, 
                                // but not the requested document
                                status = new HttpStatusWithDescription(response.StatusCode,
                                    String.Format("<a href='{0}'>{1}</a>",
                                    url,
                                    Resources.Strings.Download_HtmlReturned));
                                return false;
                            }
                        }
                    case HttpStatusCode.Found:
                        {
                            if(acceptOnlyData)
                            {
                                // This status means that document exists, but forms authentications needed
                                status = new HttpStatusWithDescription(response.StatusCode,
                                    String.Format("<a href='{0}{1}'>{2}</a>",
                                    response.ResponseUri.GetLeftPart(UriPartial.Authority),
                                    response.Headers["Location"],
                                    Resources.Strings.Download_AuthenticationRequired));
                                return false;
                            }
                            else
                            {
                                _context.Response.Redirect(response.Headers["Location"]);
                                status = new HttpStatusWithDescription(response.StatusCode,
                                    response.StatusDescription);
                                return true;
                            }
                        }
                }
                status = new HttpStatusWithDescription(
                     response.StatusCode, response.StatusDescription);
            }
            catch(WebException ex)
            {
                if(ex.Status == WebExceptionStatus.ProtocolError)
                {
                    status = new HttpStatusWithDescription(
                        (ex.Response as HttpWebResponse).StatusCode,
                        (ex.Response as HttpWebResponse).StatusDescription);
                }
                else
                {
                    status = new HttpStatusWithDescription(
                       HttpStatusCode.InternalServerError,
                       String.Format(Resources.Strings.Download_ErrorThenProcessing, ex.Message));
                }
            }
            return false;
        }

        private delegate void StreamIsReady(string fileName);

        private HttpStatusCode performRequest()
        {
            string status;
            string fileName;
            if(_multipleIds == null)
                return performRequest(_id, out status, out fileName);

            // Perform request of multiple files adding them into archive
            List<System.IO.MemoryStream> files = new List<System.IO.MemoryStream>();
            try
            {
                /*using(Ionic.Zip.ZipFile zip = new Ionic.Zip.ZipFile())
                {
                    StringBuilder errors = new StringBuilder();
                    foreach(int id in _multipleIds)
                    {
                        if(_xoutputStream != null)
                        {
                            _xoutputStream.Dispose();
                        }
                        _xoutputStream = new System.IO.MemoryStream();
                        _xoutputStream.Position = 0;
                        HttpStatusCode code = performRequest(id, out status, out fileName,
                            filename =>
                            {
                                _xoutputStream.Position = 0;
                                System.IO.MemoryStream file = new System.IO.MemoryStream();
                                files.Add(file);
                                _xoutputStream.WriteTo(file);
                                file.Position = 0;
                                zip.AddEntry(filename, "", file);
                            });
                        if(code != HttpStatusCode.OK)
                            errors.AppendFormat("{0} - {1} - {2}\r\n", id, status, fileName);
                    }
                    if(errors.Length > 0)
                        zip.AddEntry("errors.txt", "", errors.ToString());

                    configureResponse(true, "application/zip", "files.zip");
                    zip.Save(_context.Response.OutputStream);
                }*/
            }
            finally
            {
                files.ForEach(ms => ms.Dispose());
            }

            _context.Response.End();

            return HttpStatusCode.OK;
        }

        private HttpStatusCode performRequest(int id, out string status,
            out string fileName, StreamIsReady streamIsReady = null)
        {
            fileName = "";

            if(!checkId(id))
            {
                status = String.Format(Resources.Strings.Download_ErrorThenProcessing, Resources.Strings.Download_WrongID);
                if(_multipleIds == null)
                {
                    renderPage(status);
                }
                return HttpStatusCode.OK;
            }

            // Check what the file exists and ASP.NET has access to it
            ObjectInfo fileInfo = null;
            try
            {
                //fileInfo = _dalc.FileInfo(id, 0);
            }
            catch(SqlException ex)
            {
                status = ex.Message;
                return processNotFound(ex);
            }

            if(!isUserAuthorised(fileInfo))
            {
                status = String.Format("User not authorised: {0}", getFileName(fileInfo));
                return processNotAuthorised(fileInfo);
            }

            // Everything is fine, send the file

            fileName = getFileName(fileInfo);

            // Replace illegal characters \ / : * ? " < > | with _
            System.Text.RegularExpressions.Regex regex =
                new System.Text.RegularExpressions.Regex(@"[\\/\:\*\?""<>\|]");
            fileName = regex.Replace(fileName, "_");

            // Replace space-like characters with space
            regex = new System.Text.RegularExpressions.Regex("\\s");
            fileName = regex.Replace(fileName, " ");

            // Determine mime type
            bool asAttachment;
            string mime = determineMime(ref fileName, out asAttachment);

            if(_multipleIds == null)
                configureResponse(asAttachment, mime, fileName);

            // download the file
            //const int bufferSize = 32768;
            int sizeRead = 0;

            preWriteData();
            int pos = 0;
            do
            {
                byte[] data = new byte[0];
                //sizeRead = _dalc.FileDownload(id, 0, pos, bufferSize, out data);
                pos += sizeRead;
                if(sizeRead > 0)
                {
                    writeDataToOutputStream(data, sizeRead);
                }
            }
            while(sizeRead > 0);

            processData();

            if(_multipleIds == null)
            {
                writeStreamToResponse();
            }

            if(streamIsReady != null)
            {
                streamIsReady(fileName);
            }

            postWriteData();

            // add file download log record
            AddDownloadLog(fileInfo, _context);

            status = "";
            return HttpStatusCode.OK;
        }

        private void configureResponse(bool asAttachment, string mime, string fileName)
        {
            // Encode filename for the stupid IE
            string encodedFilename;
            if(_context.Request.Browser.Browser == "IE")
            {
                encodedFilename = _context.Server.UrlEncode(fileName).Replace("+", "%20");
            }
            else
            {
                encodedFilename = fileName;
            }

            _context.Response.Clear();
            _context.Response.StatusCode = 200;
            _context.Response.ContentType = mime;
            _context.Response.AppendHeader("Content-Disposition",
                (asAttachment ? "attachment" : "inline") + ";filename=\"" + encodedFilename + "\"");
            _context.Response.BufferOutput = true;
        }

        protected virtual bool checkId(int id)
        {
            return id > 0;
        }

        protected virtual HttpStatusCode processNotFound(SqlException ex)
        {
            return HttpStatusCode.NotFound;
        }

        protected virtual HttpStatusCode processNotAuthorised(ObjectInfo fileInfo)
        {
            return HttpStatusCode.Unauthorized;
        }


        protected virtual void preWriteData()
        {
        }

        protected virtual void processData()
        {
            // nothing to do for simple downloader
        }

        protected virtual void postWriteData()
        {
            _outputStream.Close();
            _outputStream.Dispose();
        }

        protected virtual void writeDataToOutputStream(byte[] data, int size)
        {
            _outputStream.Write(data, 0, size);
        }

        protected virtual void writeStreamToResponse()
        {
            _outputStream.Seek(0, System.IO.SeekOrigin.Begin);
            _outputStream.WriteTo(_context.Response.OutputStream);
        }

        protected virtual string determineMime(ref string fileName, out bool asAttachment)
        {
            string mime;
            mime = DownloadHelper.DetermineMimeTypeByFilename(fileName, true);
            asAttachment = !mime.StartsWith("text/");
            return mime;
        }

        protected virtual string getFileName(ObjectInfo fileInfo)
        {
            var prop = fileInfo.Properties.FirstOrDefault(p => p.Name == "FileShortcut.Name");
            if(prop != null)
                return prop.Value.ToString();
            return "file";
        }

        protected virtual bool isUserAuthorised(ObjectInfo fileInfo)
        {
            return true;
            /*
            // Check what logged user has access to the file's folder
            bool f = _dalc.WebAuthorise(_context.User.Identity.Name, _websiteId, fileInfo.FolderID);

            if(f && !String.IsNullOrEmpty(FileInfoWebHelper.WebRolesAccessFieldName))
            {
                FileInfoWebWrapper fileInfoWebWrapper = new FileInfoWebWrapper(fileInfo);
                f = f && fileInfoWebWrapper.IsAllowedToUser;
            }
            return f;
            */
        }

        /// <summary>
        /// Adds download file log record
        /// </summary>
        public void AddDownloadLog(ObjectInfo fileInfo, HttpContext ctx)
        {
            /*bool addLog;
            if(ConfigurationManager.AppSettings["PW:DownloadLogEnable"] != null
                && bool.TryParse(ConfigurationManager.AppSettings["PW:DownloadLogEnable"], out addLog)
                && addLog)
            {
                string lang = string.Empty;
                FileInfoWebWrapper fileInfoWebWrapper = new FileInfoWebWrapper(fileInfo);
                if(fileInfoWebWrapper != null)
                {
                    lang = fileInfoWebWrapper.FileLanguage;
                }

                List<FolderInfo> folders = _dalc.GetFoldersList(new List<int>(new int[] { fileInfo.FolderID }));

                string fileName = getFileName(fileInfo);
                string userName = ctx.User.Identity.Name;

                UserInfo user = _dalc.UserGetByWebLogin(userName);
                List<Person> persons = user != null ? _dalc.PersonsByID(new List<int>(new int[] { user.ID }), LinkedFields.None) : new List<Person>();

                string fullName = persons.Count > 0 ? persons[0].FullName2 : string.Empty;

                DownloadLogEntry logEntry = new DownloadLogEntry(ctx.Request.UserHostAddress,
                    userName,
                    ctx.Request.Url.AbsoluteUri,
                    ctx.Request.UrlReferrer != null ? ctx.Request.UrlReferrer.AbsoluteUri : string.Empty,
                    DateTime.Now,
                    fileName,
                    fileInfo.ID,
                    lang,
                    fullName,
                    _multipleIds != null,
                    fileInfoWebWrapper.Description,
                    folders.Count > 0 ? folders[0].Name : string.Empty);
                logEntry.Insert();
            }*/
        }
    }

    public class DownloadHelper
    {
        public static string DetermineMimeTypeByFilename(string filename, bool treatXmlAsWord)
        {
            switch(System.IO.Path.GetExtension(filename).ToLower())
            {
                case ".doc":
                    return "application/msword";
                case ".xml":
                    if(treatXmlAsWord)
                        return "application/msword";
                    else
                        return "text/xml";
                case ".pdf":
                    return "application/pdf";
                case ".txt":
                    return "text/plain";
                case ".htm":
                case ".html":
                    return "text/html";
                case ".jpg":
                case ".jpeg":
                    return "image/jpeg";
                case ".gif":
                    return "image/gif";
                case ".png":
                    return "image/png";
                case ".bmp":
                    return "image/bmp";
                case ".tif":
                case ".tiff":
                    return "image/tiff";
                default:
                    return "application/octet-stream";
            }

        }
    }
}