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

using Algolia.Search.Clients;
using Algolia.Search.Models.Requests;
using Algolia.Search.Models.Responses;
using System.Collections.Generic;

namespace Algolia.Search.Iterators
{
    public class IndexIterator<T> where T : class
    {
        private readonly SearchIndex _index;
        private readonly BrowseIndexQuery _query;

        public IndexIterator(SearchIndex index, BrowseIndexQuery query)
        {
            _index = index;
            _query = query;
        }

        public IEnumerator<T> GetEnumerator()
        {
            while (_query.Cursor != null)
            {
                BrowseIndexResponse<T> result = _index.BrowseFrom<T>(_query);
                _query.Cursor = result.Cursor;

                foreach (var hit in result.Hits)
                {
                    yield return hit;
                }
            }
        }
    }
}