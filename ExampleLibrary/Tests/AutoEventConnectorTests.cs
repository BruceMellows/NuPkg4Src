// There is no copyright, you can use and abuse this source without limit.
// There is no warranty, you are responsible for the consequences of your use of this source.
// There is no burden, you do not need to acknowledge this source in your use of this source.

namespace ExampleLibrary
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class AutoEventConnectorTests
    {
        public AutoEventConnectorTests()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }

            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes

        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //

        #endregion Additional test attributes

        [TestMethod]
        public void AutoEventConnector_TestConnectDisconnect()
        {
            var testEventPublisher = new TestEventPublisher();
            var testEventSubscriber = new TestEventSubscriber();

            testEventSubscriber.ConnectTo(testEventPublisher);

            testEventPublisher.RaiseTestEvent1();
            testEventPublisher.RaiseTestEvent2();
            Assert.AreEqual(0, testEventSubscriber.HandleTestEvent1Count);
            Assert.AreEqual(1, testEventSubscriber.HandleTestEventPublisherTestEvent1Count);

            testEventSubscriber.DisconnectFrom(testEventPublisher);

            testEventPublisher.RaiseTestEvent1();
            testEventPublisher.RaiseTestEvent2();
            Assert.AreEqual(0, testEventSubscriber.HandleTestEvent1Count);
            Assert.AreEqual(1, testEventSubscriber.HandleTestEventPublisherTestEvent1Count);
        }

        [TestMethod]
        public void AutoEventConnector_TestCache()
        {
            new TestEventSubscriber().ConnectTo(new TestEventPublisher());
            Assert.AreEqual(1, System.AutoEventConnector.CacheCount);

            new TestEventSubscriber().ConnectTo(new TestEventPublisher());
            Assert.AreEqual(1, System.AutoEventConnector.CacheCount);
        }

        private sealed class TestEventPublisher
        {
            public event EventHandler TestEvent1;

            public event EventHandler TestEvent2;

            public void RaiseTestEvent1()
            {
                var handler = this.TestEvent1;
                if (handler != null)
                {
                    handler(this, EventArgs.Empty);
                }
            }

            public void RaiseTestEvent2()
            {
                var handler = this.TestEvent2;
                if (handler != null)
                {
                    handler(this, EventArgs.Empty);
                }
            }
        }

        private sealed class TestEventSubscriber
        {
            public int HandleTestEvent1Count { get; private set; }

            public int HandleTestEventPublisherTestEvent1Count { get; private set; }

            private void HandleTestEvent1(object sender, EventArgs args)
            {
                ++this.HandleTestEvent1Count;
            }

            private void HandleTestEventPublisherTestEvent1(object sender, EventArgs args)
            {
                ++this.HandleTestEventPublisherTestEvent1Count;
            }
        }
    }
}