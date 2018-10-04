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

using Algolia.Search.Http;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Algolia.Search.Transport
{
    public interface IRequesterWrapper
    {
        /// <summary>
        /// Execute the request with the specified TData class and will return the TResult
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <typeparam name="TData"></typeparam>
        /// <param name="method"></param>
        /// <param name="uri"></param>
        /// <param name="data"></param>
        /// <param name="requestOptions"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task<TResult> ExecuteRequestAsync<TResult, TData>(HttpMethod method, string uri, TData data = default(TData),
            RequestOption requestOptions = null, CancellationToken ct = default(CancellationToken))
            where TResult : class
            where TData : class;

        /// <summary>
        /// Execute the request (more likely request with no body like GET or Delete)
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="method"></param>
        /// <param name="uri"></param>
        /// <param name="requestOptions"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task<TResult> ExecuteRequestAsync<TResult>(HttpMethod method, string uri, RequestOption requestOptions = null,
            CancellationToken ct = default(CancellationToken)) 
            where TResult : class;
    }
}