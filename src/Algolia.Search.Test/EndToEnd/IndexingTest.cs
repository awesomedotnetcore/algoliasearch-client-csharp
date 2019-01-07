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
using Algolia.Search.Models.Common;
using Algolia.Search.Models.Enums;
using Algolia.Search.Models.Rules;
using Algolia.Search.Models.Settings;
using Algolia.Search.Models.Synonyms;
using Algolia.Search.Utils;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Algolia.Search.Test.EndToEnd
{
    [TestFixture]
    [Parallelizable]
    public class IndexingTest
    {
        private SearchIndex _index;
        private string _indexName;

        [OneTimeSetUp]
        public void Init()
        {
            _indexName = TestHelper.GetTestIndexName("indexing");
            _index = BaseTest.SearchClient.InitIndex(_indexName);
        }

        [Test]
        [Parallelizable]
        public async Task IndexOperationsAsyncTest()
        {
            // AddObject with ID
            var objectOne = new AlgoliaStub { ObjectID = "one" };
            var addObject = _index.SaveObjectAsync(objectOne);

            // AddObject without ID
            var objectWoId = new AlgoliaStub();
            var addObjectWoId = _index.SaveObjectAsync(objectWoId, autoGenerateObjectId: true);

            // Save two objects with objectID
            var objectsWithIds = new List<AlgoliaStub>
            {
                new AlgoliaStub { ObjectID = "two" },
                new AlgoliaStub { ObjectID = "three" }
            };
            var addObjects = _index.SaveObjectsAsync(objectsWithIds);

            // Save two objects w/o objectIDs
            var objectsWoId = new List<AlgoliaStub>
            {
                new AlgoliaStub { Property = "addObjectsWoId" },
                new AlgoliaStub { Property = "addObjectsWoId" }
            };
            var addObjectsWoId = _index.SaveObjectsAsync(objectsWoId, autoGenerateObjectId: true);

            // Batch 1000 objects
            List<AlgoliaStub> objectsToBatch = new List<AlgoliaStub>();
            List<string> ids = new List<string>();
            for (int i = 0; i < 1000; i++)
            {
                var id = (i + 1).ToString();
                objectsToBatch.Add(new AlgoliaStub { ObjectID = id, Property = $"Property{id}" });
                ids.Add(id);
            }

            var batch = _index.SaveObjectsAsync(objectsToBatch);

            // Wait for all http call to finish
            var responses = await Task.WhenAll(new[] { addObject, addObjectWoId, addObjects, addObjectsWoId, batch });

            // Wait for Algolia's task to finish (indexing)
            responses.Wait();

            // Six first records
            var generatedID = addObjectWoId.Result.Responses[0].ObjectIDs;
            objectWoId.ObjectID = generatedID.ElementAt(0);

            var generatedIDs = addObjectsWoId.Result.Responses[0].ObjectIDs;
            objectsWoId[0].ObjectID = generatedIDs.ElementAt(0);
            objectsWoId[1].ObjectID = generatedIDs.ElementAt(1);

            var settedIds = new List<string> { "one", "two", "three" };

            var sixFirstRecordsIds = settedIds.Concat(generatedID).Concat(generatedIDs);
            var sixFirstRecords = (await _index.GetObjectsAsync<AlgoliaStub>(sixFirstRecordsIds)).ToList();
            Assert.True(sixFirstRecords.Count() == 6);

            var objectsToCompare = new List<AlgoliaStub> { objectOne }.Concat(objectsWithIds)
                .Concat(new List<AlgoliaStub> { objectWoId })
                .Concat(objectsWoId)
                .ToList();

            // Check retrieved objects againt original content
            for (int i = 0; i < sixFirstRecords.Count; i++)
                Assert.True(TestHelper.AreObjectsEqual(sixFirstRecords[i], objectsToCompare[i]));

            // 1000 records
            var batchResponse = (await _index.GetObjectsAsync<AlgoliaStub>(ids)).ToList();
            Assert.True(batchResponse.Count() == 1000);

            // Check retrieved objects againt original content
            for (int i = 0; i < batchResponse.Count; i++)
                Assert.True(TestHelper.AreObjectsEqual(objectsToBatch[i], batchResponse[i]));

            // Browse all index to assert that we have 1006 objects
            List<AlgoliaStub> objectsBrowsed = new List<AlgoliaStub>();

            foreach (var item in _index.Browse<AlgoliaStub>(new BrowseIndexQuery()))
                objectsBrowsed.Add(item);

            Assert.True(objectsBrowsed.Count() == 1006);

            // Update one object
            var objectToPartialUpdate = objectsToBatch.ElementAt(0);
            objectToPartialUpdate.Property = "PartialUpdated";

            var partialUpdateObject = await _index.PartialUpdateObjectAsync(objectToPartialUpdate);
            partialUpdateObject.Wait();

            var getUpdatedObject = await _index.GetObjectAsync<AlgoliaStub>(objectToPartialUpdate.ObjectID);
            Assert.True(getUpdatedObject.Property.Equals(objectToPartialUpdate.Property));

            // Update two objects
            var objectToPartialUpdate1 = objectsToBatch.ElementAt(1);
            objectToPartialUpdate1.Property = "PartialUpdated1";
            var objectToPartialUpdate2 = objectsToBatch.ElementAt(2);
            objectToPartialUpdate2.Property = "PartialUpdated2";

            var partialUpdateObjects = await _index.PartialUpdateObjectsAsync(new List<AlgoliaStub>
            {
                objectToPartialUpdate1,
                objectToPartialUpdate2
            });

            partialUpdateObjects.Wait();

            var getUpdatedObjects = await _index.GetObjectsAsync<AlgoliaStub>(new List<string>
            {
                objectToPartialUpdate1.ObjectID,
                objectToPartialUpdate2.ObjectID
            });
            Assert.True(getUpdatedObjects.ElementAt(0).Property.Equals(objectToPartialUpdate1.Property));
            Assert.True(getUpdatedObjects.ElementAt(1).Property.Equals(objectToPartialUpdate2.Property));

            // Delete six first objects
            var deleteObjects = await _index.DeleteObjectsAsync(sixFirstRecordsIds);
            deleteObjects.Wait();

            // Assert that the objects were deleted
            List<AlgoliaStub> objectsBrowsedAfterDelete = new List<AlgoliaStub>();

            foreach (var item in _index.Browse<AlgoliaStub>(new BrowseIndexQuery()))
                objectsBrowsedAfterDelete.Add(item);

            Assert.True(objectsBrowsedAfterDelete.Count() == 1000);

            // Delete remaining objects
            var deleteRemainingObjects = await _index.DeleteObjectsAsync(ids);
            deleteRemainingObjects.Wait();

            // Assert that all objects were deleted
            List<AlgoliaStub> objectsAfterFullDelete = new List<AlgoliaStub>();
            foreach (var item in _index.Browse<AlgoliaStub>(new BrowseIndexQuery()))
                objectsAfterFullDelete.Add(item);

            Assert.True(objectsAfterFullDelete.Count() == 0);
        }
    }

    public class AlgoliaStub
    {
        public string ObjectID { get; set; }
        public string Property { get; set; } = "Default";
    }
}