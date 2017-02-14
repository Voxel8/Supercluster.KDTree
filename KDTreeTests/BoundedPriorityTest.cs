using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KDTreeTests
{
    using Supercluster.KDTree;

    [TestClass]
    public class BoundedPriorityTest
    {
        [TestMethod]
        public void InsertTest()
        {
            var bp = new BoundedPriorityList<int, double>(3, true)
                         {
                             { 34, 98744.90383 },
                             { 23, 67.39030 },
                             { 2, 2 },
                             { 89, 3 }
                         };


            Assert.AreEqual(bp[0], 2);
            Assert.AreEqual(bp[1], 89);
            Assert.AreEqual(bp[2], 23);
        }
    }
}
