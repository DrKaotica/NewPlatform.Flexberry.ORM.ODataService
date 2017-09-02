﻿namespace NewPlatform.Flexberry.ORM.ODataService.Tests.Functions
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Web.Script.Serialization;

    using Xunit;

    using NewPlatform.Flexberry.ORM.ODataService.Functions;

    /// <summary>
    /// Unit test class for OData Service user-defined functions
    /// </summary>
    public class DelegateFunctionsTest : BaseODataServiceIntegratedTest
    {
        /// <summary>
        /// Unit test for <see cref="IFunctionContainer.Register(Delegate)"/>.
        /// Tests the function call without query parameters.
        /// </summary>
        [Fact]
        public void TestFunctionCallWithoutQueryParameters()
        {
            ActODataService(args =>
            {
                args.Token.Functions.Register(new Func<int, int, int>(AddWithoutQueryParameters));

                string url = "http://localhost/odata/AddWithoutQueryParameters(a=2,b=2)";
                using (HttpResponseMessage response = args.HttpClient.GetAsync(url).Result)
                {
                    var resultText = response.Content.ReadAsStringAsync().Result;
                    var result = new JavaScriptSerializer().Deserialize<Dictionary<string, object>>(resultText);

                    Assert.Equal(4, result["value"]);
                }
            });
        }

        /// <summary>
        /// Unit test for <see cref="IFunctionContainer.Register(Delegate)"/>.
        /// Tests the function call with query parameters.
        /// </summary>
        [Fact]
        public void TestFunctionCallWithQueryParameters()
        {
            ActODataService(args =>
            {
                args.Token.Functions.Register(new Func<QueryParameters, int, int, int>(AddWithQueryParameters));

                string url = "http://localhost/odata/AddWithQueryParameters(a=2,b=2)";
                using (HttpResponseMessage response = args.HttpClient.GetAsync(url).Result)
                {
                    var resultText = response.Content.ReadAsStringAsync().Result;
                    var result = new JavaScriptSerializer().Deserialize<Dictionary<string, object>>(resultText);

                    Assert.Equal(4, result["value"]);
                }
            });
        }

        private static int AddWithoutQueryParameters(int a, int b)
        {
            return a + b;
        }

        private static int AddWithQueryParameters(QueryParameters @params, int a, int b)
        {
            Assert.NotNull(@params);

            return a + b;
        }
    }
}
