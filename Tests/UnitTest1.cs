using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Web;
using Moq;
using System.Web.Routing;
using UrlsAndRoutes;
using UrlsAndRoutes.Controllers;
using System.Reflection;
using System.Web.Mvc;

namespace Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestIncomingRoutes()
        {
            //check for the url that we hope to achieve
            TestRouteMatch("~/Admin/Index", "Admin", "Index");
            TestRouteMatch("~/One/Two", "One", "Two");
            TestRouteMatch("~/Customer/List/All", "Customer", "List", new { id = "All" });
            TestRouteMatch("~/Customer/", "Customer", "Index");
            TestRouteMatch("~/Customer/List/All/Delete/From", "Customer", "List",
                new { id = "All", catchall = "Delete/From" });

            //ensure that too many or too few segments fails to match
            //            TestRouteFail("~/Admin/Index/Segment");
            //            TestRouteFail("~/Admin");
        }

        [TestMethod]
        public void ViewSelectionTest()
        {
            ExampleController target = new ExampleController();

            ViewResult result = target.Index() as ViewResult;
            Assert.AreEqual("Homepage", result.ViewName);
            Assert.AreEqual("Hello, World", result.ViewData.Model);

            //ref: shared data structure, keys are the same = data is the same
            Assert.AreEqual(result.ViewBag.Message, result.ViewData["Message"]);

            Assert.AreEqual("Hello, again", result.ViewData["Message"]);
        }

        HttpContextBase CreateHttpContext(string targetUrl = null, string httpMethod = "GET")
        {
            Mock<HttpRequestBase> mockRequest = new Mock<HttpRequestBase>();
            mockRequest.Setup(m => m.AppRelativeCurrentExecutionFilePath).Returns(targetUrl);
            mockRequest.Setup(m => m.HttpMethod).Returns(httpMethod);

            Mock<HttpResponseBase> mockResopnse = new Mock<HttpResponseBase>();
            mockResopnse.Setup(m => m.ApplyAppPathModifier(It.IsAny<string>())).Returns<string>(s => s);

            Mock<HttpContextBase> mockContext = new Mock<HttpContextBase>();
            mockContext.Setup(m => m.Request).Returns(mockRequest.Object);
            mockContext.Setup(m => m.Response).Returns(mockResopnse.Object);

            return mockContext.Object;
        }

        void TestRouteMatch(string url, string controller, string action, object routeProperties = null, string httpMethod = "GET")
        {
            RouteCollection routes = new RouteCollection();
            MvcApplication.RegisterRoutes(routes);

            RouteData result = routes.GetRouteData(CreateHttpContext(url, httpMethod));

            Assert.IsNotNull(result);
            Assert.IsTrue(TestIncomingRouteResult(result, controller, action, routeProperties));
        }

        private bool TestIncomingRouteResult(RouteData routeResult, string controller, string action, object propertySet = null)
        {
            Func<object, object, bool> valCompare = (v1, v2) =>
            {
                return StringComparer.InvariantCultureIgnoreCase.Compare(v1, v2) == 0;
            };

            bool result = valCompare(routeResult.Values["controller"], controller)
                && valCompare(routeResult.Values["action"], action);
            //return if false?

            if (propertySet != null)
            {
                PropertyInfo[] propInfo = propertySet.GetType().GetProperties();
                foreach (PropertyInfo pi in propInfo)
                {
                    if (!(routeResult.Values.ContainsKey(pi.Name)
                        && valCompare(routeResult.Values[pi.Name],
                                        pi.GetValue(propertySet, null))))
                    {
                        result = false;
                        break;
                    }
                }
            }
            return result;
        }

        void TestRouteFail(string url)
        {
            RouteCollection routes = new RouteCollection();
            MvcApplication.RegisterRoutes(routes);

            RouteData result = routes.GetRouteData(CreateHttpContext(url));

            Assert.IsTrue(result != null || result.Route == null);
        }
    }
}
