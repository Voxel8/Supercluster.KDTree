using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KDTreeTests
{
    using System.Linq;
    
    using Supercluster.KDTree;

    using static Supercluster.KDTree.Utilities.BinaryTreeNavigation;

    [TestClass]
    public class AccuracyTest
    {

        /// <summary>
        /// Should build the tree displayed in the article:
        /// https://en.wikipedia.org/wiki/K-d_tree
        /// </summary>
        [TestMethod]
        public void WikipediaBuildTests()
        {
            // Should generate the following tree:
            //             7,2
            //              |
            //       +------+-----+
            //      5,4          9,6
            //       |            |
            //   +---+---+     +--+
            //  2,3     4,7   8,1 


            var points = new double[][]
                             {
                                 new double[] { 7, 2 }, new double[] { 5, 4 }, new double[] { 2, 3 },
                                 new double[] { 4, 7 }, new double[] { 9, 6 }, new double[] { 8, 1 }
                             };

            var nodes = new string[] { "Eric", "Is", "A", "Really", "Stubborn", "Ferret" };
            var tree = new KDTree<double, string>(
                2,
                points,
                nodes,
                Utilities.L2Norm_Squared_Double,
                double.MinValue,
                double.MaxValue);

            Assert.AreEqual(tree.InternalPointArray[0], points[0]);
            Assert.AreEqual(tree.InternalPointArray[LeftChildIndex(0)], points[1]);
            Assert.AreEqual(tree.InternalPointArray[LeftChildIndex(LeftChildIndex(0))], points[2]);
            Assert.AreEqual(tree.InternalPointArray[RightChildIndex(LeftChildIndex(0))], points[3]);
            Assert.AreEqual(tree.InternalPointArray[RightChildIndex(0)], points[4]);
            Assert.AreEqual(tree.InternalPointArray[LeftChildIndex(RightChildIndex(0))], points[5]);
        }



        /// <summary>
        /// Should build the tree displayed in the article:
        /// https://en.wikipedia.org/wiki/K-d_tree
        /// </summary>
        [TestMethod]
        public void NodeNavigatorTests()
        {
            // Should generate the following tree:
            //             7,2
            //              |
            //       +------+-----+
            //      5,4          9,6
            //       |            |
            //   +---+---+     +--+
            //  2,3     4,7   8,1 


            var points = new double[][]
                             {
                                 new double[] { 7, 2 }, new double[] { 5, 4 }, new double[] { 2, 3 },
                                 new double[] { 4, 7 }, new double[] { 9, 6 }, new double[] { 8, 1 }
                             };

            var nodes = new string[] { "Eric", "Is", "A", "Really", "Stubborn", "Ferret" };

            var tree = new KDTree<double, string>(2, points, nodes, Utilities.L2Norm_Squared_Double);

            var nav = tree.Navigator;

            Assert.AreEqual(nav.Point, points[0]);
            Assert.AreEqual(nav.Left.Point, points[1]);
            Assert.AreEqual(nav.Left.Left.Point, points[2]);
            Assert.AreEqual(nav.Left.Right.Point, points[3]);
            Assert.AreEqual(nav.Right.Point, points[4]);
            Assert.AreEqual(nav.Right.Left.Point, points[5]);
            
            Assert.AreEqual(nav.Node, nodes[0]);
            Assert.AreEqual(nav.Left.Node, nodes[1]);
            Assert.AreEqual(nav.Left.Left.Node, nodes[2]);
            Assert.AreEqual(nav.Left.Right.Node, nodes[3]);
            Assert.AreEqual(nav.Right.Node, nodes[4]);
            Assert.AreEqual(nav.Right.Left.Node, nodes[5]);
        }




        [TestMethod]
        public void FindNearestNeighborTest()
        {
            var dataSize = 10000;
            var testDataSize = 100;
            var range = 1000;

            var treePoints = Utilities.GenerateDoubles(dataSize, range);
            var treeNodes = Utilities.GenerateDoubles(dataSize, range).Select(d => d.ToString()).ToArray();
            var testData = Utilities.GenerateDoubles(testDataSize, range);


            var tree = new KDTree<double, string>(2, treePoints, treeNodes, Utilities.L2Norm_Squared_Double);

            for (int i = 0; i < testDataSize; i++)
            {
                var treeNearest = tree.NearestNeighbors(testData[i], 1);
                var linearNearest = Utilities.LinearSearch(treePoints, treeNodes, testData[i], Utilities.L2Norm_Squared_Double);

                Assert.AreEqual(Utilities.L2Norm_Squared_Double(testData[i], linearNearest.Item1), Utilities.L2Norm_Squared_Double(testData[i], treeNearest[0].Item1));

                // TODO: wrote linear search for both node and point arrays
                Assert.AreEqual(treeNearest[0].Item2, linearNearest.Item2);
            }
        }

        [TestMethod]
        public void RadialSearchTest()
        {
            var dataSize = 10000;
            var testDataSize = 100;
            var range = 1000;
            var radius = 100;

            var treeData = Utilities.GenerateDoubles(dataSize, range);
            var treeNodes = Utilities.GenerateDoubles(dataSize, range).Select(d => d.ToString()).ToArray();
            var testData = Utilities.GenerateDoubles(testDataSize, range);
            var tree = new KDTree<double, string>(2, treeData, treeNodes, Utilities.L2Norm_Squared_Double);

            for (int i = 0; i < testDataSize; i++)
            {
                var treeRadial = tree.RadialSearch(testData[i], radius);
                var linearRadial = Utilities.LinearRadialSearch(
                    treeData,
                    treeNodes,
                    testData[i],
                    Utilities.L2Norm_Squared_Double,
                    radius);

                for (int j = 0; j < treeRadial.Length; j++)
                {
                    Assert.AreEqual(treeRadial[j].Item1, linearRadial[j].Item1);
                    Assert.AreEqual(treeRadial[j].Item2, linearRadial[j].Item2);
                }


            }
        }

        [TestMethod]
        public void SkipTest() {
            var points = new double[][]
                            {
                                 new double[] { 7, 2 }, new double[] { 5, 4 }, new double[] { 2, 3 },
                                 new double[] { 4, 7 }, new double[] { 9, 6 }, new double[] { 8, 1 }
                            };

            var nodes = new string[] { "Eric", "Is", "A", "Really", "Stubborn", "Ferret" };

            var tree = new KDTree<double, string>(2, points, nodes, Utilities.L2Norm_Squared_Double);

            var neighbors = tree.NearestNeighbors(points[0], 1, (a, b) => b == "Eric" || b == "Ferret");

            Assert.AreEqual(1, neighbors.Length);
            Assert.AreEqual("Is", neighbors[0].Item2);
        }
    }
}
