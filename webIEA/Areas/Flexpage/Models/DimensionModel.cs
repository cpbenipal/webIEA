using System.Text;
using System.Linq;
using Flexpage.Domain.Enum;

namespace Flexpage.Models
{
    public class DimensionModel
    {
        public SizeType Type { get; set; } = SizeType.Auto;
        public int CustomValue { get; set; }
        public SizeUnitType CustomValueUnit { get; set; } = SizeUnitType.Pixel;

        public string CreateSizeString()
        {
            if (Type == SizeType.Auto)
                return "auto";
            if (Type == SizeType.Full)
                return "100%";
            string indentString = CustomValue.ToString();

            switch (CustomValueUnit)
            {
                case SizeUnitType.EM:
                    indentString += "em";
                    break;
                case SizeUnitType.Percentage:
                    indentString += "%";
                    break;
                case SizeUnitType.Pixel:
                    indentString += "px";
                    break;
                case SizeUnitType.REM:
                    indentString += "rem";
                    break;
            }

            return indentString;
        }


        private static string GetNumber(string text, ref int i)
        {
            StringBuilder sb = new StringBuilder();
            while (i < text.Length)
            {
                char c = text[i];
                if ("0123456789.".Contains(c))
                    sb.Append(c);
                else
                    break;
                i++;
            }
            return sb.ToString();
        }



        public static implicit operator DimensionModel(string text)
        {
            DimensionModel m = new DimensionModel();
            int i = 0;
            string txt = text.ToLower();
            string n = GetNumber(txt, ref i);
            int v;
            if (txt == "auto")
                m.Type = SizeType.Auto;
            else
            if (txt == "full")
                m.Type = SizeType.Full;
            else
            if (int.TryParse(n, out v))
            {
                m.Type = SizeType.Custom;
                m.CustomValue = v;
                string s = text.Substring(i).ToLower();
                switch (s)
                {
                    case "px":
                        m.CustomValueUnit = SizeUnitType.Pixel;
                        break;
                    case "%":
                        m.CustomValueUnit = SizeUnitType.Percentage;
                        break;
                    case "em":
                        m.CustomValueUnit = SizeUnitType.EM;
                        break;
                    case "rem":
                        m.CustomValueUnit = SizeUnitType.REM;
                        break;
                }
            }
            else
            {
                m.CustomValueUnit = SizeUnitType.Pixel;
                m.CustomValue = 100;
            }
            return m;
        }
    }
}