using NUnit.Framework;
using System.Reflection;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace aggregator_server_test.Language
{
    // These tests illustrate why LiteDB had trouble in certain cases on some of my entities. When inheriting, the setters for properties in the base class must
    // be protected, not private, in order for them to show up in reflection on derived classes
    [TestFixture]
    class TestPrivateMemberReflection
    {
        public class Base
        {
#pragma warning disable 169, 649
            public int BasePublicField;

            protected int BaseProtectedField;

            private int BasePrivateField;
#pragma warning restore 169, 649

            public int BasePublicProperty { get; set; }

            public int BasePropertyPrivateSetter { get; private set; }

            public int BasePropertyProtectedSetter { get; protected set; }

            protected int BaseProtectedProperty { get; set; }

            private int BasePrivateProperty { get; set; }

            public void BasePublicMethod() { }

            protected void BaseProtectedMethod() { }

            private void BasePrivateMethod() { }
        }

        public class Derived : Base
        {
#pragma warning disable 169, 649
            public int DerivedPublicField;

            protected int DerivedProtectedField;

            private int DerivedPrivateField;
#pragma warning restore 169, 649

            public int DerivedPublicProperty { get; set; }

            public int DerivedPropertyPrivateSetter { get; private set; }

            public int DerivedPropertyProtectedSetter { get; protected set; }

            protected int DerivedProtectedProperty { get; set; }

            private int DerivedPrivateProperty { get; set; }

            public void DerivedPublicMethod() { }

            protected void DerivedProtectedMethod() { }

            private void DerivedPrivateMethod() { }
        }

        [Test]
        public void BaseFields()
        {
            var fields = typeof(Base).GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            Assert.AreEqual(8, fields.Length);  // three defined fields, plus backing fields for each of the 5 properties

            Assert.IsTrue(fields.Any(f => f.Name == "BasePublicField"));
            Assert.IsTrue(fields.Any(f => f.Name == "BaseProtectedField"));
            Assert.IsTrue(fields.Any(f => f.Name == "BasePrivateField"));
        }

        [Test]
        public void BaseMethods()
        {
            var methods = typeof(Base).GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            Assert.AreEqual(19, methods.Length);    // 10 for property setters/getters, 3 defined in class, 6 inherited from Object

            Assert.IsTrue(methods.Any(f => f.Name == "BasePublicMethod"));
            Assert.IsTrue(methods.Any(f => f.Name == "BaseProtectedMethod"));
            Assert.IsTrue(methods.Any(f => f.Name == "BasePrivateMethod"));
            Assert.IsTrue(methods.Any(f => f.Name == "get_BasePublicProperty"));
            Assert.IsTrue(methods.Any(f => f.Name == "set_BasePublicProperty"));
            Assert.IsTrue(methods.Any(f => f.Name == "get_BaseProtectedProperty"));
            Assert.IsTrue(methods.Any(f => f.Name == "set_BaseProtectedProperty"));
            Assert.IsTrue(methods.Any(f => f.Name == "get_BasePrivateProperty"));
            Assert.IsTrue(methods.Any(f => f.Name == "set_BasePrivateProperty"));
            Assert.IsTrue(methods.Any(f => f.Name == "get_BasePropertyPrivateSetter"));
            Assert.IsTrue(methods.Any(f => f.Name == "set_BasePropertyPrivateSetter"));
            Assert.IsTrue(methods.Any(f => f.Name == "get_BasePropertyProtectedSetter"));
            Assert.IsTrue(methods.Any(f => f.Name == "set_BasePropertyProtectedSetter"));
        }

        [Test]
        public void DerivedFields()
        {
            var fields = typeof(Derived).GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            Assert.AreEqual(10, fields.Length);  // three defined fields, plus backing fields for each of 5 properties, plus inherited public and protected fields
            // NOTE: backing fields for all properties in base class are private, and so don't appear here.

            Assert.IsTrue(fields.Any(f => f.Name == "BasePublicField"));
            Assert.IsTrue(fields.Any(f => f.Name == "BaseProtectedField"));
            Assert.IsFalse(fields.Any(f => f.Name == "BasePrivateField"));

            Assert.IsTrue(fields.Any(f => f.Name == "DerivedPublicField"));
            Assert.IsTrue(fields.Any(f => f.Name == "DerivedProtectedField"));
            Assert.IsTrue(fields.Any(f => f.Name == "DerivedPrivateField"));
        }

        [Test]
        public void DerivedMethods()
        {
            var methods = typeof(Derived).GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            Assert.AreEqual(28, methods.Length);    // 10 for property setters/getters, 3 defined in class, 6 inherited from Object, 2 non-private methods inherited from base, 7 inherited property methods

            Assert.IsTrue(methods.Any(f => f.Name == "BasePublicMethod"));
            Assert.IsTrue(methods.Any(f => f.Name == "BaseProtectedMethod"));
            Assert.IsFalse(methods.Any(f => f.Name == "BasePrivateMethod"));
            Assert.IsTrue(methods.Any(f => f.Name == "get_BasePublicProperty"));
            Assert.IsTrue(methods.Any(f => f.Name == "set_BasePublicProperty"));
            Assert.IsTrue(methods.Any(f => f.Name == "get_BaseProtectedProperty"));
            Assert.IsTrue(methods.Any(f => f.Name == "set_BaseProtectedProperty"));
            Assert.IsFalse(methods.Any(f => f.Name == "get_BasePrivateProperty"));
            Assert.IsFalse(methods.Any(f => f.Name == "set_BasePrivateProperty"));
            Assert.IsTrue(methods.Any(f => f.Name == "get_BasePropertyPrivateSetter"));
            Assert.IsFalse(methods.Any(f => f.Name == "set_BasePropertyPrivateSetter"));
            Assert.IsTrue(methods.Any(f => f.Name == "get_BasePropertyProtectedSetter"));
            Assert.IsTrue(methods.Any(f => f.Name == "set_BasePropertyProtectedSetter"));

            Assert.IsTrue(methods.Any(f => f.Name == "DerivedPublicMethod"));
            Assert.IsTrue(methods.Any(f => f.Name == "DerivedProtectedMethod"));
            Assert.IsTrue(methods.Any(f => f.Name == "DerivedPrivateMethod"));
            Assert.IsTrue(methods.Any(f => f.Name == "get_DerivedPublicProperty"));
            Assert.IsTrue(methods.Any(f => f.Name == "set_DerivedPublicProperty"));
            Assert.IsTrue(methods.Any(f => f.Name == "get_DerivedProtectedProperty"));
            Assert.IsTrue(methods.Any(f => f.Name == "set_DerivedProtectedProperty"));
            Assert.IsTrue(methods.Any(f => f.Name == "get_DerivedPrivateProperty"));
            Assert.IsTrue(methods.Any(f => f.Name == "set_DerivedPrivateProperty"));
            Assert.IsTrue(methods.Any(f => f.Name == "get_DerivedPropertyPrivateSetter"));
            Assert.IsTrue(methods.Any(f => f.Name == "set_DerivedPropertyPrivateSetter"));
            Assert.IsTrue(methods.Any(f => f.Name == "get_DerivedPropertyProtectedSetter"));
            Assert.IsTrue(methods.Any(f => f.Name == "set_DerivedPropertyProtectedSetter"));
        }
    }   
}
