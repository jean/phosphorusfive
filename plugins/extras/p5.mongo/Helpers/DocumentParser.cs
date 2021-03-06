﻿/*
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

using System.Linq;
using p5.core;
using MongoDB.Bson;

namespace p5.mongo
{
    /// <summary>
    ///     Class allowing parsing of BsonDocuments
    /// </summary>
    public static class DocumentParser
    {
        /*
         * Parses a BsonDocument and returns it to caller in args
         */
        public static void ParseDocument (
            ApplicationContext context, 
            Node docNode, 
            BsonDocument doc,
            string skipID)
        {
            // Looping through each element in BsonDocument
            foreach (var idxEl in doc.Elements.Where (ix => ix.Name != skipID)) {

                // Adding currently iterated element
                var idxNode = docNode.Add (idxEl.Name).LastChild;
                ParseElementValue (context, idxNode, idxEl.Value);
            }
        }

        /*
         * Parses an element's value and returns to caller in elNode
         */
        private static void ParseElementValue (
            ApplicationContext context, 
            Node elNode, 
            BsonValue value)
        {
            // Figuring out type of element
            switch (value.BsonType) {
            case BsonType.Array:
                foreach (var idxValue in value.AsBsonArray) {
                    var childNode = elNode.Add ("").LastChild;
                    ParseElementValue (context, childNode, idxValue);
                }
                break;
            case BsonType.Binary:
                elNode.Value = value.AsByteArray;
                break;
            case BsonType.Boolean:
                elNode.Value = value.AsBoolean;
                break;
            case BsonType.DateTime:
                elNode.Value = value.ToUniversalTime ();
                break;
            case BsonType.Document:
                ParseDocument (context, elNode, value.AsBsonDocument, null);
                break;
            case BsonType.Double:
                elNode.Value = value.AsDouble;
                break;
            case BsonType.Int32:
                elNode.Value = value.AsInt32;
                break;
            case BsonType.Int64:
                elNode.Value = value.AsInt64;
                break;
            case BsonType.JavaScript:
                elNode.Value = value.ToString ();
                break;
            case BsonType.JavaScriptWithScope:
                elNode.Value = value.ToString ();
                break;
            case BsonType.Null:
                elNode.Value = null;
                break;
            case BsonType.ObjectId:
                elNode.Value = value.ToString ();
                break;
            case BsonType.RegularExpression:
                elNode.Value = value.ToString ();
                break;
            case BsonType.String:
                elNode.Value = value.AsString;
                break;
            case BsonType.Symbol:
                elNode.Value = value.ToString ();
                break;
            case BsonType.Timestamp:
                elNode.Value = value.ToString ();
                break;
            case BsonType.Undefined:
                elNode.Value = value.ToString ();
                break;
            }
        }
    }
}

