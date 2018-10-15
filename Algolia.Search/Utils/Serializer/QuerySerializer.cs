/*
* Copyright (c) 2018 Algolia
* http://www.algolia.com/
* Based on the first version developed by Christopher Maneu under the same license:
*  https://github.com/cmaneu/algoliasearch-client-csharp
* 
* Permission is hereby granted, free of charge, to any person obtaining a copy
* of this software and associated documentation files (the "Software"), to deal
* in the Software without restriction, including without limitation the rights
* to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
* copies of the Software, and to permit persons to whom the Software is
* furnished to do so, subject to the following conditions:
* 
* The above copyright notice and this permission notice shall be included in
* all copies or substantial portions of the Software.
* 
* THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
* IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
* FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
* AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
* LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
* OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
* THE SOFTWARE.
*/

using Algolia.Search.Models.Query;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Reflection;
using System.Net;

namespace Algolia.Search.Utils.Serializer
{
    /// <summary>
    /// Custom serializer for the query object because it must math specific syntax 
    /// For more informations regarding the syntax 
    /// https://www.algolia.com/doc/rest-api/search/#search-endpoints
    /// https://www.newtonsoft.com/json/help/html/JsonConverterAttributeClass.htm
    /// </summary>
    public class QuerySerializer : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            SearchQuery query = (SearchQuery)value;
            var properties = typeof(SearchQuery).GetTypeInfo().DeclaredProperties
                                .Where(p => p.GetValue(query, null) != null)
                                .Select(p => p.Name.ToCamelCase() + "=" + WebUtility.UrlEncode(p.GetValue(query, null).ToString()));

            string queryString = String.Join("&", properties.ToArray());

            writer.WriteStartObject();
            writer.WritePropertyName("params");
            writer.WriteValue(queryString.ToString());
            writer.WriteEnd();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException("Unnecessary : we don't need to deserialize the Query");
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(SearchQuery);
        }
    }
}