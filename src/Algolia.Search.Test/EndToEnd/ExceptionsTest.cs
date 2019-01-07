/*
* Copyright (c) 2018 Algolia
* http://www.algolia.com/
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
using NUnit.Framework;
using System;

namespace Algolia.Search.Test.EndToEnd
{
    [TestFixture]
    [Parallelizable]
    public class ExceptionsTests
    {
        private SearchIndex _index;
        private string _indexName;

        [OneTimeSetUp]
        public void Init()
        {
            _indexName = TestHelper.GetTestIndexName("exceptions");
            _index = BaseTest.SearchClient.InitIndex(_indexName);
        }

        [Test]
        [Parallelizable]
        public void InitClientsWithoutCredentials()
        {
            Assert.Throws<ArgumentNullException>(() => new SearchClient("", ""));
            Assert.Throws<ArgumentNullException>(() => new AnalyticsClient("", ""));
            Assert.Throws<ArgumentNullException>(() => new InsightsClient("", ""));

            Assert.Throws<ArgumentNullException>(() => new SearchClient("AppID", ""));
            Assert.Throws<ArgumentNullException>(() => new AnalyticsClient("AppID", ""));
            Assert.Throws<ArgumentNullException>(() => new InsightsClient("AppID", ""));
        }

        [Test]
        [Parallelizable]
        public void InitClientsWithoutConfig()
        {
            Assert.Throws<ArgumentNullException>(() => new SearchClient(null));
            Assert.Throws<ArgumentNullException>(() => new AnalyticsClient(null));
            Assert.Throws<ArgumentNullException>(() => new InsightsClient(null));
        }

        [Test]
        [Parallelizable]
        public void InitClientsWithoutHttpClient()
        {
            var config = new AlgoliaConfig("AppID", "APIKey");
            Assert.Throws<ArgumentNullException>(() => new SearchClient(config, null));

            var analyticsConfig = new AnalyticsConfig("AppID", "APIKey");
            Assert.Throws<ArgumentNullException>(() => new AnalyticsClient(analyticsConfig, null));

            var insightConfig = new InsightsConfig("AppID", "APIKey");
            Assert.Throws<ArgumentNullException>(() => new InsightsClient(insightConfig, null));
        }

        [Test]
        [Parallelizable]
        public void Synonyms()
        {
            Assert.Throws<ArgumentNullException>(() => _index.GetSynonym(null));
            Assert.Throws<ArgumentNullException>(() => _index.SaveSynonym(null));
            Assert.Throws<ArgumentNullException>(() => _index.SaveSynonyms(null));
            Assert.Throws<ArgumentNullException>(() => _index.DeleteSynonym(null));
        }

        [Test]
        [Parallelizable]
        public void Rules()
        {
            Assert.Throws<ArgumentNullException>(() => _index.GetRule(null));
            Assert.Throws<ArgumentNullException>(() => _index.SaveRule(null));
            Assert.Throws<ArgumentNullException>(() => _index.SaveRules(null));
            Assert.Throws<ArgumentNullException>(() => _index.DeleteRule(null));
        }

        [Test]
        [Parallelizable]
        public void Objects()
        {
            Assert.Throws<ArgumentNullException>(() => _index.SetSettings(null));
            Assert.Throws<ArgumentNullException>(() => _index.GetObject<Object>(null));
            Assert.Throws<ArgumentNullException>(() => _index.GetObjects<Object>(null));
            Assert.Throws<ArgumentNullException>(() => _index.Browse<Object>(null));
            Assert.Throws<ArgumentNullException>(() => _index.BrowseFrom<Object>(null));
            Assert.Throws<ArgumentNullException>(() => _index.PartialUpdateObject<Object>(null));
            Assert.Throws<ArgumentNullException>(() => _index.SaveObject<Object>(null));
            Assert.Throws<ArgumentNullException>(() => _index.SaveObjects<Object>(null));
            Assert.Throws<ArgumentNullException>(() => _index.ReplaceAllObjects<Object>(null));
            Assert.Throws<ArgumentNullException>(() => _index.Batch<Object>(operations: null));
            Assert.Throws<ArgumentNullException>(() => _index.Batch<Object>(request: null));
            Assert.Throws<ArgumentNullException>(() => _index.DeleteObject(null));
            Assert.Throws<ArgumentNullException>(() => _index.DeleteBy(null));
            Assert.Throws<ArgumentNullException>(() => _index.SearchForFacetValue(null));
            Assert.Throws<ArgumentNullException>(() => _index.Search<Object>(null));
            Assert.Throws<ArgumentNullException>(() => _index.CopyTo(""));
            Assert.Throws<ArgumentNullException>(() => _index.MoveFrom(""));
        }
    }
}