using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using RuriLib.LS;

namespace RuriLib.Blocks
{
    /// <summary>
    /// The block is designed to retrieve cookies from a file by domain
    /// </summary>
    public class BlockCookieContainer : BlockBase
    {
        private string variableName = "";
        /// <summary>The name of the output variable.</summary>
        public string VariableName { get { return variableName; } set { variableName = value; OnPropertyChanged(); } }

        private string domain = "google.com";
        /// <summary>The URL to call, including additional GET query parameters.</summary>
        public string Domain { get { return domain; } set { domain = value; OnPropertyChanged(); } }

        private string inputString = "";
        /// <summary>The input string on which the function will be executed (not always needed).</summary>
        public string InputString { get { return inputString; } set { inputString = value; OnPropertyChanged(); } }

        private bool saveNetscape = false;

        /// <summary> Option to save cookies in Netscape format </summary>
        public bool SaveNetscape { get { return saveNetscape;  } set { saveNetscape = value; OnPropertyChanged(); } }

        /// <summary>
        /// Create block
        /// </summary>
        public BlockCookieContainer()
        {
            Label = "COOKIE CONTAINER";
        }

        public override BlockBase FromLS(string line)
        {
            var input = line.Trim();

            // Parse the label
            if (input.StartsWith("#"))
                Label = LineParser.ParseLabel(ref input);

            Domain = LineParser.ParseLiteral(ref input, "DOMAIN");

            // Try to parse the input string
            if (LineParser.Lookahead(ref input) == TokenType.Literal)
                InputString = LineParser.ParseLiteral(ref input, "IputString");

            // Try to parse the arrow, otherwise just return the block as is with default var name and var / cap choice
            if (LineParser.ParseToken(ref input, TokenType.Arrow, false) == string.Empty)
                return this;

            // Parse the SAVE / NOTSAVE
            try
            {
                var varType = LineParser.ParseToken(ref input, TokenType.Parameter, true);
                if (varType.ToUpper() == "SAVE" || varType.ToUpper() == "NOTSAVE")
                    SaveNetscape = varType.ToUpper() == "SAVE";
            }
            catch { throw new ArgumentException("Invalid or missing variable type"); }

            // Parse the variable
            try { VariableName = LineParser.ParseToken(ref input, TokenType.Literal, true); }
            catch { throw new ArgumentException("Variable name not specified"); }

            return this;

        }
        public override string ToLS(bool indent = true)
        {
            var writer = new BlockWriter(GetType(), indent, Disabled);
            writer
                .Label(Label)
                .Token("COOKIECONTAINER")
                .Literal(Domain);

            writer
                .Literal(InputString, "InputString");
            if (!writer.CheckDefault(VariableName, "VariableName"))
                writer
                .Arrow()
                    .Token(SaveNetscape?"SAVE":"NOTSAVE")
                    .Literal(VariableName);






            return writer.ToString();

        }

        public override void Process(BotData data)
        {
            base.Process(data);

            //Outputs
            StringBuilder builder = new StringBuilder();
            string outputCookiesString = "";

            //Path <COOKIEPATH>
            var cookiepath = ReplaceValues(inputString, data);

            IEnumerable<string> source;
            try
            {
                //Read all lines in cookies .txt file
                source = File.ReadAllLines(cookiepath).Distinct();
            }
            catch (Exception)
            {

                InsertVariable(data, false, "WRONGPATH", variableName);
                return;
            }

            //Keys (Anti Duplicate)
            List <string> cookiesKeys = new List<string>();


            try
            {
                
                foreach (string cooki in source)
                {
                    string[] splited;
                    if (cooki.Contains("\t") && cooki.Contains(".") && cooki.Contains("/") && (splited = cooki.Split('\t'))[0].Contains(Domain) && !cookiesKeys.Contains(splited[5]))
                    {
                        if(SaveNetscape) builder.AppendLine(cooki);
                        cookiesKeys.Add(splited[5]);
                        outputCookiesString += $"{splited[5]}: {splited[6]}\n";
                    }
                }
            }
            catch (Exception)
            {
                //InsertVariable(data, false, "NOTVALIDCOOKIE", variableName);
            }

            InsertVariable(data, false, outputCookiesString, variableName);
            if (SaveNetscape) InsertVariable(data, false, builder.ToString(), "COOKIENETSCAPE");
        }
    }
}
