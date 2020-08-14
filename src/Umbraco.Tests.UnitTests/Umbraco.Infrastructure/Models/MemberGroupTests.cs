﻿using System;
using System.Diagnostics;
using Newtonsoft.Json;
using NUnit.Framework;
using Umbraco.Core.Models;
using Umbraco.Tests.Common.Builders;
using Umbraco.Tests.Common.Builders.Extensions;

namespace Umbraco.Tests.UnitTests.Umbraco.Infrastructure.Models
{
    [TestFixture]
    public class MemberGroupTests
    {
        private MemberGroupBuilder _builder;

        [SetUp]
        public void SetUp()
        {
            _builder = new MemberGroupBuilder();
        }

        [Test]
        public void Can_Deep_Clone()
        {
            // Arrange
            var group = BuildMemberGroup();

            // Act
            var clone = (MemberGroup)group.DeepClone();

            // Assert
            Assert.AreNotSame(clone, group);
            Assert.AreEqual(clone, group);
            Assert.AreEqual(clone.Id, group.Id);
            Assert.AreEqual(clone.AdditionalData, group.AdditionalData);
            Assert.AreEqual(clone.AdditionalData.Count, group.AdditionalData.Count);
            Assert.AreEqual(clone.CreateDate, group.CreateDate);
            Assert.AreEqual(clone.CreatorId, group.CreatorId);
            Assert.AreEqual(clone.Key, group.Key);
            Assert.AreEqual(clone.UpdateDate, group.UpdateDate);
            Assert.AreEqual(clone.Name, group.Name);

            //This double verifies by reflection
            var allProps = clone.GetType().GetProperties();
            foreach (var propertyInfo in allProps)
                Assert.AreEqual(propertyInfo.GetValue(clone, null), propertyInfo.GetValue(group, null));
        }

        [Test]
        public void Can_Serialize_Without_Error()
        {
            var group = BuildMemberGroup();

            var json = JsonConvert.SerializeObject(group);
            Debug.Print(json);
        }

        private MemberGroup BuildMemberGroup()
        {
            return _builder
                .WithId(6)
                .WithKey(Guid.NewGuid())
                .WithName("Test Group")
                .WithCreatorId(4)
                .WithCreateDate(DateTime.Now)
                .WithUpdateDate(DateTime.Now)
                .AddAdditionalData()
                    .WithKeyValue("test1", 123)
                    .WithKeyValue("test2", "hello")
                    .Done()
                .Build();
        }
    }
}