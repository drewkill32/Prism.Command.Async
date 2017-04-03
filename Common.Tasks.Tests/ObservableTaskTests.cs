using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xunit;
using Assert = Xunit.Assert;

namespace Common.Tasks.Tests
{
    [TestClass]
    public class ObservableTaskTests
    {
        [TestMethod]
        public void WhenConstructedWithGenericTypeOfObject_InitializesValues()
        {
            // Prepare
            Task<object> t = Task.Run(() => new object() );
            // Act
            var actual = new ObservableTask<object>(t);

            // verify
            Assert.NotNull(actual);
        }
    }
}
