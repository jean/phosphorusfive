/*
 * phosphorus five, copyright 2014 - thomas@magixilluminate.com
 * phosphorus five is licensed as mitx11, see the enclosed LICENSE file for details
 */

namespace phosphorus.ajax.samples
{
    using System;
    using System.Web;
    using System.Web.UI;
    using phosphorus.ajax.core;
    using pf = phosphorus.ajax.widgets;

    public partial class Literal : AjaxPage
    {
        [WebMethod]
        protected void element_onclick (pf.Literal literal, EventArgs e)
        {
            literal.innerHTML = "this is the updated text";
        }
    }
}

