/*
 * Phosphorus Five, copyright 2014 - 2016, Thomas Hansen, thomas@gaiasoul.com
 * 
 * This file is part of Phosphorus Five.
 *
 * Phosphorus Five is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License version 3, as published by
 * the Free Software Foundation.
 *
 *
 * Phosphorus Five is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with Phosphorus Five.  If not, see <http://www.gnu.org/licenses/>.
 * 
 * If you cannot for some reasons use the GPL license, Phosphorus
 * Five is also commercially available under Quid Pro Quo terms. Check 
 * out our website at http://gaiasoul.com for more details.
 */

using System.IO;
using p5.core;
using p5.io.common;
using p5.exp.exceptions;

namespace p5.io.folder
{
    /// <summary>
    ///     Class to help create one or more folder(s).
    /// </summary>
    public static class Create
    {
        /// <summary>
        ///     Creates one or more folder(s).
        /// </summary>
        /// <param name="context">Application Context</param>
        /// <param name="e">Parameters passed into Active Event</param>
        [ActiveEvent (Name = "create-folder")]
        [ActiveEvent (Name = "p5.io.folder.create")]
        public static void p5_io_folder_create (ApplicationContext context, ActiveEventArgs e)
        {
            ObjectIterator.Iterate (context, e.Args, true, "modify-folder", delegate (string foldername, string fullpath) {
                if (Directory.Exists (fullpath)) {
                    throw new LambdaException (string.Format ("Folder '{0}' exist from before", foldername), e.Args, context);
                } else {
                    Directory.CreateDirectory (fullpath);
                }
            });
        }
    }
}
