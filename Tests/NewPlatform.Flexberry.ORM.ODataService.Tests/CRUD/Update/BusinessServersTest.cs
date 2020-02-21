﻿namespace NewPlatform.Flexberry.ORM.ODataService.Tests.CRUD.Update
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;

    using ICSSoft.STORMNET;
    using ICSSoft.STORMNET.KeyGen;
    using ICSSoft.STORMNET.Windows.Forms;

    using NewPlatform.Flexberry.ORM.ODataService.Tests.Extensions;
    using NewPlatform.Flexberry.ORM.ODataService.Tests.Helpers;

    using Newtonsoft.Json;

    using Xunit;

    /// <summary>
    /// Класс тестов для тестирования бизнес-серверов.
    /// </summary>
    public class BusinessServersTest : BaseODataServiceIntegratedTest
    {
        /// <summary>
        /// Осуществляет проверку того, что при POST запросах происходит вызов бизнес-сервера.
        /// </summary>
        [Fact]
        public void BSTest()
        {
            ActODataService(args =>
            {
                args.HttpClient.DefaultRequestHeaders.Add("Prefer", "return=representation");
                ExternalLangDef.LanguageDef.DataService = args.DataService;

                string[] берлогаPropertiesNames =
                {
                    Information.ExtractPropertyPath<Берлога>(x => x.ПолеБС),
                    Information.ExtractPropertyPath<Берлога>(x => x.__PrimaryKey),
                    Information.ExtractPropertyPath<Берлога>(x => x.Наименование),
                    Information.ExtractPropertyPath<Берлога>(x => x.Заброшена)
                };
                string[] медвPropertiesNames =
                {
                    Information.ExtractPropertyPath<Медведь>(x => x.ПолеБС),
                    Information.ExtractPropertyPath<Медведь>(x => x.__PrimaryKey),
                    Information.ExtractPropertyPath<Медведь>(x => x.Вес),

                    // Information.ExtractPropertyPath<Медведь>(x => x.Пол),
                    Information.ExtractPropertyPath<Медведь>(x => x.ДатаРождения),
                    Information.ExtractPropertyPath<Медведь>(x => x.ПорядковыйНомер)
                };
                var берлогаDynamicView = new View(new ViewAttribute("берлогаDynamicView", берлогаPropertiesNames), typeof(Берлога));
                var медвDynamicView = new View(new ViewAttribute("медвDynamicView", медвPropertiesNames), typeof(Медведь));

                // Объекты для тестирования создания.
                Медведь медв = new Медведь { Вес = 48 };
                Лес лес1 = new Лес { Название = "Бор" };
                Лес лес2 = new Лес { Название = "Березовая роща" };
                медв.ЛесОбитания = лес1;
                var берлога1 = new Берлога { Наименование = "Для хорошего настроения", ЛесРасположения = лес1 };
                var берлога2 = new Берлога { Наименование = "Для плохого настроения", ЛесРасположения = лес2 };
                медв.Берлога.Add(берлога1);
                медв.Берлога.Add(берлога2);

                var objs = new DataObject[] { медв };
                args.DataService.UpdateObjects(ref objs);

                // Проверим, что через сервис данных БС отрабатывает корректно.
                медв.ЛесОбитания = лес2;
                args.DataService.UpdateObject(медв);
                Assert.Equal($"Медведь обитает в {лес2.Название}", медв.ПолеБС);

                // Сделаем тоже самое, но через OData.
                string requestUrl;

                string requestJsonDataМедв = медв.ToJson(медвDynamicView, args.Token.Model);
                DataObjectDictionary objJsonМедв = DataObjectDictionary.Parse(requestJsonDataМедв, медвDynamicView, args.Token.Model);

                objJsonМедв.Add(
                    $"{nameof(Медведь.ЛесОбитания)}@odata.bind",
                    string.Format(
                        "{0}({1})",
                        args.Token.Model.GetEdmEntitySet(typeof(Лес)).Name,
                        ((KeyGuid)лес1.__PrimaryKey).Guid.ToString("D")));

                requestJsonDataМедв = objJsonМедв.Serialize();
                requestUrl = string.Format(
                    "http://localhost/odata/{0}({1})",
                    args.Token.Model.GetEdmEntitySet(typeof(Медведь)).Name,
                    ((KeyGuid)медв.__PrimaryKey).Guid.ToString());

                using (HttpResponseMessage response = args.HttpClient.PatchAsJsonStringAsync(requestUrl, requestJsonDataМедв).Result)
                {
                    Assert.Equal(HttpStatusCode.OK, response.StatusCode);

                    string receivedJsonObjs = response.Content.ReadAsStringAsync().Result.Beautify();
                    Dictionary<string, object> receivedObjs = JsonConvert.DeserializeObject<Dictionary<string, object>>(receivedJsonObjs);

                    Assert.Equal($"Медведь обитает в {лес1.Название}", receivedObjs[nameof(Медведь.ПолеБС)]);
                }

                // Проверим, что через сервис данных БС отрабатывает корректно.
                берлога1.ЛесРасположения = лес2;
                args.DataService.UpdateObject(берлога1);
                Assert.Equal($"Берлога расположена в {лес2.Название}", берлога1.ПолеБС);

                // Сделаем тоже самое, но через OData.
                string requestJsonDataБерлога = берлога1.ToJson(берлогаDynamicView, args.Token.Model);
                var objJsonБерлога = DataObjectDictionary.Parse(requestJsonDataБерлога, берлогаDynamicView, args.Token.Model);

                objJsonБерлога.Add(
                    $"{nameof(Берлога.ЛесРасположения)}@odata.bind",
                    string.Format(
                        "{0}({1})",
                        args.Token.Model.GetEdmEntitySet(typeof(Лес)).Name,
                        ((KeyGuid)лес1.__PrimaryKey).Guid.ToString("D")));

                objJsonБерлога.Add(
                    $"{nameof(Берлога.Медведь)}@odata.bind",
                    string.Format(
                        "{0}({1})",
                        args.Token.Model.GetEdmEntitySet(typeof(Медведь)).Name,
                        ((KeyGuid)медв.__PrimaryKey).Guid.ToString("D")));

                requestJsonDataБерлога = objJsonБерлога.Serialize();
                requestUrl = string.Format("http://localhost/odata/{0}({1})", args.Token.Model.GetEdmEntitySet(typeof(Берлога)).Name, ((KeyGuid)берлога1.__PrimaryKey).Guid.ToString());

                using (HttpResponseMessage response = args.HttpClient.PatchAsJsonStringAsync(requestUrl, requestJsonDataБерлога).Result)
                {
                    Assert.Equal(HttpStatusCode.OK, response.StatusCode);

                    string receivedJsonObjs = response.Content.ReadAsStringAsync().Result.Beautify();
                    Dictionary<string, object> receivedObjs = JsonConvert.DeserializeObject<Dictionary<string, object>>(receivedJsonObjs);

                    Assert.Equal($"Берлога расположена в {лес1.Название}", receivedObjs[nameof(Берлога.ПолеБС)]);
                }
            });
        }

        /// <summary>
        /// Test to check the call business server of aggregator when adding detail through batch request with aggregator.
        /// </summary>
        [Fact]
        public void CallAggregatorBSOnAddDetailTest()
        {
            ActODataService(async (args) =>
            {
                var медведь = new Медведь();
                медведь.Берлога.Add(new Берлога());

                args.DataService.UpdateObject(медведь);

                var новаяБерлога = new Берлога();
                медведь.Берлога.Add(новаяБерлога);

                const string baseUrl = "http://localhost/odata";

                string[] changesets = new[]
                {
                    CreateChangeset(
                        $"{baseUrl}/{args.Token.Model.GetEdmEntitySet(typeof(Медведь)).Name}",
                        "{}",
                        медведь),
                    CreateChangeset(
                        $"{baseUrl}/{args.Token.Model.GetEdmEntitySet(typeof(Берлога)).Name}",
                        новаяБерлога.ToJson(Берлога.Views.БерлогаE, args.Token.Model),
                        новаяБерлога),
                };
                HttpRequestMessage batchRequest = CreateBatchRequest(baseUrl, changesets);
                using (HttpResponseMessage response = await args.HttpClient.SendAsync(batchRequest))
                {
                    Assert.Equal(HttpStatusCode.OK, response.StatusCode);

                    args.DataService.LoadObject(Медведь.Views.МедведьE, медведь);

                    var берлоги = медведь.Берлога.GetAllObjects().Cast<Берлога>();

                    Assert.Equal(1, берлоги.Count(б => б.Заброшена));
                    Assert.Equal(1, берлоги.Count(б => !б.Заброшена));
                }
            });
        }

        /// <summary>
        /// Test to check the call business server of aggregator when updating detail through batch request with aggregator.
        /// </summary>
        [Fact]
        public void CallAggregatorBSOnUpdateDetailTest()
        {
            ActODataService(async (args) =>
            {
                var медведь = new Медведь();
                медведь.Берлога.Add(new Берлога() { Заброшена = true });
                медведь.Берлога.Add(new Берлога() { Заброшена = true });

                args.DataService.UpdateObject(медведь);

                медведь.Берлога[0].Комфортность += 1;

                const string baseUrl = "http://localhost/odata";

                string[] changesets = new[]
                {
                    CreateChangeset(
                        $"{baseUrl}/{args.Token.Model.GetEdmEntitySet(typeof(Медведь)).Name}",
                        "{}",
                        медведь),
                    CreateChangeset(
                        $"{baseUrl}/{args.Token.Model.GetEdmEntitySet(typeof(Берлога)).Name}",
                        медведь.Берлога[0].ToJson(Берлога.Views.БерлогаE, args.Token.Model),
                        медведь.Берлога[0]),
                };

                using (HttpResponseMessage response = await args.HttpClient.SendAsync(CreateBatchRequest(baseUrl, changesets)))
                {
                    Assert.Equal(HttpStatusCode.OK, response.StatusCode);

                    args.DataService.LoadObject(Медведь.Views.МедведьE, медведь);

                    var берлоги = медведь.Берлога.GetAllObjects().Cast<Берлога>();
                    var комфортнаяБерлога = берлоги.FirstOrDefault(б => б.Комфортность == 1);

                    Assert.False(комфортнаяБерлога?.Заброшена);
                    Assert.Equal(1, берлоги.Count(б => б.Заброшена));
                }
            });
        }

        /// <summary>
        /// Test to check the call business server of aggregator when deleting detail through batch request with aggregator.
        /// </summary>
        [Fact]
        public void CallAggregatorBSOnDeleteDetailTest()
        {
            ActODataService(async (args) =>
            {
                var медведь = new Медведь();
                медведь.Берлога.Add(new Берлога());
                медведь.Берлога.Add(new Берлога());

                args.DataService.UpdateObject(медведь);

                медведь.Берлога[0].SetStatus(ObjectStatus.Deleted);

                const string baseUrl = "http://localhost/odata";

                string[] changesets = new[]
                {
                    CreateChangeset($"{baseUrl}/{args.Token.Model.GetEdmEntitySet(typeof(Медведь)).Name}", "{}", медведь),
                    CreateChangeset($"{baseUrl}/{args.Token.Model.GetEdmEntitySet(typeof(Берлога)).Name}", string.Empty, медведь.Берлога[0]),
                };

                using (HttpResponseMessage response = await args.HttpClient.SendAsync(CreateBatchRequest(baseUrl, changesets)))
                {
                    Assert.Equal(HttpStatusCode.OK, response.StatusCode);

                    args.DataService.LoadObject(Медведь.Views.МедведьE, медведь);

                    Assert.Equal(1, медведь.Берлога.GetAllObjects().Cast<Берлога>().First().Комфортность);
                }
            });
        }

        private HttpRequestMessage CreateBatchRequest(string url, string[] changesets)
        {
            string boundary = $"batch_{Guid.NewGuid()}";
            string body = CreateBatchBody(boundary, changesets);

            var request = new HttpRequestMessage()
            {
                RequestUri = new Uri($"{url}/$batch"),
                Method = new HttpMethod("POST"),
                Content = new StringContent(body),
            };

            request.Content.Headers.ContentType.MediaType = "multipart/mixed";
            request.Content.Headers.ContentType.Parameters.Add(new NameValueHeaderValue("boundary", boundary));

            return request;
        }

        private string CreateBatchBody(string boundary, string[] changesets)
        {
            var body = new StringBuilder($"--{boundary}");
            body.AppendLine();

            string changesetBoundary = $"changeset_{Guid.NewGuid()}";

            body.AppendLine($"Content-Type: multipart/mixed;boundary={changesetBoundary}");
            body.AppendLine();

            for (var i = 0; i < changesets.Length; i++)
            {
                body.AppendLine($"--{changesetBoundary}");
                body.AppendLine($"Content-Type: application/http");
                body.AppendLine($"Content-Transfer-Encoding: binary");
                body.AppendLine($"Content-ID: {i + 1}");
                body.AppendLine();

                body.AppendLine(changesets[i]);

                body.AppendLine($"--{changesetBoundary}--");
            }

            body.AppendLine($"--{boundary}--");
            body.AppendLine();

            return body.ToString();
        }

        private string CreateChangeset(string url, string body, DataObject dataObject)
        {
            var changeset = new StringBuilder();

            changeset.AppendLine($"{GetMethodAndUrl(dataObject, url)} HTTP/1.1");
            changeset.AppendLine($"Content-Type: application/json;type=entry");
            changeset.AppendLine($"Prefer: return=representation");
            changeset.AppendLine();

            changeset.AppendLine(body);
            changeset.AppendLine();

            return changeset.ToString();
        }

        private string GetMethodAndUrl(DataObject dataObject, string url)
        {
            switch (dataObject.GetStatus())
            {
                case ObjectStatus.Created:
                    return $"POST {url}";

                case ObjectStatus.Altered:
                case ObjectStatus.UnAltered:
                    return $"PATCH {url}({dataObject.__PrimaryKey})";

                case ObjectStatus.Deleted:
                    return $"DELETE {url}({dataObject.__PrimaryKey})";

                default:
                    throw new InvalidOperationException();
            }
        }
    }
}
