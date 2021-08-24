using IndexerService.Models;
using MoreComplexDataStructures;

namespace IndexerService.Core
{
    public static class IndexingServiceQueue
    {
        private static MinHeap<Document> queue = new MinHeap<Document>();

        public static void Enqueue(Document doc)
        {
            queue.Insert(doc);
        }

        public static Document Pop()
        {
            if (queue.Count > 0)
            {
                return queue.ExtractMin();
            }

            return null;
        }
    }
}