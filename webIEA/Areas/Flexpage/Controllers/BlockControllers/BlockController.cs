using System;
using System.Web;
using System.Web.Mvc;
using ExCSS;
using Flexpage.Abstract;
using FlexPage2.Areas.Flexpage.Infrastructure;

namespace Flexpage.Controllers
{
    [FlexpageAdmin]
    public class BlockController : Controller
    {     
        private readonly IBlockProvider _blockProvider;

        public BlockController(IBlockProvider blockProvider)
        {
            _blockProvider = blockProvider;
        }

        // GET: Flexpage/Block
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Action to change the block visibility
        /// </summary>
        /// <param name="blockID">ID of the block</param>
        /// <returns></returns>
        [HttpPost]
        public bool ToggleBlockVisibility(int blockID)
        {
            try
            {
                return _blockProvider.ToggleBlockVisibility(blockID);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// Action to move the block in any direction
        /// </summary>
        /// <param name="blockID">ID of the block</param>
        /// <param name="blocklistID">ID of the blocklist</param>
        /// <param name="direction">Direction of movement</param>
        /// <returns></returns>
        public string MoveBlock(int blockID, int blocklistID, string direction)
        {
            try
            {
                _blockProvider.MoveBlock(blockID, blocklistID, direction);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return "Ok";
        }

        [HttpPost]
        public bool CutBlock(string blockID, string blocklistID)
        {
            try
            {
                System.Web.HttpContext.Current.Session["FP:CopyBlock"] = "cut;" + blockID + ";" + blocklistID;
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public bool CopyBlock(string blockID, string blocklistID)
        {
            try
            {
                System.Web.HttpContext.Current.Session["FP:CopyBlock"] = "copy;" + blockID + ";" + blocklistID;
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public string PasteBlock(string pasteafterID, string targetBlocklistID)
        {
            try
            {
                if (System.Web.HttpContext.Current.Session["FP:CopyBlock"] == null)
                {
                    return "There are nothing to paste";
                }
                string[] blockToPaste = System.Web.HttpContext.Current.Session["FP:CopyBlock"].ToString().Split(';');
                if (blockToPaste.Length < 3 || (blockToPaste[0] != "cut" && blockToPaste[0] != "copy"))
                    return "Incorrect parameter to paste";

                if (blockToPaste[0] == "cut")
                {
                    _blockProvider.PasteAfterCut(Convert.ToInt32(blockToPaste[1]), Convert.ToInt32(blockToPaste[2]), Convert.ToInt32(targetBlocklistID), Convert.ToInt32(pasteafterID));
                }
                else if (blockToPaste[0] == "copy")
                {

                }
                System.Web.HttpContext.Current.Session.Remove("FP:CopyBlock");
                return string.Empty;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string CheckCSS(string cookieId)
        {
            string text = HttpUtility.UrlDecode(Request.Cookies[cookieId]?.Value);
            var parser = new StylesheetParser();
            var result = parser.Parse(text);
            return result.ToCss();
        }
    }
}