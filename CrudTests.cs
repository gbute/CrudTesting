using NUnit.Framework;
using System;
using System.IO;
using System.Net;
using static UnitTestProject1.Verbs;

namespace UnitTestProject1
{
    public class Tests
    {
        private static string baseUri = "https://1ryu4whyek.execute-api.us-west-2.amazonaws.com/dev/skus";

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void TestCreateAndGetSku()
        {
            HttpWebRequest httpWebRequest = SetupPost();

            string skuWithGuid = "berliner" + Guid.NewGuid().ToString();

            WriteToStream(httpWebRequest, skuWithGuid);

            string result;
            HttpWebResponse httpResponse;
            GetResponse(httpWebRequest, out result, out httpResponse);

            Assert.IsTrue(result.Contains(skuWithGuid),
                "The result for Sku creation did not contain the Sku we submitted: " + skuWithGuid);

            //Verify as well you can get the newly created object
            HttpWebRequest getHttpWebRequest = SetupGet(httpWebRequest, skuWithGuid);

            GetResponse(getHttpWebRequest, out result, out httpResponse);

            Assert.IsTrue(result.Contains(skuWithGuid),
                "Unable to GET sku that was just created: " + skuWithGuid);
        }

        private static HttpWebRequest SetupGet(HttpWebRequest httpWebRequest, string sku)
        {
            var getHttpWebRequest = (HttpWebRequest)WebRequest.Create(baseUri + "/" + sku);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = HttpVerb.GET.ToString();
            return getHttpWebRequest;
        }

        private static HttpWebRequest SetupPost()
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(baseUri);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = HttpVerb.POST.ToString();
            return httpWebRequest;
        }

        private static void GetResponse(HttpWebRequest httpWebRequest, out string result, out HttpWebResponse httpResponse)
        {
            httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            Assert.AreEqual(httpResponse.StatusCode, HttpStatusCode.OK);
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                result = streamReader.ReadToEnd();
            }
        }

        private static void WriteToStream(HttpWebRequest httpWebRequest, string skuWithGuid)
        {
            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                string json = "{ \"sku\":\"" + skuWithGuid + "\",\"description\": \"Jelly donut\",\"price\":\"2.99\"}";
                streamWriter.Write(json);
            }
        }
    }
}