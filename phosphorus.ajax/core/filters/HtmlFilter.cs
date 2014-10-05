/*
 * phosphorus five, copyright 2014 - thomas@magixilluminate.com
 * phosphorus five is licensed as mit, see the enclosed license.txt file for details
 */

using System;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Text;
using phosphorus.ajax.core;

namespace phosphorus.ajax.core.filters
{
    /// <summary>
    /// the http response filter for rendering plain html back to client
    /// </summary>
    public class HtmlFilter : Filter
    {
        /// <summary>
        /// initializes a new instance of the <see cref="phosphorus.ajax.core.filters.HtmlFilter"/> class
        /// </summary>
        /// <param name="manager">the manager this instance is rendering for</param>
        public HtmlFilter (Manager manager)
            : base (manager)
        { }

        /// <summary>
        /// renders the response
        /// </summary>
        /// <returns>the response returned back to client</returns>
        protected override string RenderResponse ()
        {
            TextReader reader = new StreamReader (this, Manager.Page.Response.ContentEncoding);
            string content = reader.ReadToEnd ();
            content = IncludeJavaScriptFiles (content);
            return content;
        }

        /// <summary>
        /// includes the javascript files for this response
        /// </summary>
        /// <returns>the javascript files</returns>
        /// <param name="content">html response content</param>
        private string IncludeJavaScriptFiles (string content)
        {
            if (Manager.JavaScriptFiles.Count == 0)
                return content; // nothing to do here

            // stripping away "</html>" from the end
            string endBuffer = "";
            int idxPosition = content.Length - 1;
            for (; idxPosition >= 0; idxPosition --) {
                endBuffer = content [idxPosition] + endBuffer;
                if (endBuffer.StartsWith ("</html>", StringComparison.InvariantCultureIgnoreCase))
                    break;
            }
            StringBuilder builder = new StringBuilder (content.Substring (0, idxPosition));

            // including javascript files
            foreach (string idxFile in Manager.JavaScriptFiles) {
                builder.Append (string.Format(@"<script type=""text/javascript"" src=""{0}""></script>", idxFile));
            }

            // adding back up again the "</html>" parts
            builder.Append (endBuffer);
            return builder.ToString ();
        }
    }
}

