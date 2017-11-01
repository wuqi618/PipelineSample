using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebApiSample.Controllers;
using Xunit;

namespace Tests
{
    public class ValuesControllerTests
    {
        [Fact]
        public void GetShouldSucceed()
        {
            var valuesController = new ValuesController();
            var result = valuesController.Get();
            Assert.Equal(2, result.Count());
        }
    }
}
