using NUnit.Framework;
using System;
using System.IO;
using System.Net;
using static CrudTests.Verbs;
using System.Text.Json;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Assert = NUnit.Framework.Assert;
using System.Collections.Generic;

namespace CrudTests
{
    public class Item
    {
        public DateTimeOffset CreatedAt;
        public string Description;
        public decimal Price;
        public string Sku;
        public DateTimeOffset UpdatedAt;
    }


    public class Tests
    {
        private static string baseUri = "https://1ryu4whyek.execute-api.us-west-2.amazonaws.com/dev/skus";
        public List<string> aFewLanguages = new List<string>();

        [SetUp]
        public void Setup()
        {
            aFewLanguages.Add("The quick brown fox jumps over the lazy dog");
            //Danish
            aFewLanguages.Add("Quizdeltagerne spiste jordbær med fløde, mens cirkusklovnen Wolther spillede på xylofon.");
            //German
            aFewLanguages.Add("Victor jagt zwölf Boxkämpfer quer über den großen Sylter Deich");
            //Italian
            aFewLanguages.Add("Pranzo d'acqua fa volti sghembi");
            //Polish
            aFewLanguages.Add("Stróż pchnął kość w quiz gędźb vel fax myjń ");
            //Turkish
            aFewLanguages.Add("Pijamalı hasta yağız şoföre çabucak güvendi ");
        }

        [Test]
        public void CreateAndGetSkuTest()
        {
            Item itemForSale = new Item()
            {
                Description = "Jelly Donut",
                Price = 0.57M,
                Sku = Guid.NewGuid().ToString()
            };

            CreateOrUpdateSku(itemForSale);

            GetAndValidateSku(itemForSale);
        }

        [Test]
        public void UpdateSkuTest()
        {           
            Item itemForSale = new Item()
            {
                Description = "Jelly Donut",
                Price = 0.57M,
                Sku = Guid.NewGuid().ToString()
            };

            CreateOrUpdateSku(itemForSale);
            
            Item updatedItemForSale = new Item()
            {
                Description = "Maple Bar",
                Price = 0.58M,
                Sku = itemForSale.Sku
            };

            CreateOrUpdateSku(updatedItemForSale);

            GetAndValidateSku(updatedItemForSale);
        }

        [Test]
        public void DeleteSkuTest()
        {
            Item itemForSale = new Item()
            {
                Description = "Jelly Donut",
                Price = 0.57M,
                Sku = Guid.NewGuid().ToString()
            };

            CreateOrUpdateSku(itemForSale);

            DeleteSku(itemForSale.Sku);

            GetAndValidateSku(itemForSale, itemDeleted: true);
        }

        [Test]
        public void CreateSkuBlankDescriptionTest()
        {
            Item itemForSale = new Item()
            {
                Description = "",
                Price = 0.57M,
                Sku = Guid.NewGuid().ToString()
            };

            CreateOrUpdateSku(itemForSale);

            GetAndValidateSku(itemForSale);
        }

        [Test]
        public void CreateSkuNullDescriptionTest()
        {
            Item itemForSale = new Item()
            {
                Description = null,
                Price = 0.57M,
                Sku = Guid.NewGuid().ToString()
            };

            CreateOrUpdateSku(itemForSale);

            GetAndValidateSku(itemForSale);
        }

        [Test]
        public void CreateSkuNegativePriceTest()
        {
            Item itemForSale = new Item()
            {
                Description = null,
                Price = -0.57M,
                Sku = Guid.NewGuid().ToString()
            };

            CreateOrUpdateSku(itemForSale);

            GetAndValidateSku(itemForSale);
        }

        [Test]
        public void CreateSkuMaximumPriceTest()
        {
            Item itemForSale = new Item()
            {
                Description = null,
                Price = decimal.MaxValue,
                Sku = Guid.NewGuid().ToString()
            };

            CreateOrUpdateSku(itemForSale);

            GetAndValidateSku(itemForSale);
        }

        [Test]
        public void CreateSkuMinimumPriceTest()
        {
            Item itemForSale = new Item()
            {
                Description = null,
                Price = decimal.MinValue,
                Sku = Guid.NewGuid().ToString()
            };

            CreateOrUpdateSku(itemForSale);

            GetAndValidateSku(itemForSale);
        }

        [Test]
        public void CreateAndGetSkuLargeDescriptionTest()
        {
            Item itemForSale = new Item()
            {
                Description = "Jelly Donut",
                Price = 0.57M,
                Sku = Guid.NewGuid().ToString()
            };

            CreateOrUpdateSku(itemForSale);

            GetAndValidateSku(itemForSale);
        }

        [Test]
        public void DescriptionsInAFewLanguagesTest()
        {
            foreach (string description in aFewLanguages)
            {
                Item itemForSale = new Item()
                {
                    Description = description,
                    Price = 0.57M,
                    Sku = Guid.NewGuid().ToString()
                };

                CreateOrUpdateSku(itemForSale);

                GetAndValidateSku(itemForSale);
            }
        }

        #region ExpectedFailures
        [Test]
        [ExpectedException(typeof(WebException))]
        public void CreateSkuNoSkuProvidedTest()
        {
            Item itemForSale = new Item()
            {
                Description = null,
                Price = decimal.MinValue,
                Sku = null
            };

            try
            {
                CreateOrUpdateSku(itemForSale);

                GetAndValidateSku(itemForSale);
            }
            catch (WebException webEx)
            {
                Assert.IsTrue(webEx.Message.Contains("Bad Gateway"));
            }
            catch (Exception ex)
            {
                Assert.Fail("Unexpected exception: " + ex.StackTrace);
            }
        }
        #endregion ExpectedFailures

        #region private
        private static void GetAndValidateSku(Item expectedItemForSale, bool itemDeleted = false)
        {
            HttpWebRequest getHttpWebRequest = SetupGet(expectedItemForSale.Sku);

            string result;
            result = CallApi(getHttpWebRequest);

            Item itemForSale = DeserializeItem(result, itemDeleted);
            if (itemDeleted)
            {
                Assert.IsNull(itemForSale, "Deleted sku should return null object but didn't: " + itemForSale?.ToString());
            }
            else
            {
                Assert.AreEqual(expectedItemForSale.Sku, itemForSale.Sku, "Unable to GET expected sku: " + expectedItemForSale.Sku);
                Assert.AreEqual(expectedItemForSale.Description ?? string.Empty, itemForSale.Description, "Description not as expected.");
                Assert.AreEqual(expectedItemForSale.Price, itemForSale.Price, "Price not as expected.");
            }
        }

        private static Item DeserializeItem(string json, bool deleted = false)
        {
            JObject jObject = JObject.Parse(json);
            JToken jItem = jObject["Item"];
            if (deleted && jItem == null)
            {
                return null;
            }

            Item item = new Item();
            item.Description = (string)jItem["description"];
            item.Price = (decimal)jItem["price"];
            item.Sku = (string)jItem["sku"];

            return item;
        }

        private static string CreateOrUpdateSku(Item itemForSale)
        {
            HttpWebRequest httpWebRequest = SetupPost();
            WriteToStream(httpWebRequest, itemForSale);
            string result = CallApi(httpWebRequest);
            return result;
        }

        private static string DeleteSku(string sku)
        {
            HttpWebRequest httpWebRequest = SetupDelete(sku);
            //WriteToStream(httpWebRequest, itemForSale);
            string result = CallApi(httpWebRequest);
            return result;
        }

        private static HttpWebRequest SetupGet(string sku)
        {
            HttpWebRequest getHttpWebRequest = (HttpWebRequest)WebRequest.Create(baseUri + "/" + sku);
            getHttpWebRequest.ContentType = "application/json";
            getHttpWebRequest.Method = HttpVerb.GET.ToString();
            return getHttpWebRequest;
        }

        private static HttpWebRequest SetupPost()
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(baseUri);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = HttpVerb.POST.ToString();
            return httpWebRequest;
        }

        private static HttpWebRequest SetupDelete(string sku)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(baseUri + "/" + sku);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = HttpVerb.DELETE.ToString();
            return httpWebRequest;
        }

        private static string CallApi(HttpWebRequest httpWebRequest)
        {
            string result;

            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            
            Assert.AreEqual(httpResponse.StatusCode, HttpStatusCode.OK);
            
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                result = streamReader.ReadToEnd();
            }

            return result;
        }

        private static void WriteToStream(HttpWebRequest httpWebRequest, Item itemForSale)
        {
            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                string json = "{ \"sku\":\"" + itemForSale.Sku + "\",\"description\": \"" + itemForSale.Description + "\",\"price\":\"" +
                    String.Format("{0:0.00}", itemForSale.Price) + "\"}";
                streamWriter.Write(json);
            }
        }
        #endregion private

    }
}