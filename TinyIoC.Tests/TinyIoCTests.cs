﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text.RegularExpressions;

namespace TinyIoC.Tests
{
    [TestClass]
    public class TinyIoCTests
    {
        #region Utility Classes / Interfaces
        internal interface ITestInterface
        {
        }

        internal class TestClassDefaultCtor : ITestInterface
        {
            public string Prop1 { get; set; }

            public TestClassDefaultCtor()
            {

            }

            public static ITestInterface CreateNew(TinyIoC container)
            {
                return new TestClassDefaultCtor() { Prop1 = "Testing" };
            }
        }

        internal interface ITestInterace2
        {
        }

        internal class TestClassWithInterfaceDependency : ITestInterace2
        {
            public ITestInterface Dependency { get; set; }

            public int Param1 { get; private set; }

            public string Param2 { get; private set; }

            public TestClassWithInterfaceDependency(ITestInterface dependency)
            {
                Dependency = dependency;
            }

            public TestClassWithInterfaceDependency(ITestInterface dependency, int param1, string param2)
            {
                Dependency = dependency;
                Param1 = param1;
                Param2 = param2;
            }
        }

        internal class TestClassWithDependency
        {
            TestClassDefaultCtor Dependency { get; set; }

            public int Param1 { get; private set; }

            public string Param2 { get; private set; }

            public TestClassWithDependency(TestClassDefaultCtor dependency)
            {
                
            }

            public TestClassWithDependency(TestClassDefaultCtor dependency, int param1, string param2)
            {
                Param1 = param1;
                Param2 = param2;
            }
        }

        internal class TestClassWithDependencyAndParameters
        {
            TestClassDefaultCtor Dependency { get; set; }

            public int Param1 { get; private set; }

            public string Param2 { get; private set; }

            public TestClassWithDependencyAndParameters(TestClassDefaultCtor dependency, int param1, string param2)
            {
                Param1 = param1;
                Param2 = param2;
            }
        }

        internal class TestClassNoInterfaceDefaultCtor
        {
            public TestClassNoInterfaceDefaultCtor()
            {

            }
        }

        internal class TestClassNoInterfaceDependency
        {
            public ITestInterface Dependency { get; set; }

            public TestClassNoInterfaceDependency(ITestInterface dependency)
            {
                Dependency = dependency;
            }
        }
        #endregion

        [TestMethod]
        public void Current_Get_ReturnsInstanceOfTinyIoC()
        {
            var container = TinyIoC.Current;

            Assert.IsInstanceOfType(container, typeof(TinyIoC));
        }

        [TestMethod]
        public void Current_GetTwice_ReturnsSameInstance()
        {
            var container1 = TinyIoC.Current;
            var container2 = TinyIoC.Current;

            Assert.ReferenceEquals(container1, container2);
        }

        [TestMethod]
        public void Register_ImplementationOnly_CanRegister()
        {
            TinyIoC.Register<TestClassDefaultCtor>();

            Assert.IsTrue(true);
        }

        [TestMethod]
        public void Register_InterfaceAndImplementation_CanRegister()
        {
            TinyIoC.Register<ITestInterface, TestClassDefaultCtor>();

            Assert.IsTrue(true);
        }

        [TestMethod]
        [ExpectedException(typeof(TinyIoCResolutionException))]
        public void ClearTypes_RegisteredInterface_CanNoLongerResolveType()
        {
            TinyIoC.Register<ITestInterface, TestClassDefaultCtor>();
            TinyIoC.ClearTypes();

            var result = TinyIoC.Resolve<ITestInterface>();

            Assert.IsTrue(true);
        }

        [TestMethod]
        public void Resolve_RegisteredTypeWithImplementation_ReturnsInstanceOfCorrectType()
        {
            TinyIoC.ClearTypes();
            TinyIoC.Register<ITestInterface, TestClassDefaultCtor>();

            var output = TinyIoC.Resolve<ITestInterface>();

            Assert.IsInstanceOfType(output, typeof(TestClassDefaultCtor));
        }

        [TestMethod]
        public void Resolve_RegisteredTypeImplementationOnly_ReturnsInstanceOfCorrectType()
        {
            TinyIoC.ClearTypes();
            TinyIoC.Register<TestClassDefaultCtor>();

            var output = TinyIoC.Resolve<TestClassDefaultCtor>();

            Assert.IsInstanceOfType(output, typeof(TestClassDefaultCtor));
        }

        [TestMethod]
        public void Register_WithDelegateFactoryStaticMethod_CanRegister()
        {
            TinyIoC.Register<ITestInterface>((c) => TestClassDefaultCtor.CreateNew(c));

            Assert.IsTrue(true);
        }

        [TestMethod]
        public void Register_WithDelegateFactoryLambda_CanRegister()
        {
            TinyIoC.Register<ITestInterface>((c) => new TestClassDefaultCtor() { Prop1 = "Testing" });

            Assert.IsTrue(true);
        }

        [TestMethod]
        public void Resolve_TypeRegisteredWithDelegateFactoryStaticMethod_ResolvesCorrectlyUsingDelegateFactory()
        {
            TinyIoC.ClearTypes();
            TinyIoC.Register<ITestInterface>((c) => TestClassDefaultCtor.CreateNew(c));

            TestClassDefaultCtor output = TinyIoC.Resolve<ITestInterface>() as TestClassDefaultCtor;

            Assert.AreEqual("Testing", output.Prop1);
        }

        [TestMethod]
        public void Resolve_TypeRegisteredWithDelegateFactoryLambda_ResolvesCorrectlyUsingDelegateFactory()
        {
            TinyIoC.ClearTypes();
            TinyIoC.Register<ITestInterface>((c) => new TestClassDefaultCtor() { Prop1 = "Testing" });

            TestClassDefaultCtor output = TinyIoC.Resolve<ITestInterface>() as TestClassDefaultCtor;

            Assert.AreEqual("Testing", output.Prop1);
        }

        [TestMethod]
        public void Resolve_UnregisteredClassTypeWithDefaultCtor_ResolvesType()
        {
            TinyIoC.ClearTypes();
            var output = TinyIoC.Resolve<TestClassDefaultCtor>();

            Assert.IsInstanceOfType(output, typeof(TestClassDefaultCtor));
        }

        [TestMethod]
        [ExpectedException(typeof(TinyIoCResolutionException))]
        public void Resolve_UnregisteredInterface_ThrowsException()
        {
            TinyIoC.ClearTypes();
            var output = TinyIoC.Resolve<ITestInterface>();

            Assert.IsInstanceOfType(output, typeof(TestClassDefaultCtor));
        }

        [TestMethod]
        public void CanResolveType_RegisteredTypeDefaultCtor_ReturnsTrue()
        {
            TinyIoC.ClearTypes();
            TinyIoC.Register<TestClassDefaultCtor>();

            var result = TinyIoC.CanResolve(typeof(TestClassDefaultCtor));

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void CanResolveType_UnregisteredTypeDefaultCtor_ReturnsTrue()
        {
            TinyIoC.ClearTypes();
            var result = TinyIoC.CanResolve(typeof(TestClassDefaultCtor));

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void CanResolveType_UnregisteredInterface_ReturnsFalse()
        {
            TinyIoC.ClearTypes();
            var result = TinyIoC.CanResolve(typeof(ITestInterface));

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void CanResolveType_RegisteredInterface_ReturnsTrue()
        {
            TinyIoC.ClearTypes();
            TinyIoC.Register<ITestInterface, TestClassDefaultCtor>();

            var result = TinyIoC.CanResolve(typeof(ITestInterface));

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void CanResolveType_RegisteredTypeWithRegisteredDependencies_ReturnsTrue()
        {
            TinyIoC.ClearTypes();
            TinyIoC.Register<TestClassDefaultCtor>();
            TinyIoC.Register<TestClassWithDependency>();

            var result = TinyIoC.CanResolve(typeof(TestClassWithDependency));

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void CanResolveType_RegisteredTypeWithRegisteredDependenciesAndParameters_ReturnsTrue()
        {
            TinyIoC.ClearTypes();
            TinyIoC.Register<TestClassDefaultCtor>();
            TinyIoC.Register<TestClassWithDependencyAndParameters>();

            var result = TinyIoC.CanResolve(typeof(TestClassWithDependencyAndParameters), new TinyIoC.NamedParameterOverloads { {"param1", 12}, {"param2", "Testing"} });

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void CanResolveType_RegisteredTypeWithRegisteredDependenciesAndIncorrectParameters_ReturnsFalse()
        {
            TinyIoC.ClearTypes();
            TinyIoC.Register<TestClassDefaultCtor>();
            TinyIoC.Register<TestClassWithDependencyAndParameters>();

            var result = TinyIoC.CanResolve(typeof(TestClassWithDependencyAndParameters), new TinyIoC.NamedParameterOverloads { { "wrongparam1", 12 }, { "wrongparam2", "Testing" } });

            Assert.IsFalse(result);
        }
    }
}
