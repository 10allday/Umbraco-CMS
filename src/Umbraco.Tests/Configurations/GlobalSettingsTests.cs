﻿using Moq;
using NUnit.Framework;
using Umbraco.Core.Configuration;
using Umbraco.Core.IO;
using Umbraco.Tests.TestHelpers;

namespace Umbraco.Tests.Configurations
{

    [TestFixture]
    public class GlobalSettingsTests : BaseWebTest
    {
        private string _root;

        public override void SetUp()
        {
            base.SetUp();
            _root = TestHelper.IOHelper.Root;
        }

        public override void TearDown()
        {
            base.TearDown();
            TestHelper.IOHelper.Root = _root;
        }

        [TestCase("~/umbraco", "/", "umbraco")]
        [TestCase("~/umbraco", "/MyVirtualDir", "umbraco")]
        [TestCase("~/customPath", "/MyVirtualDir/", "custompath")]
        [TestCase("~/some-wacky/nestedPath", "/MyVirtualDir", "some-wacky-nestedpath")]
        [TestCase("~/some-wacky/nestedPath", "/MyVirtualDir/NestedVDir/", "some-wacky-nestedpath")]
        public void Umbraco_Mvc_Area(string path, string rootPath, string outcome)
        {

            var globalSettings = SettingsForTests.GenerateMockGlobalSettings();
            var ioHelper = new IOHelper(TestHelper.GetHostingEnvironment(), globalSettings);

            var globalSettingsMock = Mock.Get(globalSettings);
            globalSettingsMock.Setup(x => x.Path).Returns(() => path);

            ioHelper.Root = rootPath;
            Assert.AreEqual(outcome, globalSettings.GetUmbracoMvcAreaNoCache(ioHelper));
        }





    }
}
